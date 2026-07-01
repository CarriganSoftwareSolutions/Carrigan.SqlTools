using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.GeneratorTests;

public sealed class SqlGenerator_AggregateSelectTests
{
    [Fact]
    public void Select_WithAggregateOnly_AllowsAggregateSelectList()
    {
        SqlGenerator<Customer> generator = new();
        SelectTags selects = new(new SelectTag(new Count(new Column<Customer>(nameof(Customer.Id))), "TotalCount"));

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(\"Customer\".\"Id\") AS \"TotalCount\" FROM \"Customer\"", query.QueryText);
    }

    [Fact]
    public void Select_WithCountStar_AllowsAggregateSelectListWithoutSelectedTableTag()
    {
        SqlGenerator<Customer> generator = new();
        SelectTags selects = new(new SelectTag(new Count(), "TotalCount"));

        SqlQuery query = generator.Select(null, null, selects, null, null, null, null, null);

        Assert.Equal("SELECT COUNT(*) AS \"TotalCount\" FROM \"Customer\"", query.QueryText);
    }

    [Fact]
    public void Select_WithGroupBysAndNoSelects_UsesGroupByColumnsAsSelects()
    {
        SqlGenerator<Customer> generator = new();
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));

        SqlQuery query = generator.Select(null, null, null, null, null, groupBys, null, null);

        Assert.Equal("SELECT \"Customer\".\"Name\" FROM \"Customer\" GROUP BY \"Customer\".\"Name\"", query.QueryText);
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

        Assert.Equal("SELECT \"Customer\".\"Name\", COUNT(\"Customer\".\"Id\") AS \"TotalCount\" FROM \"Customer\" GROUP BY \"Customer\".\"Name\"", query.QueryText);
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
