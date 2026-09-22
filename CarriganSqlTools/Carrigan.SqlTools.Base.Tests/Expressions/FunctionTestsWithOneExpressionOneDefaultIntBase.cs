using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithOneExpressionOneDefaultIntBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract int DefaultInt { get; }

    protected abstract FunctionalExpression New(SqlExpression? sqlExpression);

    private static readonly Parameter Default = new (Default);

    private static Parameter ParameterValue => new(1);
    private static Parameter ParameterDifferentValue => new(10);
    private static Parameter DifferentParameter => new(20, "Different");
    private static Count Aggregate => new(ParameterValue);
    private static Add LeftRight => new(Left, Right);
    private static Parameter Left => new(1, "Left");
    private static Parameter Right => new(2, "Right");

    [Fact]
    public void Constructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New((SqlExpression)null!));


    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Parameter, Parameter)",
            New(ParameterValue).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}((Left + Right), Parameter)",
            New(LeftRight).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValues()
    {
        Parameter first = new(1);

        SqlExpression[] actual = [.. New(ParameterValue).ChildNodes];

        SqlExpression[] expected = [first, Default];

        Assert.Equal(expected, expected);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        FunctionalExpression first = New(ParameterValue);
        FunctionalExpression equivalent = New(ParameterDifferentValue);
        FunctionalExpression different = New(DifferentParameter);

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValues() =>
        Assert.False(New(ParameterValue).IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValues() =>
        Assert.True
        (
            New(Aggregate).IsAggregate()
        );
}
