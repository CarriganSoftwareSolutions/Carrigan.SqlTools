using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.JoinTests;
public class JoinsTest
{
    private static readonly SqlServerDialect Dialect = new();

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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }
    [Fact]
    public void NewJoinsNewJoin()
    {
        Joins<JoinLeftTable> relation = new(new Join<JoinRightTable>(RightOnLeftPredicate));
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }
    [Fact]
    public void NewJoinsNewInnerJoin()
    {
        Joins<JoinLeftTable> relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }
    [Fact]
    public void NewJoinsNewFullJoin()
    {
        Joins<JoinLeftTable> relation = new(new FullJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsLeftJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsRightJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.RightJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate);
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsInnerJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.InnerJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsFullJoin()
    {
        Joins<JoinLeftTable> relation = Joins<JoinLeftTable>.FullJoin<JoinRightTable>(RightOnLeftPredicate);
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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

        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
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
        Assert.Equal(expected, Joins<JoinLeftTable>.TableTag);
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
        string actual = Joins<JoinLeftTable>.CrossJoin<JoinRightTable>().ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Append_NewJoin_AddsJoinToEnd()
    {
        Joins<JoinLeftTable> relation = new Joins<JoinLeftTable>(new InnerJoin<JoinRightTable>(RightOnLeftPredicate))
            .Append(new LeftJoin<JoinLastTable>(LastOnRightPredicate));

        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Append_NewJoin_ReturnsNewJoinsAndDoesNotModifyOriginal()
    {
        Joins<JoinLeftTable> original = new(new Join<JoinRightTable>(RightOnLeftPredicate));
        Joins<JoinLeftTable> actual = original.Append(new FullJoin<JoinLastTable>(LastOnRightPredicate));

        string originalExpected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string actualExpected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.NotSame(original, actual);
        Assert.Equal(originalExpected, original.ToSqlFragments(Dialect).ToSql(Dialect));
        Assert.Equal(actualExpected, actual.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Append_NullJoin_ThrowsArgumentNullException()
    {
        Joins<JoinLeftTable> relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<ArgumentNullException>(() => relation.Append(null!));
    }

    [Fact]
    public void Append_InvalidJoinOrder_ThrowsInvalidTableException()
    {
        Joins<JoinLeftTable> relation = new();

        Assert.Throws<InvalidTableException>(() => relation.Append(new LeftJoin<JoinLastTable>(LastOnRightPredicate)));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_AddsJoinsToEnd()
    {
        Joins<JoinLeftTable> relation = new Joins<JoinLeftTable>(new InnerJoin<JoinRightTable>(RightOnLeftPredicate))
            .Concat([new RightJoin<JoinLastTable>(LastOnRightPredicate)]);

        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) RIGHT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_ReturnsNewJoinsAndDoesNotModifyOriginal()
    {
        Joins<JoinLeftTable> original = new(new LeftJoin<JoinRightTable>(RightOnLeftPredicate));
        Joins<JoinLeftTable> actual = original.Concat([new Join<JoinLastTable>(LastOnRightPredicate)]);

        string originalExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string actualExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.NotSame(original, actual);
        Assert.Equal(originalExpected, original.ToSqlFragments(Dialect).ToSql(Dialect));
        Assert.Equal(actualExpected, actual.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_NullEnumerable_ThrowsArgumentNullException()
    {
        Joins<JoinLeftTable> relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<ArgumentNullException>(() => relation.Concat((IEnumerable<JoinBase>)null!));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_NullJoinEntry_ThrowsArgumentNullException()
    {
        Joins<JoinLeftTable> relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<ArgumentNullException>(() => relation.Concat([null!]));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_InvalidJoinOrder_ThrowsInvalidTableException()
    {
        Joins<JoinLeftTable> relation = new();

        Assert.Throws<InvalidTableException>(() => relation.Concat([new LeftJoin<JoinLastTable>(LastOnRightPredicate)]));
    }

    [Fact]
    public void Concat_Joins_AddsJoinsToEnd()
    {
        Joins<JoinLeftTable> existingJoins = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));
        Joins<JoinRightTable> newJoins = new(new FullJoin<JoinLastTable>(LastOnRightPredicate));

        Joins<JoinLeftTable> relation = existingJoins.Concat(newJoins);

        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_Joins_ReturnsNewJoinsAndDoesNotModifyOriginal()
    {
        Joins<JoinLeftTable> original = new(new LeftJoin<JoinRightTable>(RightOnLeftPredicate));
        Joins<JoinRightTable> newJoins = new(new InnerJoin<JoinLastTable>(LastOnRightPredicate));

        Joins<JoinLeftTable> actual = original.Concat(newJoins);

        string originalExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string actualExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.NotSame(original, actual);
        Assert.Equal(originalExpected, original.ToSqlFragments(Dialect).ToSql(Dialect));
        Assert.Equal(actualExpected, actual.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_Joins_NullJoins_ThrowsArgumentNullException()
    {
        Joins<JoinLeftTable> relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<NullReferenceException>(() => relation.Concat<JoinRightTable>(null!));
    }
}
