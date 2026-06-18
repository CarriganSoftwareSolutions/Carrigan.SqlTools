using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.JoinTests;

public class FullJoinTest
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void NewJoinsNewFullJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        FullJoin<JoinRightTable> join = new(id);

        string actual = (new Joins<JoinLeftTable>(join)).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewFullJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        FullJoin<JoinRightTable> join = new(id);

        string actual = join.AsJoins<JoinLeftTable>().ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsFullJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.FullJoin<JoinRightTable>(id).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        FullJoin<JoinRightTable> join = new(id);

        TableTag expected = new(null, "Right");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void Constructor_Null_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new FullJoin<JoinRightTable>(null!));

    [Fact]
    public void Joins_Null_Exception() =>
        Assert.Throws<ArgumentNullException>(() => FullJoin<JoinRightTable>.Joins<JoinLeftTable>(null!));

    [Fact]
    public void ToSql_EmptyPredicate_Exception()
    {
        Predicates predicate = new EmptyPredicate();
        FullJoin<JoinRightTable> join = new(predicate);

        Assert.Throws<InvalidOperationException>(() => join.ToSqlFragments(Dialect).ToSql(Dialect));
    }
}
