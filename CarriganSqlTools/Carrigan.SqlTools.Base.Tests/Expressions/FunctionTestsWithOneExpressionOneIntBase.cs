using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithOneExpressionOneIntBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New(SqlExpression? sqlExpression, int precision);

    private static Parameter Value => new(1, "Value");
    private static Add LeftRight => new(Left, Right);
    private static Parameter Left => new(1, "Left");
    private static Parameter Right => new(2, "Right");

    [Fact]
    public void Constructor_NullValueWithStringCharacters_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, 42));

    [Fact]
    public void ToString_StringCharacters_RendersCharactersAsParameter() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter)",
            New(Value, 42).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Parameter)",
            New(LeftRight, 42).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValueAndCharactersParameter()
    {
        FunctionalExpression expression = New(Value, 42);

        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(2, actual.Length);
        Assert.Equal(Value, actual[0]);

        Parameter parameter = Assert.IsType<Parameter>(actual[1]);
        Assert.Equal(42, parameter.Value);
    }

    [Fact]
    public void Characters_AreNotEmbeddedInRenderedSql()
    {
        const string characters = "42";

        string actual = New(Value, 42).ToString();

        Assert.Equal($"{ExpectedFunctionName}(Value, Parameter)", actual);
        Assert.DoesNotContain(characters, actual);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        FunctionalExpression first = New(new Parameter(1, "Value"), 42);
        FunctionalExpression equivalent = New(new Parameter(10, "Value"), 1776);
        FunctionalExpression different = New(new Parameter(1, "Different"), 42);

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValue() =>
        Assert.False(New(Value, 42).IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValue() =>
        Assert.True(New(new Count(Value), 42).IsAggregate());
}