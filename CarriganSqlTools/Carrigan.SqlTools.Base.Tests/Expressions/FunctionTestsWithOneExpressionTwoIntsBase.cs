using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithOneExpressionTwoIntsBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New(SqlExpression? expression, int first, int second);

    private static Parameter Value => new(1, "Value");
    private static Add LeftRight => new(LeftValue, RightValue);
    private static Parameter LeftValue => new(1, "Left");
    private static Parameter RightValue => new(2, "Right");

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
