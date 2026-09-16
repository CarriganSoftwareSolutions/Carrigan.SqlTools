using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithMultipleValuesBase
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New(params IEnumerable<SqlExpression>? sqlExpression);

    private static Parameter ParameterValue => new(1);
    private static Parameter ParameterDifferentValue => new(10);
    private static Parameter DifferentParameter => new(20, "Different");
    private static Count Aggregate => new(ParameterValue);
    private static Add LeftRight => new(Left, Right);
    private static Parameter Left => new(1, "Left");
    private static Parameter Right => new(2, "Right");


    [Fact]
    public void Constructor_NullValues_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null!));

    [Fact]
    public void Constructor_NoValues_Exception()
    {
        IEnumerable<SqlExpression> values = [];

        Assert.Throws<ArgumentException>(() => New(values));
    }

    [Fact]
    public void Constructor_OneValue_Exception()
    {
        IEnumerable<SqlExpression> values = [new Parameter(1, "Only")];

        Assert.Throws<ArgumentException>(() => New(values));
    }

    [Fact]
    public void Constructor_NullValue_Exception()
    {
        IEnumerable<SqlExpression> values =
        [
            new Parameter(1, "First"),
            null!
        ];

        Assert.Throws<NullReferenceException>(() => New(values));
    }

    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(First, Second, Third)",
            New
            (
                new Parameter(1, "First"),
                new Parameter(2, "Second"),
                new Parameter(3, "Third")
            ).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}, Fallback)",
            New
            (
                new Add(new Parameter(1, "Left"), new Parameter(2, "Right")),
                new Parameter(0, "Fallback")
            ).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValues()
    {
        Parameter first = new(1, "First");
        Parameter second = new(2, "Second");
        SqlExpression coalesce = New(first, second);

        SqlExpression[] expected = [first, second];

        Assert.Equal(expected, coalesce.ChildNodes);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        SqlExpression first = New(new Parameter(1, "First"), new Parameter(2, "Second"));
        SqlExpression equivalent = New(new Parameter(10, "First"), new Parameter(20, "Second"));
        SqlExpression different = New(new Parameter(1, "First"), new Parameter(2, "Different"));

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_NonAggregateValues() =>
        Assert.False(New(new Parameter(1, "First"), new Parameter(2, "Second")).IsAggregate());

    [Fact]
    public void IsAggregate_AggregateValues() =>
        Assert.True
        (
            New
            (
                new Count(new Parameter(1, "First")),
                new Average(new Parameter(2, "Second"))
            ).IsAggregate()
        );

    [Fact]
    public void IsAggregate_MixedValues_Exception() =>
        Assert.Throws<AggregateInconsistencyException>
        (
            () => New
            (
                new Count(new Parameter(1, "Aggregate")),
                new Parameter(2, "NonAggregate")
            ).IsAggregate()
        );
}
