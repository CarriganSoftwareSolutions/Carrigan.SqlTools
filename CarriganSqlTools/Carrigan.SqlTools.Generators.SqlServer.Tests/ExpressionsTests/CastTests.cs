using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public sealed class CastTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Cast(null!, SqlServerTypesProvider.AsNVarChar(100, true))),
    ];


    private static readonly ISqlDialects Dialect = new SqlServerDialect();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Cast(ParameterValue, SqlServerTypesProvider.AsNVarChar(100, true)),
        new Cast(DifferentParameter, SqlServerTypesProvider.AsNVarChar(100, true)),
        new Cast(ParameterValue, SqlServerTypesProvider.AsDecimal(18, 2, true)),
        new Cast(ColumnA, SqlServerTypesProvider.AsNVarChar(100, true)),
        new Cast(ColumnB, SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Cast(new Add(ColumnA, ParameterValue), SqlServerTypesProvider.AsNVarChar(100, true)),
        new Cast(new Add(ColumnA, ParameterDifferentValue), SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Cast(AggregateWithParameterNoColumn, SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Cast(ParameterValue, SqlServerTypesProvider.AsNVarChar(100, true)),
        new Cast(ColumnA, SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Cast(ColumnA, SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Cast(ParameterValue, SqlServerTypesProvider.AsNVarChar(100, true)),
        new Cast(AggregateWithParameterNoColumn, SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Cast(ParameterValue, SqlServerTypesProvider.AsNVarChar(100, true)),
        new Cast(AggregateWithParameterNoColumn, SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Cast(ColumnA, SqlServerTypesProvider.AsNVarChar(100, true)),
    ];

    [Fact]
    public void SelectTag_WithCast_RendersExpectedSql()
    {
        string expected = "CAST([Customer].[Id] AS NVARCHAR(100)) AS [IdText]";
        Cast cast = new
        (
            new Column<Customer>(nameof(Customer.Id)),
            SqlServerTypesProvider.AsNVarChar(100, true)
        );
        SelectTag select = new(cast, "IdText");

        string actual = select.ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Select_WithCastAroundAggregate_RendersExpectedGroupedQuery()
    {
        string expected = "SELECT [Customer].[Name], AVG(CAST([Customer].[Id] AS DECIMAL(18, 2))) AS [AverageId] FROM [Customer] GROUP BY [Customer].[Name]";
        SqlGenerator<Customer> generator = new();
        GroupBys groupBys = new GroupBys<Customer>(nameof(Customer.Name));

        Column<Customer> customerIdColumn = new (nameof(Customer.Id));
        Cast customerId = new(customerIdColumn, SqlServerTypesProvider.AsDecimal(18, 2, true));

        Average averageId = new(customerId);
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(averageId, "AverageId")
        );

        SqlQuery actual = generator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal(expected, actual.QueryText);
    }
}
