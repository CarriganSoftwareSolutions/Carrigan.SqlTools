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

        string actual = (new Joins<JoinLeftTable>(join)).ToSql();
        string expected = "CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewCrossJoinAsJoins()
    {
        CrossJoin<JoinRightTable> join = new();

        string actual = join.AsJoins<JoinLeftTable>().ToSql();
        string expected = "CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoin_JoinsOn_Empty()
    {
        CrossJoin<JoinRightTable> join = new();

        Assert.Empty(join.JoinsOn);
    }

    [Fact]
    public void CrossJoin_Parameters_Empty()
    {
        CrossJoin<JoinRightTable> join = new();

        Assert.Empty(join.GetParameters(string.Empty));
    }

    [Fact]
    public void TableTag()
    {
        CrossJoin<JoinRightTable> join = new();
        TableTag expected = new(null, "Right");

        Assert.Equal(expected, join.TableTag);
    }
}
