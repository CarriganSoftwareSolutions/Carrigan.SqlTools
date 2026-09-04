using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class ArithmeticExpressionTests
{
    private sealed class TestArithmeticExpression : ArithmeticExpression
    {
        public TestArithmeticExpression(string operation, IEnumerable<NumericExpression> numericExpressions) : base(operation, numericExpressions)
        {
        }
    }

    [Fact]
    public void Constructor_NullOperation_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new TestArithmeticExpression(null!, [new NumericParameter<int>(1, "Left")]));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_InvalidOperation_Exception(string operation) =>
        Assert.Throws<ArgumentException>(() => new TestArithmeticExpression(operation, [new NumericParameter<int>(1, "Left")]));

    [Fact]
    public void Constructor_NullExpressions_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new TestArithmeticExpression("+", null!));

    [Fact]
    public void Constructor_EmptyExpressions_Exception() =>
        Assert.Throws<ArgumentException>(() => new TestArithmeticExpression("+", []));

    [Fact]
    public void Constructor_NullExpression_Exception() =>
        Assert.Throws<NullReferenceException>(() => new TestArithmeticExpression("+", [new NumericParameter<int>(1, "Left"), null!]));

    [Fact]
    public void ToString_SingleExpression() =>
        Assert.Equal("Left", new TestArithmeticExpression("+", [new NumericParameter<int>(1, "Left")]).ToString());

    [Fact]
    public void ToString_MultipleExpressions() =>
        Assert.Equal("(Left + Right)", new TestArithmeticExpression("+", [new NumericParameter<int>(1, "Left"), new NumericParameter<int>(2, "Right")]).ToString());

    [Fact]
    public void Negate_NullExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Negate(null!));

    [Fact]
    public void Negate_ToString() =>
        Assert.Equal("(-Value)", new Negate(new NumericParameter<int>(1, "Value")).ToString());
}