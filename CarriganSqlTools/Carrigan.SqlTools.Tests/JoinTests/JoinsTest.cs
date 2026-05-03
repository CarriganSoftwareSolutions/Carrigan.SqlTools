using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.JoinTests;
public class JoinsTest
{
    private readonly Predicates RightOnLeftPredicate;
    private readonly Predicates LastOnRightPredicate;
    public JoinsTest()
    {
        RightOnLeftPredicate = new ColumnEqualsColumn<JoinLeftTable, JoinRightTable>(nameof(JoinLeftTable.RightId), nameof(JoinRightTable.Id));
        LastOnRightPredicate = new ColumnEqualsColumn<JoinRightTable, JoinLastTable>(nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));

    }
    [Fact]
    public void NewJoinsNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new (new LeftJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }
    [Fact]
    public void NewJoinsNewJoin()
    {
        Joins<JoinLeftTable> relation = new(new Join<JoinRightTable>(RightOnLeftPredicate));
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }
    [Fact]
    public void NewJoinsNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }
    [Fact]
    public void NewJoinsNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new(new FullJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void JoinsLeftJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void JoinsRightJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.RightJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void JoinsJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate);
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void JoinsInnerJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.InnerJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void JoinsFullJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.FullJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewFullJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewFullJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewFullJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void NewJoinsNewFullJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments().ToSql(new SqlServerDialect()));
    }

    [Fact]
    public void InvalidTableException()
    {

        Predicates stupidPredicate = new ColumnEqualsColumn<ColumnIdentifiers, PhoneModel>(nameof(ColumnIdentifiers.Id), nameof(PhoneModel.Id));
        Assert.Throws<InvalidTableException>(() =>  new Joins<JoinLeftTable>
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate),
            new InnerJoin<ColumnIdentifiers>(stupidPredicate)
        ));
    }

    [Fact]
    public void TableTag()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        TableTag expected = new(new SqlServerDialect(), null, "Left");
        Assert.Equal(expected, relation.TableTag);
    }

    [Fact]
    public void Constructor_NullJoins_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new Joins<JoinLeftTable>(null!));

    [Fact]
    public void Constructor_NullJoinEntry_Exception()
    {
        JoinBase[] joins =
        [
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
        null!
        ];

        Assert.Throws<ArgumentNullException>(() => new Joins<JoinLeftTable>(joins));
    }

    [Fact]
    public void JoinsCrossJoin()
    {
        string actual = Joins<JoinLeftTable>.CrossJoin<JoinRightTable>().ToSqlFragments().ToSql(new SqlServerDialect());
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }
}
