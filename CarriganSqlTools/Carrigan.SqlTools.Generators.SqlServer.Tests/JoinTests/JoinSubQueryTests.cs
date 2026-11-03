using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
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

    private static Subquery RightSubquery(Predicates? predicates = null, Joins? joins = null) =>
        RightGenerator.InternalSubquery(null, null, joins, predicates, null, null, null, null);

    [Fact]
    public void Join_WithSubquery_RendersSubqueryAndAlias()
    {
        Join join = new(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = new Joins(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InnerJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        InnerJoin join = new(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = new Joins(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LeftJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        LeftJoin join = new(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = new Joins(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RightJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        RightJoin join = new(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = new Joins(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FullJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        FullJoin join = new(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = new Joins(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        CrossJoin join = new(RightSubquery(), "Right");

        string actual = new Joins(join).ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = Join.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsInnerJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = InnerJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsLeftJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = LeftJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsRightJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = RightJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsFullJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = FullJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinsCrossJoin_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = Joins.CrossJoin(RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = Join.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InnerJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = InnerJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " INNER JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LeftJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = LeftJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " LEFT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RightJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = RightJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " RIGHT JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FullJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = FullJoin.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " FULL JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CrossJoinStaticFactory_WithSubquery_RendersSubqueryAndAlias()
    {
        Joins joins = CrossJoin.Joins(RightSubquery(), "Right");

        string actual = joins.ToSqlFragments(Dialect).ToSql(Dialect);
        string expected = " CROSS JOIN (SELECT [Right].* FROM [Right]) AS [Right]";

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Select_WithSubqueryJoin_AllowsSelectingFromJoinedSubqueryAlias()
    {
        SelectTags selectTags =
            SelectTagGenerator
                .Get<JoinLeftTable>(nameof(JoinLeftTable.Id))
                .Append<JoinRightTable>(nameof(JoinRightTable.Col1), "RightCol1");

        Joins joins = Join.Joins(RightOnLeftPredicate(), RightSubquery(), "Right");

        SqlQuery query = LeftGenerator.InternalSelect(null, null, selectTags, joins, null, null, null, null, null);

        Assert.Equal("SELECT [Left].[Id], [Right].[Col1] AS [RightCol1] FROM [Left] JOIN (SELECT [Right].* FROM [Right]) AS [Right] ON ([Left].[RightId] = [Right].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void Select_WithSubqueryJoin_CombinesSubqueryAndOuterPredicateParameters()
    {
        Predicates subQueryPredicate = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Col1)), new Parameter("Open", "RightCol1"));
        Predicates outerPredicate = new Equal(new Column<JoinRightTable>(nameof(JoinRightTable.Col2)), new Parameter("Closed", "RightCol2"));
        Joins joins = Join.Joins(RightOnLeftPredicate(), RightSubquery(subQueryPredicate), "Right");

        SqlQuery query = LeftGenerator.InternalSelect(null, null, null, joins, outerPredicate, null, null, null, null);

        Assert.Equal("SELECT [Left].* FROM [Left] JOIN (SELECT [Right].* FROM [Right] WHERE ([Right].[Col1] = @RightCol1_1)) AS [Right] ON ([Left].[RightId] = [Right].[Id]) WHERE ([Right].[Col2] = @RightCol2_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@RightCol1_1", "Open");
        SqlQueryTestHelper.AssertParameterValue(query, "@RightCol2_2", "Closed");
    }

    [Fact]
    public void Select_WithSubqueryJoin_AllowsSubqueryToHaveItsOwnJoins()
    {
        Joins subQueryJoins = Join<JoinLastTable>.Joins(LastOnRightPredicate());
        Joins joins = Join.Joins(RightOnLeftPredicate(), RightSubquery(null, subQueryJoins), "Right");

        SqlQuery query = LeftGenerator.InternalSelect(null, null, null, joins, null, null, null, null, null);

        Assert.Equal("SELECT [Left].* FROM [Left] JOIN (SELECT [Right].* FROM [Right] JOIN [Last] ON ([Right].[LastId] = [Last].[Id])) AS [Right] ON ([Left].[RightId] = [Right].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
