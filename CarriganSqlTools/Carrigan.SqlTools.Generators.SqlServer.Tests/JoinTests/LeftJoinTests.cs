using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.JoinTests;

public class LeftJoinTest
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void NewJoinsNewLeftJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> join = new(id);

        string actual = (new Joins<JoinLeftTable>(join)).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NewLeftJoinAsJoins()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> join = new(id);

        string actual = join.AsJoins<JoinLeftTable>().ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsLeftJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(id).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TableTag()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));
        LeftJoin<JoinRightTable> join = new(id);

        TableTag expected = new(null, "Right");

        Assert.Equal(expected, join.TableTag);
    }

    [Fact]
    public void Constructor_Null_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new LeftJoin<JoinRightTable>(null!));

    [Fact]
    public void Joins_Null_Exception() => 
        Assert.Throws<ArgumentNullException>(() => LeftJoin<JoinRightTable>.Joins<JoinLeftTable>(null!));

    [Fact]
    public void JoinsNewLeftJoin()
    {
        Predicates id = new Equal(new Column<JoinLeftTable>("RightId"), new Column<JoinRightTable>("Id"));

        string actual = LeftJoin<JoinRightTable>.Joins<JoinLeftTable>(id).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToSql_EmptyPredicate_Exception()
    {
        Predicates id = new EmptyPredicate();
        LeftJoin<JoinRightTable> join = new(id);

        Assert.Throws<InvalidOperationException>(() => join.ToSqlFragments(Dialect).ToSql(Dialect));
    }
}
