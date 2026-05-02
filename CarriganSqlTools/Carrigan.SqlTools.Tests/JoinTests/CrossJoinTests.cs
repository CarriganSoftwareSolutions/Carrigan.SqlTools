using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class CrossJoinTest
{
    [Fact]
    public void NewJoinsNewCrossJoin()
    {
        CrossJoin<JoinRightTable> join = new();

        string actual = (new Joins<JoinLeftTable>(join)).ToSqlFragments().ToSql(new SqlServerDialect());
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewCrossJoinAsJoins()
    {
        CrossJoin<JoinRightTable> join = new();

        string actual = join.AsJoins<JoinLeftTable>().ToSqlFragments().ToSql(new SqlServerDialect());
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
        TableTag expected = new(new SqlServerDialect(), null, "Right");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void CrossJoin_JoinsFactoryMethod()
    {
        string actual = CrossJoin<JoinRightTable>.Joins<JoinLeftTable>().ToSqlFragments().ToSql(new SqlServerDialect());
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoin_NotEmpty()
    {
        CrossJoin<Customer> join = new();
        JoinsBase? joins = new Joins<Order>(join);
        Assert.False(joins.IsNullOrEmpty());
    }
}
