using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithTwoExpressionsBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New
    (
        SqlExpression? first,
        SqlExpression? second
    );

    private static Parameter First => new(1, "First");
    private static Parameter Second => new(2, "Second");
    private static Add LeftRight => new(Left, Right);
    private static Parameter Left => new(1, "Left");
    private static Parameter Right => new(2, "Right");

    [Fact]
    public void Constructor_NullFirstValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, Second));

    [Fact]
    public void Constructor_NullSecondValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(First, null));

    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(First, Second)",
            New(First, Second).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Second)",
            New(LeftRight, Second).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValues()
    {
        FunctionalExpression expression = New(First, Second);

        SqlExpression[] actual = [.. expression.ChildNodes];
        SqlExpression[] expected = [First, Second];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        FunctionalExpression first = New
        (
            new Parameter(1, "First"),
            new Parameter(2, "Second")
        );

        FunctionalExpression equivalent = New
        (
            new Parameter(10, "First"),
            new Parameter(20, "Second")
        );

        FunctionalExpression different = New
        (
            new Parameter(1, "First"),
            new Parameter(2, "Different")
        );

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValues() =>
        Assert.False(New(First, Second).IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValues() =>
        Assert.True
        (
            New
            (
                new Count(First),
                new Average(Second)
            ).IsAggregate()
        );

    [Fact]
    public void IsAggregate_AggregateAndRowIndependentValues_ReturnsTrue() =>
        Assert.True
        (
            New
            (
                new Count(First),
                Second
            ).IsAggregate()
        );

    [Fact]
    public void IsAggregate_AggregateAndColumnValue_Exception() =>
        Assert.Throws<AggregateInconsistencyException>
        (
            () => New
            (
                new Count(First),
                new TestColumnExpression()
            ).IsAggregate()
        );
}