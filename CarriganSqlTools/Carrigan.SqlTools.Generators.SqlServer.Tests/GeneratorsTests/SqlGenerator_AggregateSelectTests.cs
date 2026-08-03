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
    public void SqlExpression_LeafTables_ReturnsParticipatingLeafTables()
    {
        Count count = new(new Column<Order>(nameof(Order.Total)));

        Assert.Empty(count.LeafTables);
        Assert.Equal("Order", Assert.Single(count.DescendantLeafTables).ToString());
    }

    [Fact]
    public void SelectTagGenerator_GetManyFromGroupBys_ReturnsSelectsForEachGroupBy()
    {
        GroupBys groupBys = GroupBys
            .New<Customer>(nameof(Customer.Name))
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
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));

        SqlQuery query = customerGenerator.InternalSelect(null, null, null, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT [Customer].[Name] FROM [Customer] GROUP BY [Customer].[Name]", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupedColumnAndAggregate_AllowsAggregateSelectList()
    {
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount")
        );

        SqlQuery query = customerGenerator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal("SELECT [Customer].[Name], COUNT([Customer].[Id]) AS [TotalCount] FROM [Customer] GROUP BY [Customer].[Name]", query.QueryText);
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
        GroupBys groupBys = GroupBys.New<Grades>(nameof(Grades.StudentId), nameof(Grades.AcademicYear), nameof(Grades.SemesterNumber));
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
}
