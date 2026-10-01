using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public abstract class FunctionTestsWithOneExpressionTwoStringsBase : SqlExpressionsBaseTests
{
    protected abstract string ExpectedFunctionName { get; }

    protected abstract FunctionalExpression New
    (
        SqlExpression? sqlExpression,
        string? firstValue,
        string? secondValue
    );


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        New(Value, "First", "Second"),
        New(DifferentParameter, "First", "Second"),
        New(ColumnA, "First", "Second"),
        New(ColumnB, "First", "Second"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        New(ColumnA, "First", "Second"),
        New(ColumnA, "OtherFirst", "OtherSecond"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        New(AggregateWithParameterNoColumn, "First", "Second"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        New(Value, "First", "Second"),
        New(ColumnA, "First", "Second"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        New(ColumnA, "First", "Second"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        New(Value, "First", "Second"),
        New(AggregateWithParameterNoColumn, "First", "Second"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        New(Value, "First", "Second"),
        New(ColumnA, "First", "Second"),
        New(AggregateWithParameterNoColumn, "First", "Second"),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
        [];

    [Fact]
    public void Constructor_NullExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(null, "First", "Second"));

    [Fact]
    public void Constructor_NullFirstString_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(Value, null, "Second"));

    [Fact]
    public void Constructor_NullSecondString_Exception() =>
        Assert.Throws<ArgumentNullException>(() => New(Value, "First", null));

    [Fact]
    public void ToString_RendersStringsAsParameters() =>
        Assert.Equal
        (
            $"{ExpectedFunctionName}(Value, Parameter, Parameter)",
            New(Value, "First", "Second").ToString()
        );

    [Fact]
    public void ToString_RendersNestedExpression() =>
        Assert.Equal
        (
            //TODO: this works, but we need to try and fix the double parenthesis
            $"{ExpectedFunctionName}((Left + Right), Parameter, Parameter)",
            New(LeftRight, "First", "Second").ToString()
        );

    [Fact]
    public void ChildNodes_ContainsExpressionAndStringParameters()
    {
        FunctionalExpression expression = New(Value, "First", "Second");

        SqlExpression[] actual = [.. expression.ChildNodes];

        Assert.Equal(3, actual.Length);
        Assert.Equal(Value, actual[0]);

        Parameter first = Assert.IsType<Parameter>(actual[1]);
        Assert.Equal("First", first.Value);

        Parameter second = Assert.IsType<Parameter>(actual[2]);
        Assert.Equal("Second", second.Value);
    }

    [Fact]
    public void Strings_AreNotEmbeddedInRenderedSql()
    {
        const string first = "'); DROP TABLE Customer;--";
        const string second = "'); DELETE FROM Customer;--";

        string actual = New(Value, first, second).ToString();

        Assert.Equal($"{ExpectedFunctionName}(Value, Parameter, Parameter)", actual);
        Assert.DoesNotContain(first, actual);
        Assert.DoesNotContain(second, actual);
    }

    [Fact]
    public void Equality_UsesValues()
    {
        FunctionalExpression first = New(new Parameter(1, "Value"), "First", "Second");
        FunctionalExpression equivalent = New(new Parameter(10, "Value"), "OtherFirst", "OtherSecond");
        FunctionalExpression different = New(new Parameter(1, "Different"), "First", "Second");

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }
}
