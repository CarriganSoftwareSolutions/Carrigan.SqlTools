using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithOneExpressionOneStringBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New(SqlExpression? sqlExpression, string? characters);

    protected abstract FunctionalExpression New(SqlExpression? sqlExpression, char[]? characters);

    private static Parameter Value => new(1, "Value");
    private static Add LeftRight => new(Left, Right);
    private static Parameter Left => new(1, "Left");
    private static Parameter Right => new(2, "Right");

    [Fact]
    public void Constructor_NullValueWithStringCharacters_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, "xy"));

    [Fact]
    public void Constructor_NullValueWithCharArrayCharacters_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, ['x', 'y']));

    [Fact]
    public void Constructor_NullStringCharacters_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(Value, (string?)null));

    [Fact]
    public void Constructor_NullCharArrayCharacters_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(Value, (char[]?)null));

    [Fact]
    public void ToString_StringCharacters_RendersCharactersAsParameter() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter)",
            New(Value, "xy").ToString()
        );

    [Fact]
    public void ToString_CharArrayCharacters_RendersCharactersAsParameter() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter)",
            New(Value, ['x', 'y']).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Parameter)",
            New(LeftRight, "xy").ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValueAndCharactersParameter()
    {
        FunctionalExpression expression = New(Value, "xy");

        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(2, actual.Length);
        Assert.Equal(Value, actual[0]);

        Parameter characters = Assert.IsType<Parameter>(actual[1]);
        Assert.Equal("xy", characters.Value);
    }

    [Fact]
    public void Constructor_CharArray_MaterializesCharactersAsString()
    {
        char[] characters = ['x', 'y'];

        FunctionalExpression expression = New(Value, characters);
        characters[0] = 'z';

        Parameter parameter = Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(1));
        Assert.Equal("xy", parameter.Value);
    }

    [Fact]
    public void Characters_AreNotEmbeddedInRenderedSql()
    {
        const string characters = "'); DROP TABLE Customer;--";

        string actual = New(Value, characters).ToString();

        Assert.Equal($"{ExpectedFunctionName}(Value, Parameter)", actual);
        Assert.DoesNotContain(characters, actual);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        FunctionalExpression first = New(new Parameter(1, "Value"), "xy");
        FunctionalExpression equivalent = New(new Parameter(10, "Value"), "ab");
        FunctionalExpression different = New(new Parameter(1, "Different"), "xy");

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValue() =>
        Assert.False(New(Value, "xy").IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValue() =>
        Assert.True(New(new Count(Value), "xy").IsAggregate());
}