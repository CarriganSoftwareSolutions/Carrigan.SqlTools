using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public class SqlGenerator_AggregateSelectTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

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
        SqlGenerator<Customer> generator = new();
        SelectTags selects = new(new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount"));

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal("SELECT COUNT([Customer].[Id]) AS [TotalCount] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void Select_WithCountStar_AllowsAggregateSelectListWithoutSelectedTableTag()
    {
        SqlGenerator<Customer> generator = new();
        SelectTags selects = new(new SelectTag(new Count(), "TotalCount"));

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(*) AS [TotalCount] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void AggregateExpressions_AreAggregate()
    {
        Assert.True(new Count().IsAggregate(null));
        Assert.True(new Count(new Column<Customer>(nameof(Customer.Id))).IsAggregate(null));
        Assert.True(new Sum(new Column<Order>(nameof(Order.Total))).IsAggregate(null));
        Assert.True(new Avg(new Column<Order>(nameof(Order.Total))).IsAggregate(null));
        Assert.True(new Average(new Column<Order>(nameof(Order.Total))).IsAggregate(null));
        Assert.True(new Min(new Column<Order>(nameof(Order.Total))).IsAggregate(null));
        Assert.True(new Max(new Column<Order>(nameof(Order.Total))).IsAggregate(null));
    }

    [Fact]
    public void Column_IsAggregateOnlyWhenContainedInGroupBy()
    {
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));

        Assert.True(new Column<Customer>(nameof(Customer.Name)).IsAggregate(groupBys));
        Assert.False(new Column<Customer>(nameof(Customer.Email)).IsAggregate(groupBys));
        Assert.False(new Column<Customer>(nameof(Customer.Name)).IsAggregate(null));
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
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));

        SelectTag groupedColumn = SelectTagGenerator.Get<Customer>(nameof(Customer.Name));
        SelectTag aggregate = new(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount");
        SelectTag ungroupedColumn = SelectTagGenerator.Get<Customer>(nameof(Customer.Email));

        Assert.True(groupedColumn.IsAggregate(groupBys));
        Assert.True(aggregate.IsAggregate(groupBys));
        Assert.False(ungroupedColumn.IsAggregate(groupBys));
    }

    [Fact]
    public void Select_WithGroupBysAndNoSelects_UsesGroupByColumnsAsSelects()
    {
        SqlGenerator<Customer> generator = new();
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));

        SqlQuery query = generator.Select(null, null, null, null, null, groupBys, null, null);

        Assert.Equal("SELECT [Customer].[Name] FROM [Customer] GROUP BY [Customer].[Name]", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupedColumnAndAggregate_AllowsAggregateSelectList()
    {
        SqlGenerator<Customer> generator = new();
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount")
        );

        SqlQuery query = generator.Select(null, null, selects, null, null, groupBys, null, null);

        Assert.Equal("SELECT [Customer].[Name], COUNT([Customer].[Id]) AS [TotalCount] FROM [Customer] GROUP BY [Customer].[Name]", query.QueryText);
    }

    [Fact]
    public void Select_WithMixedAggregateAndNonAggregateSelects_Throws()
    {
        SqlGenerator<Customer> generator = new();
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount")
        );

        Assert.Throws<MixedAggregateSelectException>(() => generator.Select(null, null, selects, null, null, null, null, null));
    }
}
