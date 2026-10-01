using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public sealed class TryCastTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new TryCast(null!, SqlServerTypesProvider.AsInt())),
        (() => new TryCast(Value, null!)),
    ];


    private static readonly ISqlDialects Dialect = new SqlServerDialect();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new TryCast(ParameterValue, SqlServerTypesProvider.AsInt()),
        new TryCast(DifferentParameter, SqlServerTypesProvider.AsInt()),
        new TryCast(ParameterValue, SqlServerTypesProvider.AsBigInt()),
        new TryCast(ColumnA, SqlServerTypesProvider.AsInt()),
        new TryCast(ColumnB, SqlServerTypesProvider.AsInt()),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new TryCast(new Add(ColumnA, ParameterValue), SqlServerTypesProvider.AsInt()),
        new TryCast(new Add(ColumnA, ParameterDifferentValue), SqlServerTypesProvider.AsInt()),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new TryCast(AggregateWithParameterNoColumn, SqlServerTypesProvider.AsInt()),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new TryCast(ParameterValue, SqlServerTypesProvider.AsInt()),
        new TryCast(ColumnA, SqlServerTypesProvider.AsInt()),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new TryCast(ColumnA, SqlServerTypesProvider.AsInt()),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new TryCast(ParameterValue, SqlServerTypesProvider.AsInt()),
        new TryCast(AggregateWithParameterNoColumn, SqlServerTypesProvider.AsInt()),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new TryCast(ParameterValue, SqlServerTypesProvider.AsInt()),
        new TryCast(AggregateWithParameterNoColumn, SqlServerTypesProvider.AsInt()),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new TryCast(ColumnA, SqlServerTypesProvider.AsInt()),
    ];

    [Fact]
    public void ToSql_RendersExpectedSql()
    {
        TryCast expression = new(new Parameter("123", "Value"), SqlServerTypesProvider.AsInt(true));
        SelectTag select = new(expression, "Result");

        Assert.Equal("TRY_CAST(@Value_1 AS INT) AS [Result]", select.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_NullExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new TryCast(null!, SqlServerTypesProvider.AsInt()));

    [Fact]
    public void Constructor_NullFieldProperties_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new TryCast(new Parameter("123"), null!));

    [Fact]
    public void ChildNodes_ContainsExpression()
    {
        Parameter value = new("123", "Value");
        TryCast expression = new(value, SqlServerTypesProvider.AsInt());

        Assert.Equal(value, Assert.Single(expression.ChildNodes));
    }

    [Fact]
    public void Equality_UsesExpressionAndTargetType()
    {
        TryCast first = new(new Parameter("123", "Value"), SqlServerTypesProvider.AsInt());
        TryCast equivalent = new(new Parameter("456", "Value"), SqlServerTypesProvider.AsInt());
        TryCast differentType = new(new Parameter("123", "Value"), SqlServerTypesProvider.AsBigInt());

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, differentType);
    }

    [Fact]
    public void IsAggregate_DelegatesToExpression() =>
        Assert.True(new TryCast(new Average(new Parameter(1)), SqlServerTypesProvider.AsDecimal(18, 2)).IsAggregate());
}
