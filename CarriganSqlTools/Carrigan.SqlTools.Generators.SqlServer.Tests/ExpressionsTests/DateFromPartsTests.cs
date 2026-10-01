using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DateFromPartsTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new DateFromParts(null!, Second, Third)),
        (() => new DateFromParts(First, null!, Third)),
        (() => new DateFromParts(First, Second, null!)),
    ];



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new DateFromParts(AggregateWithParameterNoColumn, ColumnA, Third)),
        (() => new DateFromParts(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new DateFromParts(ColumnA, AggregateWithParameterNoColumn, Third)),
        (() => new DateFromParts(First, AggregateWithParameterNoColumn, ColumnA)),
        (() => new DateFromParts(ColumnA, Second, AggregateWithParameterNoColumn)),
        (() => new DateFromParts(First, ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new DateFromParts(First, Second, Third),
        new DateFromParts(First, Second, DifferentParameter),
        new DateFromParts(First, DifferentParameter, Third),
        new DateFromParts(ColumnA, ParameterValue, ParameterValue),
        new DateFromParts(ColumnB, ParameterValue, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new DateFromParts(ColumnA, ParameterValue, ParameterValue),
        new DateFromParts(ColumnA, ParameterDifferentValue, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new DateFromParts(AggregateWithParameterNoColumn, Second, Third),
        new DateFromParts(First, AggregateWithParameterNoColumn, Third),
        new DateFromParts(First, Second, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new DateFromParts(First, Second, Third),
        new DateFromParts(ColumnA, ColumnB, Column1),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new DateFromParts(ColumnA, Second, Third),
        new DateFromParts(First, ColumnA, Third),
        new DateFromParts(First, Second, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new DateFromParts(First, Second, Third),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new DateFromParts(First, Second, Third),
        new DateFromParts(ColumnA, Second, Third),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new DateFromParts(ColumnA, ColumnB, Column1),
    ];

    [Fact]
    public void ExpressionConstructor_Test()
    {
        SqlExpression expression = new DateFromParts(new Parameter(2026, "Year"), new Parameter(9, "Month"), new Parameter(23, "Day"));
        Assert.Equal("DATEFROMPARTS(Year, Month, Day)", expression.ToString());
    }

    [Fact]
    public void IntegerConstructor_WrapsValuesInParameters_Test()
    {
        DateFromParts expression = new(2026, 9, 23);
        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(3, children.Length);
        Assert.Equal(2026, (int)Assert.IsType<Parameter>(children[0]).Value!);
        Assert.Equal(9, (int)Assert.IsType<Parameter>(children[1]).Value!);
        Assert.Equal(23, (int)Assert.IsType<Parameter>(children[2]).Value!);
    }
}
