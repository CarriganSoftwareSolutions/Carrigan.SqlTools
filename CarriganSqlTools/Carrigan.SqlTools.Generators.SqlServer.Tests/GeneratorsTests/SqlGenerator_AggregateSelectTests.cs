using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

//IGNORE SPELLING: Ungrouped

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public class SqlGenerator_AggregateSelectTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();
    private static readonly SqlGenerator<Grades> gradesGenerator = new();
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void AggregateSelectTag_RendersExpressionAndAlias()
    {
        SelectTag select = new(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount");

        Assert.Equal("COUNT([Customer].[Id]) AS [TotalCount]", select.ToSql(Dialect));
        Assert.Equal("COUNT(Customer.Id) AS TotalCount", select.ToString());
        Assert.Single(select.TableTags);
    }


    [Fact]
    public void CountStarSelectTag_RendersExpressionAndAlias()
    {
        SelectTag select = new(new Count(), "TotalCount");

        Assert.Equal("COUNT(*) AS [TotalCount]", select.ToSql(Dialect));
        Assert.Equal("COUNT(*) AS TotalCount", select.ToString());
        Assert.Empty(select.TableTags);
    }

    [Fact]
    public void Select_WithAggregateOnly_AllowsAggregateSelectList()
    {
        SelectTags selects = new(new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount"));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null);

        Assert.Equal("SELECT COUNT([Customer].[Id]) AS [TotalCount] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void Select_WithDistinctCount_RendersExpectedSql()
    {
        SelectTags selects = new(new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id)), true), "TotalCount"));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(DISTINCT [Customer].[Id]) AS [TotalCount] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void Select_WithCountStar_AllowsAggregateSelectListWithoutSelectedTableTag()
    {
        SelectTags selects = new(new SelectTag(new Count(), "TotalCount"));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(*) AS [TotalCount] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void AggregateExpressions_AreAggregate()
    {
        Assert.True(new Count().IsAggregate());
        Assert.True(new Count(new Column<Customer>(nameof(Customer.Id))).IsAggregate());
        Assert.True(new Sum(new Column<Order>(nameof(Order.Total))).IsAggregate());
        Assert.True(new Avg(new Column<Order>(nameof(Order.Total))).IsAggregate());
        Assert.True(new Average(new Column<Order>(nameof(Order.Total))).IsAggregate());
        Assert.True(new Min(new Column<Order>(nameof(Order.Total))).IsAggregate());
        Assert.True(new Max(new Column<Order>(nameof(Order.Total))).IsAggregate());
    }

    [Fact]
    public void Column_IsNotAggregate()
    {
        Assert.False(new Column<Customer>(nameof(Customer.Email)).IsAggregate());
        Assert.False(new Column<Customer>(nameof(Customer.Name)).IsAggregate());
    }

    [Fact]
    [Obsolete]
    public void ContainsAggregate_WithNestedAggregate_ReturnsTrue()
    {
        SqlExpression expression = new LessThan
        (
            new Min(new Column<Customer>(nameof(Customer.Id))),
            new Parameter(1)
        );

        Assert.True(expression.ContainsAggregate());
        Assert.True(SqlExpression.ContainsAggregate(expression));
    }

    [Fact]
    [Obsolete]
    public void ContainsAggregate_WithoutAggregate_ReturnsFalse()
    {
        SqlExpression expression = new LessThan
        (
            new Column<Customer>(nameof(Customer.Id)),
            new Parameter(1)
        );

        Assert.False(expression.ContainsAggregate());
        Assert.False(SqlExpression.ContainsAggregate(expression));
    }

    [Fact]
    public void SqlExpression_AllParticipatingTables_ReturnsNestedColumnTable()
    {
        Count count = new(new Column<Order>(nameof(Order.Total)));

        Assert.Empty(count.LeafTables);
        Assert.Equal("Order", Assert.Single(count.AllParticipatingTables).ToString());
    }

    [Fact]
    [Obsolete("Tests obsolete code.")]
    public void SelectTagGenerator_GetManyFromGroupBys_ReturnsSelectsForEachGroupBy()
    {
        GroupBys groupBys = new GroupBys<Customer>(nameof(Customer.Name))
            .Append<Customer>(nameof(Customer.Email));

        IEnumerable<string> actual = SelectTagGenerator.GetMany(groupBys).Select(select => select.ToSql(Dialect));

        Assert.Equal(["[Customer].[Name]", "[Customer].[Email]"], actual);
    }

    [Fact]
    public void SelectTag_IsAggregate_DelegatesToSqlExpression()
    {       
        SelectTag columnSelect = SelectTagGenerator.Get<Customer>(nameof(Customer.Name));
        SelectTag aggregateSelect = new(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount");

        Assert.False(columnSelect.IsAggregate());
        Assert.True(aggregateSelect.IsAggregate());
    }

    [Fact]
    public void Select_WithGroupBysAndNoSelects_UsesGroupByColumnsAsSelects()
    {
        GroupBys groupBys = new GroupBys<Customer>(nameof(Customer.Name));
        Assert.Throws<GroupByRequiresSelectException>(() => customerGenerator.InternalSelect(null, null, null, null, null, groupBys, null, null, null));
    }

    [Fact]
    public void Select_WithGroupedColumnAndAggregate_AllowsAggregateSelectList()
    {
        GroupBys groupBys = new GroupBys<Customer>(nameof(Customer.Name));
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount")
        );

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT [Customer].[Name], COUNT([Customer].[Id]) AS [TotalCount] FROM [Customer] GROUP BY [Customer].[Name]", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupedColumnInExpressionContainingAggregate_AllowsAggregateSelectList()
    {
        GroupBys groupBys = new GroupBys<Grades>(nameof(Grades.AcademicYear));
        SelectTags selects = new
        (
            new SelectTag
            (
                new Add
                (
                    new Column<Grades>(nameof(Grades.AcademicYear)),
                    new Sum(new Column<Grades>(nameof(Grades.CreditHours)))
                ),
                "Value"
            )
        );

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT ([Grades].[AcademicYear] + SUM([Grades].[CreditHours])) AS [Value] FROM [Grades] GROUP BY [Grades].[AcademicYear]", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupedSubexpressionInsideMixedAggregate_AllowsAggregateSelectList()
    {
        Add groupedExpression = new
        (
            new Column<Grades>(nameof(Grades.AcademicYear)),
            new Column<Grades>(nameof(Grades.SemesterNumber))
        );
        GroupBys groupBys = new(new GroupBy(groupedExpression));
        SelectTags selects = new
        (
            new SelectTag
            (
                new Add(groupedExpression, new Sum(new Column<Grades>(nameof(Grades.CreditHours)))),
                "Value"
            )
        );

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT (([Grades].[AcademicYear] + [Grades].[SemesterNumber]) + SUM([Grades].[CreditHours])) AS [Value] FROM [Grades] GROUP BY ([Grades].[AcademicYear] + [Grades].[SemesterNumber])", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupByAndMatchingSelectWithoutAggregate_AllowsSelectList()
    {
        GroupBys groupBys = new GroupBys<Customer>(nameof(Customer.Name));
        SelectTags selects = new(SelectTagGenerator.Get<Customer>(nameof(Customer.Name)));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT [Customer].[Name] FROM [Customer] GROUP BY [Customer].[Name]", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupByAndUngroupedSelectWithoutAggregate_Throws()
    {
        GroupBys groupBys = new GroupBys<Customer>(nameof(Customer.Name));
        SelectTags selects = new(SelectTagGenerator.Get<Customer>(nameof(Customer.Email)));

        Assert.Throws<MixedAggregateSelectException>(() => customerGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null));
    }

    [Fact]
    public void Select_WithHavingAndUngroupedSelectWithoutGroupBy_Throws()
    {
        SelectTags selects = new(SelectTagGenerator.Get<Customer>(nameof(Customer.Name)));
        Predicates having = new GreaterThan(new Count(), new Parameter(1, "MinimumCount"));

        Assert.Throws<MixedAggregateSelectException>(() => customerGenerator.InternalSelect(null, null, selects, null, null, null, having, null, null));
    }

    [Fact]
    public void Select_WithUngroupedColumnInExpressionContainingAggregate_Throws()
    {
        SelectTags selects = new
        (
            new SelectTag
            (
                new Add
                (
                    new Column<Grades>(nameof(Grades.AcademicYear)),
                    new Sum(new Column<Grades>(nameof(Grades.CreditHours)))
                ),
                "Value"
            )
        );

        Assert.Throws<MixedAggregateSelectException>(() => gradesGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null));
    }

    [Fact]
    public void Select_WithMixedAggregateAndNonAggregateSelects_Throws()
    {
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount")
        );

        Assert.Throws<MixedAggregateSelectException>(() => customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null));
    }


    [Fact]
    public void Select_WithGroupedColumnAndAggregateAndHaving_AllowsAggregateSelectList()
    {
        GroupBys groupBys = new GroupBys<Grades>(nameof(Grades.StudentId), nameof(Grades.AcademicYear), nameof(Grades.SemesterNumber));
        Average semesterGpa = new(new Column<Grades>(nameof(Grades.GradePoint)));

        SelectTags selects = new
        (
            SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.AcademicYear)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.SemesterNumber)),
            new SelectTag(semesterGpa, "SemesterGPA")
        );

        Predicates having = new GreaterThan(semesterGpa, new Parameter(3.5, "HonorRollGpa"));

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, having, null, null);
        Assert.Equal("SELECT [Grades].[StudentId], [Grades].[AcademicYear], [Grades].[SemesterNumber], AVG([Grades].[GradePoint]) AS [SemesterGPA] FROM [Grades] GROUP BY [Grades].[StudentId], [Grades].[AcademicYear], [Grades].[SemesterNumber] HAVING (AVG([Grades].[GradePoint]) > @HonorRollGpa_1)", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupedExpressionInHaving_AllowsHaving()
    {
        Add groupedExpression = new
        (
            new Column<Grades>(nameof(Grades.AcademicYear)),
            new Column<Grades>(nameof(Grades.SemesterNumber))
        );
        GroupBys groupBys = new(new GroupBy(groupedExpression));
        SelectTags selects = new
        (
            new SelectTag(groupedExpression, "AcademicPeriod"),
            new SelectTag(new Count(), "TotalCount")
        );
        Predicates having = new GreaterThan(groupedExpression, new Parameter(2026, "MinimumPeriod"));

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, having, null, null);

        Assert.Equal("SELECT ([Grades].[AcademicYear] + [Grades].[SemesterNumber]) AS [AcademicPeriod], COUNT(*) AS [TotalCount] FROM [Grades] GROUP BY ([Grades].[AcademicYear] + [Grades].[SemesterNumber]) HAVING (([Grades].[AcademicYear] + [Grades].[SemesterNumber]) > @MinimumPeriod_1)", query.QueryText);
    }

    [Fact]
    public void Select_WithUngroupedColumnInHaving_Throws()
    {
        GroupBys groupBys = new GroupBys<Grades>(nameof(Grades.AcademicYear));
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Grades>(nameof(Grades.AcademicYear)),
            new SelectTag(new Count(), "TotalCount")
        );
        Predicates having = new GreaterThan(new Column<Grades>(nameof(Grades.SemesterNumber)), new Parameter(1, "MinimumSemester"));

        Assert.Throws<UngroupedColumnInHavingClauseException>(() => gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, having, null, null));
    }

    [Fact]
    public void Select_WithAggregateInGroupBy_Throws()
    {
        GroupBys groupBys = new
        (
            new GroupBy
            (
                new Add
                (
                    new Column<Grades>(nameof(Grades.AcademicYear)),
                    new Sum(new Column<Grades>(nameof(Grades.CreditHours)))
                )
            )
        );
        SelectTags selects = new(new SelectTag(new Sum(new Column<Grades>(nameof(Grades.CreditHours))), "TotalCredits"));

        Assert.Throws<AggregateExpressionInGroupByClauseException>(() => gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null));
    }

    [Fact]
    public void Select_WithHavingFromUnjoinedTable_ThrowsInvalidTableException()
    {
        Predicates having = new GreaterThan
        (
            new Sum(new Column<Order>(nameof(Order.Total))),
            new Parameter(100m, "MinimumTotal")
        );

        Assert.Throws<InvalidTableException>(() => customerGenerator.InternalSelect(null, null, null, null, null, null, having, null, null));
    }

    [Fact]
    public void Select_WithWhereAndHaving_PreservesParameterOrder()
    {
        GroupBys groupBys = new GroupBys<Grades>(nameof(Grades.StudentId));
        Average averageGradePoint = new(new Column<Grades>(nameof(Grades.GradePoint)));
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
            new SelectTag(averageGradePoint, "AverageGradePoint")
        );
        Predicates where = new GreaterThan
        (
            new Column<Grades>(nameof(Grades.AcademicYear)),
            new Parameter(2000, "MinimumYear")
        );
        Predicates having = new GreaterThan
        (
            averageGradePoint,
            new Parameter(3.5m, "MinimumGpa")
        );

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, where, groupBys, having, null, null);

        Assert.Equal("SELECT [Grades].[StudentId], AVG([Grades].[GradePoint]) AS [AverageGradePoint] FROM [Grades] WHERE ([Grades].[AcademicYear] > @MinimumYear_1) GROUP BY [Grades].[StudentId] HAVING (AVG([Grades].[GradePoint]) > @MinimumGpa_2)", query.QueryText);
        Assert.Equal(2, query.Parameters.Count());
    }

    [Fact]
    public void Select_WithAggregateOnlyHavingWithoutGroupBy_RendersExpectedSql()
    {
        Count count = new();
        SelectTags selects = new(new SelectTag(count, "TotalCount"));
        Predicates having = new GreaterThan(count, new Parameter(1, "MinimumCount"));

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, having, null, null);

        Assert.Equal("SELECT COUNT(*) AS [TotalCount] FROM [Customer] HAVING (COUNT(*) > @MinimumCount_1)", query.QueryText);
    }

    [Fact]
    public void Select_Parameter_With_Aggregates()
    {
        GroupBys groupBys = new GroupBys<Grades>(nameof(Grades.StudentId), nameof(Grades.AcademicYear), nameof(Grades.SemesterNumber));
        Average semesterGpa = new(new Column<Grades>(nameof(Grades.GradePoint)));

        SelectTags selects = new
        (
            SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.AcademicYear)),
            SelectTagGenerator.Get<Grades>(nameof(Grades.SemesterNumber)),
            new SelectTag(semesterGpa, "SemesterGPA"),
            new SelectTag(new Parameter(1), "ParamterValue")
        );

        Predicates having = new GreaterThan(semesterGpa, new Parameter(3.5, "HonorRollGpa"));

        SqlQuery query = gradesGenerator.InternalSelect(null, null, selects, null, null, groupBys, having, null, null);
        Assert.Equal("SELECT [Grades].[StudentId], [Grades].[AcademicYear], [Grades].[SemesterNumber], AVG([Grades].[GradePoint]) AS [SemesterGPA], @Parameter_1 AS [ParamterValue] FROM [Grades] GROUP BY [Grades].[StudentId], [Grades].[AcademicYear], [Grades].[SemesterNumber] HAVING (AVG([Grades].[GradePoint]) > @HonorRollGpa_2)", query.QueryText);
    }

    [Fact]
    public void Select_WithCountStarAndParameter_AllowsAggregateSelectList()
    {
        SelectTags selects = new
        (
            new SelectTag(new Count(), "TotalCount"),
            new SelectTag(new Parameter(1), "ParameterValue")
        );

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(*) AS [TotalCount], @Parameter_1 AS [ParameterValue] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void Select_WithUngroupedColumnAndCountStar_Throws()
    {
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(), "TotalCount")
        );

        Assert.Throws<MixedAggregateSelectException>(() => customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null));
    }

    [Fact]
    public void Select_WithNestedCastsUngroupedColumnAndCountStar_Throws()
    {
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name), "NameValue", new FieldProperties()),
            new SelectTag(new Count(), "TotalCount")
        );

        Assert.Throws<MixedAggregateSelectException>(() => customerGenerator.InternalSelect(null, null, selects, null, null, null, null, null, null));
    }

}
