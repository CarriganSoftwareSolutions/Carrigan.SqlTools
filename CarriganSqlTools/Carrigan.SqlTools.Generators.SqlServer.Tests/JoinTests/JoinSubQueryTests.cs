using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.JoinTests;

public class JoinSubQueryTests
{
    private static readonly SqlServerDialect Dialect = new();
    private static readonly SqlGenerator<JoinLeftTable> LeftGenerator = new();
    private static readonly SqlGenerator<JoinRightTable> RightGenerator = new();

    private static ColumnEqualsColumn<JoinLeftTable, JoinRightTable> RightOnLeftPredicate() =>
        new (nameof(JoinLeftTable.RightId), nameof(JoinRightTable.Id));

    private static ColumnEqualsColumn<JoinRightTable, JoinLastTable> LastOnRightPredicate() =>
        new (nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));

    private static SubQuery<JoinRightTable> RightSubQuery(Predicates? predicates = null, Joins<JoinRightTable>? joins = null) =>
        RightGenerator.SubQuery(null, null, joins, predicates, null, null);

    [Fact]
    public void Join_WithSubQuery_RendersSubQueryAndAlias()
    {
        Join<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubQuery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InnerJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        InnerJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubQuery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LeftJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        LeftJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubQuery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RightJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        RightJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubQuery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FullJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        FullJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubQuery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        CrossJoin<JoinRightTable> join = new(RightSubQuery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsInnerJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.InnerJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsLeftJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsRightJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.RightJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsFullJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.FullJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsCrossJoin_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.CrossJoin(RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinStaticFactory_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Join<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InnerJoinStaticFactory_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = InnerJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LeftJoinStaticFactory_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = LeftJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RightJoinStaticFactory_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = RightJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FullJoinStaticFactory_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = FullJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoinStaticFactory_WithSubQuery_RendersSubQueryAndAlias()
    {
        Joins<JoinLeftTable> joins = CrossJoin<JoinRightTable>.Joins<JoinLeftTable>(RightSubQuery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Select_WithSubQueryJoin_AllowsSelectingFromJoinedSubQueryAlias()
    {
        SelectTags selectTags = SelectTags
            .Get<JoinLeftTable>(nameof(JoinLeftTable.Id))
            .Append<JoinRightTable>(nameof(JoinRightTable.Col1), "RightCol1");

        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery());

        SqlQuery query = LeftGenerator.Select(null, selectTags, joins, null, null, null);

        Assert.Equal("SELECT [Left].[Id], [Right].[Col1] AS [RightCol1] FROM [Left] JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void Select_WithSubQueryJoin_CombinesSubQueryAndOuterPredicateParameters()
    {
        Predicates subQueryPredicate = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Col1)), new Parameter("RightCol1", "Open"));
        Predicates outerPredicate = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Col2)), new Parameter("RightCol2", "Closed"));
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery(subQueryPredicate));

        SqlQuery query = LeftGenerator.Select(null, null, joins, outerPredicate, null, null);

        Assert.Equal("SELECT [Left].* FROM [Left] JOIN (SELECT [Right].* FROM [Right] WHERE ([Right].[Col1] = @RightCol1_1)) AS [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Col2] = @RightCol2_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@RightCol1_1", "Open");
        SqlQueryTestHelper.AssertParameterValue(query, "@RightCol2_2", "Closed");
    }

    [Fact]
    public void Select_WithSubQueryJoin_AllowsSubQueryToHaveItsOwnJoins()
    {
        Joins<JoinRightTable> subQueryJoins = Joins<JoinRightTable>.Join<JoinLastTable>(LastOnRightPredicate());
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubQuery(null, subQueryJoins));

        SqlQuery query = LeftGenerator.Select(null, null, joins, null, null, null);

        Assert.Equal("SELECT [Left].* FROM [Left] JOIN (SELECT [Right].* FROM [Right] JOIN [Last] ON ([Right].[LastId] = [Last].[Id])) AS [Right] ON ([Left].[RightId] = [Right].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
