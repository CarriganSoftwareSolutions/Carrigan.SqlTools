using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.JoinTests;

public class CrossJoinTest
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void NewJoinsNewCrossJoin()
    {
        CrossJoin<JoinRightTable> join = new();

        string actual = (new Joins<JoinLeftTable>(join)).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewCrossJoinAsJoins()
    {
        CrossJoin<JoinRightTable> join = new();

        string actual = join.AsJoins<JoinLeftTable>().ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoin_JoinsOn_Empty()
    {
        CrossJoin<JoinRightTable> join = new();

        Assert.Empty(join.JoinsOn);
    }

    [Fact]
    public void TableTag()
    {
        CrossJoin<JoinRightTable> join = new();
        TableTag expected = new(null, "Right");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void CrossJoin_JoinsFactoryMethod()
    {
        string actual = CrossJoin<JoinRightTable>.Joins<JoinLeftTable>().ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoin_NotEmpty()
    {
        CrossJoin<Customer> join = new();
        Joins<Order>? joins = new (join);
        Assert.False(joins.IsNullOrEmpty());
    }
}
