using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class CrossJoinTest
{
    [Fact]
    public void NewJoinsNewCrossJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        CrossJoin<JoinRightTable> join = new(id);

        string actual = (new Joins<JoinLeftTable>(join)).ToSql();
        string expected = "CROSS JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewCrossJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        CrossJoin<JoinRightTable> join = new(id);

        string actual = join.AsJoins<JoinLeftTable>().ToSql();
        string expected = "CROSS JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsCrossJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.CrossJoin<JoinRightTable>(id).ToSql();
        string expected = "CROSS JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        CrossJoin<JoinRightTable> join = new(id);

        TableTag expected = new(null, "Right");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void Constructor_Null_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new CrossJoin<JoinRightTable>(null!));

    [Fact]
    public void Joins_Null_Exception() => 
        Assert.Throws<ArgumentNullException>(() => CrossJoin<JoinRightTable>.Joins<JoinLeftTable>(null!));
}
