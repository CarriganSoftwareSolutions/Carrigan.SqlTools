using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class EOMonthTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new EOMonth(null!)),
        (() => new EOMonth(null!, Second)),
        (() => new EOMonth(First, null!)),
    ];


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new EOMonth(AggregateWithParameterNoColumn, ColumnA)),
        (() => new EOMonth(ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new EOMonth(First),
        new EOMonth(Second),
        new EOMonth(First, Second),
        new EOMonth(ColumnA, ParameterValue),
        new EOMonth(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new EOMonth(ColumnA, ParameterValue),
        new EOMonth(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new EOMonth(AggregateWithParameterNoColumn),
        new EOMonth(AggregateWithParameterNoColumn, Second),
        new EOMonth(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new EOMonth(First),
        new EOMonth(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new EOMonth(ColumnA),
        new EOMonth(ColumnA, Second),
        new EOMonth(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new EOMonth(First),
        new EOMonth(AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new EOMonth(First),
        new EOMonth(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new EOMonth(ColumnA),
        new EOMonth(ColumnA, ColumnB),
    ];

    [Fact]
    public void SingleExpressionConstructor_Test() =>
        Assert.Equal("EOMONTH(Value)", new EOMonth(DateValue).ToString());

    [Fact]
    public void TwoExpressionConstructor_Test() =>
        Assert.Equal("EOMONTH(Value, Offset)", new EOMonth(DateValue, new Parameter(1, "Offset")).ToString());
}
