using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Base.Tests.TestEntities;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.OrderByTests;

public class OrderByBaseTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

    [Fact]
    public void ImplicitConversion_ReturnsOrderBysWithSingleItem()
    {
        OrderByBase item = new OrderBy<Address>("City");

        OrderBys orderBy = item;

        OrderByBase actual = Assert.Single(orderBy.AsEnumerable());
        Assert.Same(item, actual);
        Assert.Equal("ORDER BY [Address].[City] ASC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void Flatten_ReturnsCurrentItemOnly()
    {
        OrderByBase item = new OrderBy<Address>("Street");

        IEnumerable<ISqlFragment> fragments = item.Flatten();

        ISqlFragment fragment = Assert.Single(fragments);
        Assert.Same(item, fragment);
    }

    [Fact]
    public void GetSqlFragmentParameters_ReturnsEmptyCollection() =>
        Assert.Empty(new OrderBy<Address>("Street").GetSqlFragmentParameters());

    [Fact]
    public void ToSql_UsesColumnTagAndSortDirection() =>
        Assert.Equal("[Address].[City] DESC", new OrderBy<Address>("City", SortDirectionEnum.Descending).ToSql(Dialect));
}
