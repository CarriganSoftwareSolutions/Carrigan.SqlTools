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

public class JoinSubqueryTests
{
    private static readonly SqlServerDialect Dialect = new();
    private static readonly SqlGenerator<JoinLeftTable> LeftGenerator = new();
    private static readonly SqlGenerator<JoinRightTable> RightGenerator = new();

    private static ColumnEqualsColumn<JoinLeftTable, JoinRightTable> RightOnLeftPredicate() =>
        new (nameof(JoinLeftTable.RightId), nameof(JoinRightTable.Id));

    private static ColumnEqualsColumn<JoinRightTable, JoinLastTable> LastOnRightPredicate() =>
        new (nameof(JoinRightTable.LastId), nameof(JoinLastTable.Id));

    private static Subquery<JoinRightTable> RightSubquery(Predicates? predicates = null, Joins<JoinRightTable>? joins = null) =>
        RightGenerator.Subquery(null, null, joins, predicates, null, null);

    [Fact]
    public void Join_WithSubquery_RendersSubqueryAndAlias()
    {
        Join<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubquery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InnerJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        InnerJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubquery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LeftJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        LeftJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubquery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RightJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        RightJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubquery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FullJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        FullJoin<JoinRightTable> join = new(RightOnLeftPredicate(), RightSubquery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        CrossJoin<JoinRightTable> join = new(RightSubquery());

        string actual = new Joins<JoinLeftTable>(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsInnerJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.InnerJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsLeftJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.LeftJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsRightJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.RightJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsFullJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.FullJoin<JoinRightTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsCrossJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.CrossJoin(RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = Join<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InnerJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = InnerJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LeftJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = LeftJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RightJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = RightJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FullJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = FullJoin<JoinRightTable>.Joins<JoinLeftTable>(RightOnLeftPredicate(), RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins<JoinLeftTable> joins = CrossJoin<JoinRightTable>.Joins<JoinLeftTable>(RightSubquery());

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Select_WithSubqueryJoin_AllowsSelectingFromJoinedSubqueryAlias()
    {
        SelectTags selectTags = SelectTags
            .Get<JoinLeftTable>(nameof(JoinLeftTable.Id))
            .Append<JoinRightTable>(nameof(JoinRightTable.Col1), "RightCol1");

        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubquery());

        SqlQuery query = LeftGenerator.Select(null, null, selectTags, joins, null, null, null);

        Assert.Equal("SELECT [Left].[Id], [Right].[Col1] AS [RightCol1] FROM [Left] JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void Select_WithSubqueryJoin_CombinesSubqueryAndOuterPredicateParameters()
    {
        Predicates subQueryPredicate = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Col1)), new Parameter("Open", "RightCol1"));
        Predicates outerPredicate = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Col2)), new Parameter("Closed", "RightCol2"));
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubquery(subQueryPredicate));

        SqlQuery query = LeftGenerator.Select(null, null, null, joins, outerPredicate, null, null);

        Assert.Equal("SELECT [Left].* FROM [Left] JOIN (SELECT [Right].* FROM [Right] WHERE ([Right].[Col1] = @RightCol1_1)) AS [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Col2] = @RightCol2_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@RightCol1_1", "Open");
        SqlQueryTestHelper.AssertParameterValue(query, "@RightCol2_2", "Closed");
    }

    [Fact]
    public void Select_WithSubqueryJoin_AllowsSubqueryToHaveItsOwnJoins()
    {
        Joins<JoinRightTable> subQueryJoins = Joins<JoinRightTable>.Join<JoinLastTable>(LastOnRightPredicate());
        Joins<JoinLeftTable> joins = Joins<JoinLeftTable>.Join<JoinRightTable>(RightOnLeftPredicate(), RightSubquery(null, subQueryJoins));

        SqlQuery query = LeftGenerator.Select(null, null, null, joins, null, null, null);

        Assert.Equal("SELECT [Left].* FROM [Left] JOIN (SELECT [Right].* FROM [Right] JOIN [Last] ON ([Right].[LastId] = [Last].[Id])) AS [Right] ON ([Left].[RightId] = [Right].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
