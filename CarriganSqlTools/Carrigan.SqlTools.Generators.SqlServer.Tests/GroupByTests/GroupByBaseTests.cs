using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GroupByTests;

public class GroupByBaseTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

    [Fact]
    public void ImplicitConversion_ReturnsGroupBysWithSingleItem()
    {
        GroupBy<Address> item = new("City");

        GroupBys groupBy = item;

        GroupBy actual = Assert.Single(groupBy.AsEnumerable());
        Assert.Same(item, actual);
        Assert.Equal("GROUP BY [Address].[City]", groupBy.ToSql(Dialect));
    }

    [Fact]
    public void Flatten_ReturnsExpressionLeafFragment()
    {
        GroupBy<Address> item = new("Street");

        IEnumerable<ISqlFragment> fragments = item.Flatten(Dialect);

        ISqlFragment fragment = Assert.Single(fragments);
        Assert.IsType<ColumnTag>(fragment);
        Assert.NotSame(item, fragment);
        Assert.Equal(item.ToSql(Dialect), fragment.ToSql(Dialect));
    }

    [Fact]
    public void Flatten_WithCompositeExpression_ReturnsAllExpressionLeafFragments()
    {
        GroupBy item = new
        (
            new Add
            (
                new Column<Address>(nameof(Address.PostalCode)),
                new Parameter(1, "Offset")
            )
        );

        ISqlFragment[] fragments = [.. item.Flatten(Dialect)];

        Assert.Equal(5, fragments.Length);
        Assert.IsType<SqlFragmentText>(fragments[0]);
        Assert.IsType<ColumnTag>(fragments[1]);
        Assert.IsType<SqlFragmentText>(fragments[2]);
        Assert.IsType<SqlFragmentParameter>(fragments[3]);
        Assert.IsType<SqlFragmentText>(fragments[4]);
    }

    [Fact]
    public void GetSqlFragmentParameters_WithExpressionParameter_ReturnsParameter()
    {
        GroupBy item = new(new Add(new Column<Address>(nameof(Address.PostalCode)), new Parameter(1, "Offset")));

        SqlFragmentParameter parameter = Assert.Single(item.GetSqlFragmentParameters(Dialect));

        Assert.Equal("Offset", parameter.ParameterTag.ToString());
    }

    [Fact]
    public void GetSqlFragmentParameters_ReturnsEmptyCollection() =>
        Assert.Empty(new GroupBy<Address>("Street").GetSqlFragmentParameters(Dialect));

    [Fact]
    public void ToString_ReturnsQualifiedColumnName() =>
        Assert.Equal("Address.City", new GroupBy<Address>("City").ToString());

    [Fact]
    public void ToSql_UsesColumnTag() =>
        Assert.Equal("[Address].[City]", new GroupBy<Address>("City").ToSql(Dialect));

}
