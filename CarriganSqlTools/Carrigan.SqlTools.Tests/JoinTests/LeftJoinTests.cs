using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class LeftJoinsTest
{
    [Fact]
    public void NewJoinsNewLeftJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> leftJoin = new(id);

        string actual = (new Joins<JoinLeftTable>(leftJoin)).ToSql();
        string expected = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewLeftJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> leftJoin = new(id);

        string actual = leftJoin.AsJoins<JoinLeftTable>().ToSql();
        string expected = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsLeftJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(id).ToSql();
        string expected = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinLeftTable> join = new(id);
        TableTag expected = new(null, "Left");
        Assert.Equal(expected, join.TableTag);
    }
}
