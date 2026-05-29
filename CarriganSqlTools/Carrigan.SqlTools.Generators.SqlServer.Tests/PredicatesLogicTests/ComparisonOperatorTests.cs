using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

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
        Predicates left = new Parameter(1, "P1");
        Predicates right = new Parameter(2, "P2");

        _ = Assert.Throws<ArgumentNullException>(() =>
            new TestComparisonOperator(left, right, null!));
    }

    [Fact]
    public void ComparisonOperator_OperatorWhitespace_ThrowsArgumentException()
    {
        Predicates left = new Parameter(1, "P1");
        Predicates right = new Parameter(2, "P2");

        _ = Assert.Throws<ArgumentException>(() =>
            new TestComparisonOperator(left, right, " "));
    }
}
