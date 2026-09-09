using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.GroupByTests;

public class GroupByBaseTests
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();

    [Fact]
    public void ImplicitConversion_ReturnsGroupBysWithSingleItem()
    {
        GroupBy<Address> item = new("City");

        GroupBys groupBy = item;

        GroupByBase actual = Assert.Single(groupBy.AsEnumerable());
        Assert.Same(item, actual);
        Assert.Equal("GROUP BY \"Address\".\"City\"", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Flatten_ReturnsCurrentItemOnly()
    {
        GroupBy<Address> item = new("Street");

        IEnumerable<ISqlFragment> fragments = item.Flatten(Dialect);

        ISqlFragment fragment = Assert.Single(fragments);
        Assert.Same(item, fragment);
    }

    [Fact]
    public void GetSqlFragmentParameters_ReturnsEmptyCollection() =>
        Assert.Empty(new GroupBy<Address>("Street").GetSqlFragmentParameters(Dialect));

    [Fact]
    public void ToString_ReturnsQualifiedColumnName() =>
        Assert.Equal("Address.City", new GroupBy<Address>("City").ToString());

    [Fact]
    public void ToSql_UsesColumnTag() =>
        Assert.Equal("\"Address\".\"City\"", new GroupBy<Address>("City").ToSql(Dialect));

    [Fact]
    public void EmptyAndWhiteSpaceContracts_DelegateToColumnTag()
    {
        GroupByBase item = new GroupBy<Address>("Street");

        Assert.False(item.IsEmpty());
        Assert.True(item.IsNotEmpty());
        Assert.False(item.IsWhiteSpace());
        Assert.True(item.IsNotWhiteSpace());
    }
}
