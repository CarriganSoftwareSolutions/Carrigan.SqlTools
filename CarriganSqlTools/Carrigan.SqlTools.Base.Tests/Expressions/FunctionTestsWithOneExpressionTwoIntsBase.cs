using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

//IGNORE SPELLING: Ints

public abstract class FunctionTestsWithOneExpressionTwoIntsBase : SqlExpressionsBaseTests
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New(SqlExpression? expression, int first, int second);


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        New(Value, 2, 3),
        New(DifferentParameter, 2, 3),
        New(ColumnA, 2, 3),
        New(ColumnB, 2, 3),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        New(ColumnA, 2, 3),
        New(ColumnA, 4, 5),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        New(AggregateWithParameterNoColumn, 2, 3),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        New(Value, 2, 3),
        New(ColumnA, 2, 3),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        New(ColumnA, 2, 3),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        New(Value, 2, 3),
        New(AggregateWithParameterNoColumn, 2, 3),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        New(Value, 2, 3),
        New(ColumnA, 2, 3),
        New(AggregateWithParameterNoColumn, 2, 3),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
        [];

    [Fact]
    public void Constructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, 2, 3));

    [Fact]
    public void ToString_RendersConstantsAsParameters() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter, Parameter)",
            New(Value, 2, 3).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}((Left + Right), Parameter, Parameter)",
            New(LeftRight, 2, 3).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValueAndParameters()
    {
        FunctionalExpression expression = New(Value, 2, 3);
        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(3, actual.Length);
        Assert.Equal(Value, actual[0]);
        Assert.Equal(2, Assert.IsType<Parameter>(actual[1]).Value);
        Assert.Equal(3, Assert.IsType<Parameter>(actual[2]).Value);
    }

    [Fact]
    public void Constants_AreNotEmbeddedInRenderedSql()
    {
        string actual = New(Value, 1776, 2026).ToString();

        Assert.Equal($"{ExpectedFunctionName}(Value, Parameter, Parameter)", actual);
        Assert.DoesNotContain("1776", actual);
        Assert.DoesNotContain("2026", actual);
    }

    [Fact]
    public void Equality_UsesExpressionStructure()
    {
        FunctionalExpression first = New(new Parameter(1, "Value"), 2, 3);
        FunctionalExpression equivalent = New(new Parameter(10, "Value"), 20, 30);
        FunctionalExpression different = New(new Parameter(1, "Different"), 2, 3);

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValue() =>
        Assert.False(New(Value, 2, 3).IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValue() =>
        Assert.True(New(new Count(Value), 2, 3).IsAggregate());
}
