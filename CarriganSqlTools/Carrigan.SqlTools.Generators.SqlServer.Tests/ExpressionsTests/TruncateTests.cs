using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class TruncateTests
{
    private static Parameter Value => new(123.456m, "Value");
    private static Parameter Precision => new(2, "Precision");

    [Fact]
    public void Constructor_WithSingleExpression_GeneratesExpectedSql()
    {
        SqlExpression expression = new Truncate(Value);

        Assert.Equal
        (
            "ROUND(Value, Parameter, 1)",
            expression.ToString()
        );
    }

    [Fact]
    public void Constructor_WithIntegerPrecision_GeneratesExpectedSql()
    {
        SqlExpression expression = new Truncate(Value, 2);

        Assert.Equal
        (
            "ROUND(Value, Parameter, 1)",
            expression.ToString()
        );
    }

    [Fact]
    public void Constructor_WithExpressionPrecision_GeneratesExpectedSql()
    {
        SqlExpression expression = new Truncate(Value, Precision);

        Assert.Equal
        (
            "ROUND(Value, Precision, 1)",
            expression.ToString()
        );
    }

    [Fact]
    public void Constructor_WithSingleExpression_UsesZeroPrecision()
    {
        Truncate expression = new(Value);

        Parameter precision = Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(1));

        Assert.Equal(0, precision.Value);
    }

    [Fact]
    public void Constructor_WithIntegerPrecision_EncapsulatesPrecisionInParameter()
    {
        Truncate expression = new(Value, 2);

        Parameter precision = Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(1));

        Assert.Equal(2, precision.Value);
    }

    [Fact]
    public void Constructor_WithExpressionPrecision_UsesProvidedExpression()
    {
        Truncate expression = new(Value, Precision);

        Assert.Equal(Value, expression.ChildNodes.ElementAt(0));
        Assert.Equal(Precision, expression.ChildNodes.ElementAt(1));
    }

    [Fact]
    public void Constructor_WithNullExpression_Throws()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new Truncate(null!)
        );
    }

    [Fact]
    public void Constructor_WithNullPrecisionExpression_Throws()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => new Truncate(Value, (SqlExpression)null!)
        );
    }

    [Fact]
    public void ChildNodes_ContainsValueAndPrecision()
    {
        Truncate expression = new(Value, Precision);

        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(2, children.Length);
        Assert.Equal(Value, children[0]);
        Assert.Equal(Precision, children[1]);
    }
}