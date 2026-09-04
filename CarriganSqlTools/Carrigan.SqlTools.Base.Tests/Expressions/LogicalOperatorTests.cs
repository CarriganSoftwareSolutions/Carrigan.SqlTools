using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class LogicalOperatorTests
{
    private sealed class TestLogicalOperator : LogicalOperator
    {
        public TestLogicalOperator(string operation, IEnumerable<Predicates> predicates) : base(operation, predicates)
        {
        }
    }

    [Fact]
    public void Constructor_NullOperation_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new TestLogicalOperator(null!, [NewPredicate("Left")]));

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_InvalidOperation_Exception(string operation) =>
        Assert.Throws<ArgumentException>(() => new TestLogicalOperator(operation, [NewPredicate("Left")]));

    [Fact]
    public void Constructor_NullPredicates_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new TestLogicalOperator("AND", null!));

    [Fact]
    public void Constructor_EmptyPredicates_Exception() =>
        Assert.Throws<ArgumentException>(() => new TestLogicalOperator("AND", []));

    [Fact]
    public void Constructor_NullPredicate_Exception() =>
        Assert.Throws<NullReferenceException>(() => new TestLogicalOperator("AND", [NewPredicate("Left"), null!]));

    [Fact]
    public void ToString_SinglePredicate() =>
        Assert.Equal("(Left IS NOT NULL)", new TestLogicalOperator("AND", [NewPredicate("Left")]).ToString());

    [Fact]
    public void ToString_MultiplePredicates() =>
        Assert.Equal("((Left IS NOT NULL) AND (Right IS NOT NULL))", new TestLogicalOperator("AND", [NewPredicate("Left"), NewPredicate("Right")]).ToString());

    private static Predicates NewPredicate(string name) =>
        new IsNotNull(new Parameter(1, name));
}