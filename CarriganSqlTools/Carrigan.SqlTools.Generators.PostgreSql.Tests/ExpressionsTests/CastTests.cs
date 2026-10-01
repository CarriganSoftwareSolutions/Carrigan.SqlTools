using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public sealed class CastTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Cast(null!, PostgreSqlTypesProvider.AsVarChar(100, false, true))),
    ];


    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Cast(ParameterValue, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
        new Cast(DifferentParameter, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
        new Cast(ParameterValue, PostgreSqlTypesProvider.AsNumeric(18, 2, false, true)),
        new Cast(ColumnA, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
        new Cast(ColumnB, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Cast(new Add(ColumnA, ParameterValue), PostgreSqlTypesProvider.AsVarChar(100, false, true)),
        new Cast(new Add(ColumnA, ParameterDifferentValue), PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Cast(AggregateWithParameterNoColumn, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Cast(ParameterValue, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
        new Cast(ColumnA, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Cast(ColumnA, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Cast(ParameterValue, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
        new Cast(AggregateWithParameterNoColumn, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Cast(ParameterValue, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
        new Cast(AggregateWithParameterNoColumn, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Cast(ColumnA, PostgreSqlTypesProvider.AsVarChar(100, false, true)),
    ];

    [Fact]
    public void SelectTag_WithCast_RendersExpectedSql()
    {
        string expected = "CAST(\"Customer\".\"Id\" AS VARCHAR) AS \"IdText\"";
        Cast cast = new
        (
            new Column<Customer>(nameof(Customer.Id)),
            PostgreSqlTypesProvider.AsVarChar(100, false, true)
        );
        SelectTag select = new(cast, "IdText");

        string actual = select.ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Select_WithCastAroundAggregate_RendersExpectedGroupedQuery()
    {
        string expected = "SELECT \"Customer\".\"Name\", AVG(CAST(\"Customer\".\"Id\" AS NUMERIC)) AS \"AverageId\" FROM \"Customer\" GROUP BY \"Customer\".\"Name\"";
        SqlGenerator<Customer> generator = new();
        GroupBys groupBys = new GroupBys<Customer>(nameof(Customer.Name));

        Column<Customer> customerIdColumn = new(nameof(Customer.Id));
        Cast customerId = new (customerIdColumn, PostgreSqlTypesProvider.AsNumeric(18, 2, false, true));

        Average averageId = new(customerId);
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(averageId, "AverageId")
        );

        SqlQuery actual = generator.InternalSelect(null, null, selects, null, null, groupBys, null, null, null);

        Assert.Equal(expected, actual.QueryText);
    }

    [Fact]
    public void IsAggregate_DelegatesToGroupedColumnExpression()
    {
        Cast cast = new
        (
            new Average(new Column<Customer>(nameof(Customer.Id))),
            PostgreSqlTypesProvider.AsVarChar(100, false)
        );

        bool actual = cast.IsAggregate();

        Assert.True(actual);
    }
}
