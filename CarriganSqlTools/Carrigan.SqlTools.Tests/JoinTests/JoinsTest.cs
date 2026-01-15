using Carrigan.SqlTools.Exceptions;
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
        string expected = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }
    [Fact]
    public void NewJoinsNewJoin()
    {
        Joins<JoinLeftTable> relation = new(new Join<JoinRightTable>(RightOnLeftPredicate));
        string expected = "JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }
    [Fact]
    public void NewJoinsNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }
    [Fact]
    public void NewJoinsNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new(new FullJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = "FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void JoinsLeftJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void JoinsRightJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.RightJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = "RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void JoinsJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate);
        string expected = "JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void JoinsInnerJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.InnerJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void JoinsFullJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.FullJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = "FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new LeftJoin<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new Join<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewInnerJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewFullJoinNewLeftJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new LeftJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewFullJoinNewJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new Join<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewFullJoinNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
    }

    [Fact]
    public void NewJoinsNewFullJoinNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new
        (
            new FullJoin<JoinRightTable>(RightOnLeftPredicate),
            new FullJoin<JoinLastTable>(LastOnRightPredicate)
        );
        string expected1 = "FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string expected2 = "FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";
        string expected = $"{expected1} {expected2}";
        Assert.Equal(expected, relation.ToSql());
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
        TableTag expected = new(null, "Left");
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
        string actual = Joins<JoinLeftTable>.CrossJoin<JoinRightTable>().ToSql();
        string expected = "CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }
}
