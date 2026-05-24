using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Base.Tests.TestEntities;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.JoinTests;

public class RightJoinTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void NewJoinsNewRightJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        RightJoin<JoinRightTable> rightJoin = new(id);

        string actual = (new Joins<JoinLeftTable>(rightJoin)).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewRightJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        RightJoin<JoinRightTable> rightJoin = new(id);

        string actual = rightJoin.AsJoins<JoinLeftTable>().ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsRightJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.RightJoin<JoinRightTable>(id).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        RightJoin<JoinLeftTable> join = new(id);

        TableTag expected = new(null, "Left");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void JoinsNewRightJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = RightJoin<JoinRightTable>.Joins<JoinLeftTable>(id).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToSql_EmptyPredicate_Exception()
    {
        Predicates id = new EmptyPredicate();
        RightJoin<JoinRightTable> join = new(id);

        Assert.Throws<InvalidOperationException>(() => join.ToSqlFragments(Dialect, "Join").ToSql(Dialect));
    }

}
