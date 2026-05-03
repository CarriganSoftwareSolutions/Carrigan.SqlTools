using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.JoinTests;

public class JoinTests
{
    [Fact]
    public void NewJoinsNewJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Join<JoinRightTable> join = new(id);

        string actual = (new Joins<JoinLeftTable>(join)).ToSqlFragments().ToSql(new SqlServerDialect());
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Join<JoinRightTable> join = new(id);

        string actual = join.AsJoins<JoinLeftTable>().ToSqlFragments().ToSql(new SqlServerDialect());
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.Join<JoinRightTable>(id).ToSqlFragments().ToSql(new SqlServerDialect());
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        Join<JoinLeftTable> join = new(id);

        TableTag expected = new(new SqlServerDialect(), null, "Left");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void Constructor_Null_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new Join<JoinRightTable>(null!));

    [Fact]
    public void Joins_Null_Exception() => 
        Assert.Throws<ArgumentNullException>(() => Join<JoinRightTable>.Joins<JoinLeftTable>(null!));

    [Fact]
    public void JoinsNewJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Join<JoinRightTable>.Joins<JoinLeftTable>(id).ToSqlFragments().ToSql(new SqlServerDialect());
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToSql_EmptyPredicate_Exception()
    {
        Predicates id = new EmptyPredicate();
        Join<JoinRightTable> join = new(id);

        Assert.Throws<InvalidOperationException>(() => join.ToSqlFragments("Join").ToSql(new SqlServerDialect()));
    }
}
