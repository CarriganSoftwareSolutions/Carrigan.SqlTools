using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class InnerJoinTest
{
    [Fact]
    public void NewJoinsNewInnerJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        InnerJoin<JoinRightTable> join = new(id);

        string actual = (new Joins<JoinLeftTable>(join)).ToSql();
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewInnerJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        InnerJoin<JoinRightTable> join = new(id);

        string actual = join.AsJoins<JoinLeftTable>().ToSql();
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsInnerJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.InnerJoin<JoinRightTable>(id).ToSql();
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        InnerJoin<JoinRightTable> join = new(id);

        TableTag expected = new(null, "Right");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void Constructor_Null_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => new InnerJoin<JoinRightTable>(null!));
    }

    [Fact]
    public void Joins_Null_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => InnerJoin<JoinRightTable>.Joins<JoinLeftTable>(null!));
    }
}
