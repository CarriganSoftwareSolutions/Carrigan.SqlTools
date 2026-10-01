using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithThreeExpressionsBase : SqlExpressionsBaseTests
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New
    (
        SqlExpression? first,
        SqlExpression? second,
        SqlExpression? third
    );


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => New(AggregateWithParameterNoColumn, ColumnA, Third)),
        (() => New(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => New(ColumnA, AggregateWithParameterNoColumn, Third)),
        (() => New(First, AggregateWithParameterNoColumn, ColumnA)),
        (() => New(ColumnA, Second, AggregateWithParameterNoColumn)),
        (() => New(First, ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        New(First, Second, Third),
        New(First, Second, DifferentParameter),
        New(First, DifferentParameter, Third),
        New(ColumnA, ParameterValue, ParameterValue),
        New(ColumnB, ParameterValue, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        New(ColumnA, ParameterValue, ParameterValue),
        New(ColumnA, ParameterDifferentValue, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        New(AggregateWithParameterNoColumn, Second, Third),
        New(First, AggregateWithParameterNoColumn, Third),
        New(First, Second, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        New(First, Second, Third),
        New(ColumnA, ColumnB, Column1),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        New(ColumnA, Second, Third),
        New(First, ColumnA, Third),
        New(First, Second, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        New(First, Second, Third),
        New(AggregateWithParameterNoColumn, Second, Third),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        New(First, Second, Third),
        New(ColumnA, Second, Third),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        New(ColumnA, ColumnB, Column1),
    ];

    [Fact]
    public void Constructor_NullFirstValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, Second, Third));

    [Fact]
    public void Constructor_NullSecondValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(First, null, Third));

    [Fact]
    public void Constructor_NullThirdValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(First, Second, null));

    [Fact]
    public void ToString_RendersValues() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(First, Second, Third)",
            New(First, Second, Third).ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Second, Third)",
            New(LeftRight, Second, Third).ToString()
        );

    [Fact]
    public void ChildNodes_ContainsValues()
    {
        FunctionalExpression expression = New(First, Second, Third);

        SqlExpression[] actual = [.. expression.ChildNodes];
        SqlExpression[] expected = [First, Second, Third];

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        FunctionalExpression first = New
        (
            new Parameter(1, "First"),
            new Parameter(2, "Second"),
            new Parameter(3, "Third")
        );

        FunctionalExpression equivalent = New
        (
            new Parameter(10, "First"),
            new Parameter(20, "Second"),
            new Parameter(30, "Third")
        );

        FunctionalExpression different = New
        (
            new Parameter(1, "First"),
            new Parameter(2, "Second"),
            new Parameter(3, "Different")
        );

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }
}
