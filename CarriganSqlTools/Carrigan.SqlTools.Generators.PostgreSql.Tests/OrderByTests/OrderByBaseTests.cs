using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.OrderByClause;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.OrderByTests;

public class OrderByBaseTests
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();

    [Fact]
    public void ImplicitConversion_ReturnsOrderBysWithSingleItem()
    {
        OrderBy<Address> item = new("City");

        OrderBys orderBy = item;

        OrderBy actual = Assert.Single(orderBy.AsEnumerable());
        Assert.Same(item, actual);
        Assert.Equal("ORDER BY \"Address\".\"City\" ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void GetSqlFragmentParameters_ReturnsEmptyCollection() =>
        Assert.Empty(new OrderBy<Address>("Street").GetSqlFragmentParameters(Dialect));

    [Fact]
    public void ToSql_UsesColumnTagAndSortDirection() =>
        Assert.Equal("\"Address\".\"City\" DESC", new OrderBy<Address>("City", SortDirectionEnum.Descending).ToSql(Dialect));
}
