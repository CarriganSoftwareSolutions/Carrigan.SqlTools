using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithTwoExpressionsBase : SqlExpressionsBaseTests
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New
    (
        SqlExpression? first,
        SqlExpression? second
    );


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => New(AggregateWithParameterNoColumn, ColumnA)),
        (() => New(ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        New(First, Second),
        New(First, Third),
        New(Second, Third),
        New(ColumnA, ParameterValue),
        New(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        New(ColumnA, ParameterValue),
        New(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        New(AggregateWithParameterNoColumn, Second),
        New(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        New(First, Second),
        New(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        New(ColumnA, Second),
        New(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        New(First, Second),
        New(AggregateWithParameterNoColumn, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        New(First, Second),
        New(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        New(ColumnA, ColumnB),
    ];

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
}