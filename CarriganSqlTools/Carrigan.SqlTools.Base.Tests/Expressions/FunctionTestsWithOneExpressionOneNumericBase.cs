using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithOneExpressionOneNumericBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New(SqlExpression? sqlExpression, int number);
    protected abstract FunctionalExpression New(SqlExpression? sqlExpression, float number);
    protected abstract FunctionalExpression New(SqlExpression? sqlExpression, double number);
    protected abstract FunctionalExpression New(SqlExpression? sqlExpression, decimal number);

    private static Parameter Value => new(1, "Value");
    private static Add LeftRight => new(Left, Right);
    private static Parameter Left => new(1, "Left");
    private static Parameter Right => new(2, "Right");

    [Fact]
    public void Constructor_NullValueWithInt_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, 42));

    [Fact]
    public void Constructor_NullValueWithFloat_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, float.E));

    [Fact]
    public void Constructor_NullValueWithDouble_Exception() =>

        Assert.Throws<ArgumentNullException>(() => New(null, double.Tau));
    [Fact]
    public void Constructor_NullValueWithDecimal_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, 3.141m));

    [Fact]
    public void ToString_StringCharacters_RendersIntAsParameter() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter)",
            New(Value, 42).ToString()
        );

    [Fact]
    public void ToString_StringCharacters_RendersFloatAsParameter() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter)",
            New(Value, float.MaxValue).ToString()
        );

    [Fact]
    public void ToString_StringCharacters_RendersDoubleAsParameter() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter)",
            New(Value, double.MinValue).ToString()
        );

    [Fact]
    public void ToString_StringCharacters_RendersDecimalAsParameter() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter)",
            New(Value, decimal.MaxValue).ToString()
        );

    [Fact]
    public void ToString_RendersIntWithNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Parameter)",
            New(LeftRight, 42).ToString()
        );

    [Fact]
    public void ToString_RenderFloatWithNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Parameter)",
            New(LeftRight, float.MaxValue).ToString()
        );

    [Fact]
    public void ToString_RenderDoubleWithNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Parameter)",
            New(LeftRight, double.MaxValue).ToString()
        );

    [Fact]
    public void ToString_RenderDecimalWithNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Parameter)",
            New(LeftRight, decimal.MaxValue).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValueAndIntParameter()
    {
        FunctionalExpression expression = New(Value, 42);

        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(2, actual.Length);
        Assert.Equal(Value, actual[0]);

        Parameter parameter = Assert.IsType<Parameter>(actual[1]);
        Assert.Equal(42, parameter.Value);
    }

    [Fact]
    public void ChildNodes_ContainsValueAndFloatParameter()
    {
        FunctionalExpression expression = New(Value, float.Pi);

        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(2, actual.Length);
        Assert.Equal(Value, actual[0]);

        Parameter parameter = Assert.IsType<Parameter>(actual[1]);
        Assert.Equal(float.Pi, parameter.Value);
    }

    [Fact]
    public void ChildNodes_ContainsValueAndDoubleParameter()
    {
        FunctionalExpression expression = New(Value, double.Tau);

        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(2, actual.Length);
        Assert.Equal(Value, actual[0]);

        Parameter parameter = Assert.IsType<Parameter>(actual[1]);
        Assert.Equal(double.Tau, parameter.Value);
    }

    [Fact]
    public void ChildNodes_ContainsValueAndDecimalParameter()
    {
        FunctionalExpression expression = New(Value, 3.141m);

        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(2, actual.Length);
        Assert.Equal(Value, actual[0]);

        Parameter parameter = Assert.IsType<Parameter>(actual[1]);
        Assert.Equal(3.141m, parameter.Value);
    }

    [Fact]
    public void Int_NotEmbeddedInRenderedSql()
    {
        const string characters = "42";

        string actual = New(Value, 42).ToString();

        Assert.Equal($"{ExpectedFunctionName}(Value, Parameter)", actual);
        Assert.DoesNotContain(characters, actual);
    }

    [Fact]
    public void Float_NotEmbeddedInRenderedSql()
    {
        const string characters = "3.14";

        string actual = New(Value, float.Pi).ToString();

        Assert.Equal($"{ExpectedFunctionName}(Value, Parameter)", actual);
        Assert.DoesNotContain(characters, actual);
    }

    [Fact]
    public void Double_NotEmbeddedInRenderedSql()
    {
        const string characters = "3.14";

        string actual = New(Value, double.Pi).ToString();

        Assert.Equal($"{ExpectedFunctionName}(Value, Parameter)", actual);
        Assert.DoesNotContain(characters, actual);
    }

    [Fact]
    public void Decimal_NotEmbeddedInRenderedSql()
    {
        const string characters = "3.14";

        string actual = New(Value, 3.141m).ToString();

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