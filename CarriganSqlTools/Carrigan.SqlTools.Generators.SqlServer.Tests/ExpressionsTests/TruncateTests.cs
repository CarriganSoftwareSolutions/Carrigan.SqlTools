using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class TruncateTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Truncate(null!)),
        (() => new Truncate(null!, 2)),
        (() => new Truncate(null!, Precision)),
        (() => new Truncate(PiValue, (SqlExpression)null!)),
    ];


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Truncate(AggregateWithParameterNoColumn, ColumnA)),
        (() => new Truncate(ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Truncate(First, Second),
        new Truncate(First, Third),
        new Truncate(Second, Third),
        new Truncate(ColumnA, ParameterValue),
        new Truncate(ColumnB, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Truncate(ColumnA, ParameterValue),
        new Truncate(ColumnA, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Truncate(AggregateWithParameterNoColumn, Second),
        new Truncate(First, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Truncate(First, Second),
        new Truncate(ColumnA, ColumnB),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Truncate(ColumnA, Second),
        new Truncate(First, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Truncate(First, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Truncate(First, Second),
        new Truncate(ColumnA, Second),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Truncate(ColumnA, ColumnB),
    ];

    [Fact]
    public void Constructor_WithSingleExpression_GeneratesExpectedSql()
    {
        SqlExpression expression = new Truncate(PiValue);

        Assert.Equal
        (
            "ROUND(PiValue, Parameter, 1)",
            expression.ToString()
        );
    }

    [Fact]
    public void Constructor_WithIntegerPrecision_GeneratesExpectedSql()
    {
        SqlExpression expression = new Truncate(PiValue, 2);

        Assert.Equal
        (
            "ROUND(PiValue, Parameter, 1)",
            expression.ToString()
        );
    }

    [Fact]
    public void Constructor_WithExpressionPrecision_GeneratesExpectedSql()
    {
        SqlExpression expression = new Truncate(PiValue, Precision);

        Assert.Equal
        (
            "ROUND(PiValue, Precision, 1)",
            expression.ToString()
        );
    }

    [Fact]
    public void Constructor_WithSingleExpression_UsesZeroPrecision()
    {
        Truncate expression = new(PiValue);

        Parameter precision = Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(1));

        Assert.Equal(0, precision.Value);
    }

    [Fact]
    public void Constructor_WithIntegerPrecision_EncapsulatesPrecisionInParameter()
    {
        Truncate expression = new(PiValue, 2);

        Parameter precision = Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(1));

        Assert.Equal(2, precision.Value);
    }

    [Fact]
    public void Constructor_WithExpressionPrecision_UsesProvidedExpression()
    {
        Truncate expression = new(PiValue, Precision);

        Assert.Equal(PiValue, expression.ChildNodes.ElementAt(0));
        Assert.Equal(Precision, expression.ChildNodes.ElementAt(1));
    }

    [Fact]
    public void Constructor_WithNullExpression_Throws() =>
        Assert.Throws<ArgumentNullException>
        (
            () => new Truncate(null!)
        );

    [Fact]
    public void Constructor_WithNullPrecisionExpression_Throws() => 
        Assert.Throws<ArgumentNullException>
        (
            () => new Truncate(PiValue, (SqlExpression)null!)
        );

    [Fact]
    public void ChildNodes_ContainsValueAndPrecision()
    {
        Truncate expression = new(PiValue, Precision);

        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(2, children.Length);
        Assert.Equal(PiValue, children[0]);
        Assert.Equal(Precision, children[1]);
    }
}