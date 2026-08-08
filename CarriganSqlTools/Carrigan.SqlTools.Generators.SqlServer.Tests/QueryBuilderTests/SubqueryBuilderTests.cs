using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;
using System.Data;
using Carrigan.SqlTools.QueryBuilders;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.QueryBuilderTests;

public class SubqueryBuilderTests
{
    private static readonly SqlServerDialect Dialect = new();
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SubqueryBuilder_WithDistinctSelectsWhereOrderByAndPaging_RendersExpectedSql()
    {
        SubqueryBuilder<Customer> subqueryBuilder = new()
        {
            Distinct = true,
            Selects = SelectTagGenerator.GetMany<Customer>(nameof(Customer.Id), nameof(Customer.Name)),
            Where = new Equal(new Column<Customer>(nameof(Customer.Name)), new Parameter("Hank", "Name")),
            OrderBys = new OrderBys(new OrderBy<Customer>(nameof(Customer.Name))),
            Paging = new DefinePage(2, 25)
        };

        Subquery<Customer> subquery = customerGenerator.Subquery(subqueryBuilder);

        SqlQuery query = new(Dialect, CommandType.Text, subquery.Flatten());

        Assert.Equal("(SELECT DISTINCT [Customer].[Id], [Customer].[Name] FROM [Customer] WHERE ([Customer].[Name] = @Name_1) ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }

    [Fact]
    public void SubqueryBuilder_WithGroupByAndHaving_RendersExpectedSql()
    {
        SqlGenerator<Grades> gradesGenerator = new();
        Average averageGradePoint = new(new Column<Grades>(nameof(Grades.GradePoint)));
        SubqueryBuilder<Grades> subqueryBuilder = new()
        {
            Selects = new SelectTags
            (
                SelectTagGenerator.Get<Grades>(nameof(Grades.StudentId)),
                new SelectTag(averageGradePoint, "AverageGradePoint")
            ),
            GroupBys = GroupBys.New<Grades>(nameof(Grades.StudentId)),
            Having = new GreaterThan(averageGradePoint, new Parameter(3.5m, "MinimumGpa"))
        };

        Subquery<Grades> subquery = gradesGenerator.Subquery(subqueryBuilder);
        SqlQuery query = new(Dialect, CommandType.Text, subquery.Flatten());

        Assert.Equal("(SELECT [Grades].[StudentId], AVG([Grades].[GradePoint]) AS [AverageGradePoint] FROM [Grades] GROUP BY [Grades].[StudentId] HAVING (AVG([Grades].[GradePoint]) > @MinimumGpa_1))", query.QueryText);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@MinimumGpa_1", 3.5m);
    }

    [Fact]
    public void SubqueryBuilder_WithGroupByAndHavingFluentMethods_ReturnsUpdatedCopy()
    {
        SubqueryBuilder<Grades> original = new();
        GroupBys groupBys = GroupBys.New<Grades>(nameof(Grades.StudentId));
        Average averageGradePoint = new(new Column<Grades>(nameof(Grades.GradePoint)));
        Predicates having = new GreaterThan(averageGradePoint, new Parameter(3.5m, "MinimumGpa"));

        SubqueryBuilderBase<Grades> updated = original.WithGroupBy(groupBys).WithHaving(having);

        Assert.Null(original.GroupBys);
        Assert.Null(original.Having);
        Assert.Same(groupBys, updated.GroupBys);
        Assert.Same(having, updated.Having);
    }

}
