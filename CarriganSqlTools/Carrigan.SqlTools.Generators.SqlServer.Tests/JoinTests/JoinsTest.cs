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
        Joins relation = new (new LeftJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }
    [Fact]
    public void NewJoinsNewJoin()
    {
        Joins relation = new(new Join<JoinRightTable>(RightOnLeftPredicate));
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }
    [Fact]
    public void NewJoinsNewInnerJoin()
    {
        Joins relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }
    [Fact]
    public void NewJoinsNewFullJoin()
    {
        Joins relation = new(new FullJoin<JoinRightTable>(RightOnLeftPredicate));
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsLeftJoin()
    {
        Joins relation = LeftJoin<JoinRightTable>.Joins(RightOnLeftPredicate);
        string expected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsRightJoin()
    {
        Joins relation = RightJoin<JoinRightTable>.Joins(RightOnLeftPredicate);
        string expected = " RIGHT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsJoin()
    {
        Joins relation = Join<JoinRightTable>.Joins(RightOnLeftPredicate);
        string expected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsInnerJoin()
    {
        Joins relation = InnerJoin<JoinRightTable>.Joins(RightOnLeftPredicate);
        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void JoinsFullJoin()
    {
        Joins relation = FullJoin<JoinRightTable>.Joins(RightOnLeftPredicate);
        string expected = " FULL JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void NewJoinsNewLeftJoinNewLeftJoin()
    {
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins relation = new
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
        Joins joins = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate),
            new InnerJoin<ColumnIdentifiers>(stupidPredicate)
        );
        Assert.Throws<InvalidTableException>(() => joins.Validate(new TableTag(null, "Left")));
    }

    [Fact]
    public void TableTag()
    {
        Joins relation = new
        (
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
            new InnerJoin<JoinLastTable>(LastOnRightPredicate)
        );
        TableTag expected = new(null, "Left");
        Assert.Equal(2, relation.Validate(expected).Count());
    }

    [Fact]
    public void Constructor_NullJoins_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new Joins(null!));

    [Fact]
    public void Constructor_NullJoinEntry_Exception()
    {
        Join[] joins =
        [
            new InnerJoin<JoinRightTable>(RightOnLeftPredicate),
        null!
        ];

        Assert.Throws<ArgumentNullException>(() => new Joins(joins));
    }

    [Fact]
    public void JoinsCrossJoin()
    {
        string actual = CrossJoin<JoinRightTable>.Joins().ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Append_NewJoin_AddsJoinToEnd()
    {
        Joins relation = new Joins(new InnerJoin<JoinRightTable>(RightOnLeftPredicate))
            .Append(new LeftJoin<JoinLastTable>(LastOnRightPredicate));

        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) LEFT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Append_NewJoin_ReturnsNewJoinsAndDoesNotModifyOriginal()
    {
        Joins original = new(new Join<JoinRightTable>(RightOnLeftPredicate));
        Joins actual = original.Append(new FullJoin<JoinLastTable>(LastOnRightPredicate));

        string originalExpected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string actualExpected = " JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.NotSame(original, actual);
        Assert.Equal(originalExpected, original.ToSqlFragments(Dialect).ToSql(Dialect));
        Assert.Equal(actualExpected, actual.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Append_NullJoin_ThrowsArgumentNullException()
    {
        Joins relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<ArgumentNullException>(() => relation.Append(null!));
    }

    [Fact]
    public void Append_InvalidJoinOrder_ThrowsInvalidTableException()
    {
        Joins relation = new();

        Assert.Throws<InvalidTableException>(() => relation.Append(new LeftJoin<JoinLastTable>(LastOnRightPredicate)).Validate(new TableTag(null, "Left")));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_AddsJoinsToEnd()
    {
        Joins relation = new Joins(new InnerJoin<JoinRightTable>(RightOnLeftPredicate))
            .Concat([new RightJoin<JoinLastTable>(LastOnRightPredicate)]);

        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) RIGHT JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_ReturnsNewJoinsAndDoesNotModifyOriginal()
    {
        Joins original = new(new LeftJoin<JoinRightTable>(RightOnLeftPredicate));
        Joins actual = original.Concat([new Join<JoinLastTable>(LastOnRightPredicate)]);

        string originalExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string actualExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.NotSame(original, actual);
        Assert.Equal(originalExpected, original.ToSqlFragments(Dialect).ToSql(Dialect));
        Assert.Equal(actualExpected, actual.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_NullEnumerable_ThrowsArgumentNullException()
    {
        Joins relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<ArgumentNullException>(() => relation.Concat((IEnumerable<Join>)null!));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_NullJoinEntry_ThrowsArgumentNullException()
    {
        Joins relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<ArgumentNullException>(() => relation.Concat([null!]));
    }

    [Fact]
    public void Concat_JoinBaseEnumerable_InvalidJoinOrder_ThrowsInvalidTableException()
    {
        Joins relation = new();

        Assert.Throws<InvalidTableException>(() => relation.Concat([new LeftJoin<JoinLastTable>(LastOnRightPredicate)]).Validate(new TableTag(null, "Left")));
    }

    [Fact]
    public void Concat_Joins_AddsJoinsToEnd()
    {
        Joins existingJoins = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));
        Joins newJoins = new(new FullJoin<JoinLastTable>(LastOnRightPredicate));

        Joins relation = existingJoins.Concat(newJoins);

        string expected = " INNER JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) FULL JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.Equal(expected, relation.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_Joins_ReturnsNewJoinsAndDoesNotModifyOriginal()
    {
        Joins original = new(new LeftJoin<JoinRightTable>(RightOnLeftPredicate));
        Joins newJoins = new(new InnerJoin<JoinLastTable>(LastOnRightPredicate));

        Joins actual = original.Concat(newJoins);

        string originalExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id])";
        string actualExpected = " LEFT JOIN [Right] ON ([Left].[RightId] = [Right].[Id]) INNER JOIN [Last] ON ([Right].[LastId] = [Last].[Id])";

        Assert.NotSame(original, actual);
        Assert.Equal(originalExpected, original.ToSqlFragments(Dialect).ToSql(Dialect));
        Assert.Equal(actualExpected, actual.ToSqlFragments(Dialect).ToSql(Dialect));
    }

    [Fact]
    public void Concat_Joins_NullJoins_ThrowsArgumentNullException()
    {
        Joins relation = new(new InnerJoin<JoinRightTable>(RightOnLeftPredicate));

        Assert.Throws<NullReferenceException>(() => relation.Concat((Joins)null!));
    }
}
