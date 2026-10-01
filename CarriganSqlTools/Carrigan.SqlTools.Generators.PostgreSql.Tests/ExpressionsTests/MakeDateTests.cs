using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class MakeDateTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new MakeDate(null!, Second, Third)),
        (() => new MakeDate(First, null!, Third)),
        (() => new MakeDate(First, Second, null!)),
    ];



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new MakeDate(AggregateWithParameterNoColumn, ColumnA, Third)),
        (() => new MakeDate(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => new MakeDate(ColumnA, AggregateWithParameterNoColumn, Third)),
        (() => new MakeDate(First, AggregateWithParameterNoColumn, ColumnA)),
        (() => new MakeDate(ColumnA, Second, AggregateWithParameterNoColumn)),
        (() => new MakeDate(First, ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new MakeDate(First, Second, Third),
        new MakeDate(First, Second, DifferentParameter),
        new MakeDate(First, DifferentParameter, Third),
        new MakeDate(ColumnA, ParameterValue, ParameterValue),
        new MakeDate(ColumnB, ParameterValue, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new MakeDate(ColumnA, ParameterValue, ParameterValue),
        new MakeDate(ColumnA, ParameterDifferentValue, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new MakeDate(AggregateWithParameterNoColumn, Second, Third),
        new MakeDate(First, AggregateWithParameterNoColumn, Third),
        new MakeDate(First, Second, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new MakeDate(First, Second, Third),
        new MakeDate(ColumnA, ColumnB, Column1),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new MakeDate(ColumnA, Second, Third),
        new MakeDate(First, ColumnA, Third),
        new MakeDate(First, Second, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new MakeDate(First, Second, Third),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new MakeDate(First, Second, Third),
        new MakeDate(ColumnA, Second, Third),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new MakeDate(ColumnA, ColumnB, Column1),
    ];

    [Fact]
    public void ExpressionConstructor_Test()
    {
        SqlExpression expression = new MakeDate(new Parameter(2026, "Year"), new Parameter(9, "Month"), new Parameter(23, "Day"));
        Assert.Equal("MAKE_DATE(Year, Month, Day)", expression.ToString());
    }

    [Fact]
    public void IntegerConstructor_WrapsValuesInParameters_Test()
    {
        MakeDate expression = new(2026, 9, 23);
        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(3, children.Length);
        Assert.Equal(2026, (int)Assert.IsType<Parameter>(children[0]).Value!);
        Assert.Equal(9, (int)Assert.IsType<Parameter>(children[1]).Value!);
        Assert.Equal(23, (int)Assert.IsType<Parameter>(children[2]).Value!);
    }
}
