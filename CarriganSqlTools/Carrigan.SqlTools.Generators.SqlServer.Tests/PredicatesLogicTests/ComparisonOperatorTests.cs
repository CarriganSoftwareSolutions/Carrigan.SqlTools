using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class ComparisonOperatorTests
{
    private sealed class TestComparisonOperator : ComparisonOperator
    {
        public TestComparisonOperator(SqlExpression left, SqlExpression right, string op) : base(left, right, op)
        {
        }
    }

    [Fact]
    public void ComparisonOperator_OperatorNull_ThrowsArgumentNullException()
    {
        Parameter left = new (1, "P1");
        Parameter right = new (2, "P2");

        _ = Assert.Throws<ArgumentNullException>(() =>
            new TestComparisonOperator(left, right, null!));
    }

    [Fact]
    public void ComparisonOperator_OperatorWhitespace_ThrowsArgumentException()
    {
        Parameter left = new (1, "P1");
        Parameter right = new (2, "P2");

        _ = Assert.Throws<ArgumentException>(() =>
            new TestComparisonOperator(left, right, " "));
    }
}
