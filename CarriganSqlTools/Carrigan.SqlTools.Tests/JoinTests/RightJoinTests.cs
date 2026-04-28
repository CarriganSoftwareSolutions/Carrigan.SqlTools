using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class RightJoinTests
{
    [Fact]
    public void NewJoinsNewRightJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        RightJoin<JoinRightTable> rightJoin = new(id);

        string actual = (new Joins<JoinLeftTable>(rightJoin)).ToSql();
        string expected = "RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewRightJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        RightJoin<JoinRightTable> rightJoin = new(id);

        string actual = rightJoin.AsJoins<JoinLeftTable>().ToSql();
        string expected = "RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsRightJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.RightJoin<JoinRightTable>(id).ToSql();
        string expected = "RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        RightJoin<JoinLeftTable> join = new(id);

        TableTag expected = new(new SqlServerDialect(), null, "Left");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void JoinsNewRightJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = RightJoin<JoinRightTable>.Joins<JoinLeftTable>(id).ToSql();
        string expected = "RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToSql_EmptyPredicate_Exception()
    {
        Predicates id = new EmptyPredicate();
        RightJoin<JoinRightTable> join = new(id);

        Assert.Throws<InvalidOperationException>(() => join.ToSql("Join"));
    }

}
