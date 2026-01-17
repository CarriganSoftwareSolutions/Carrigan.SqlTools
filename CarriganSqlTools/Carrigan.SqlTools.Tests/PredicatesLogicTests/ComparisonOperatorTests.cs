using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class ComparisonOperatorTests
{
    private sealed class TestComparisonOperator : ComparisonOperator
    {
        public TestComparisonOperator(Predicates left, Predicates right, string op) : base(left, right, op)
        {
        }
    }

    [Fact]
    public void ComparisonOperator_OperatorNull_ThrowsArgumentNullException()
    {
        Predicates left = new Parameter("P1", 1, null);
        Predicates right = new Parameter("P2", 2, null);

        _ = Assert.Throws<ArgumentNullException>(() =>
            new TestComparisonOperator(left, right, null!));
    }

    [Fact]
    public void ComparisonOperator_OperatorWhitespace_ThrowsArgumentException()
    {
        Predicates left = new Parameter("P1", 1, null);
        Predicates right = new Parameter("P2", 2, null);

        _ = Assert.Throws<ArgumentException>(() =>
            new TestComparisonOperator(left, right, " "));
    }
}
