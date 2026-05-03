using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Tests.PredicatesLogicTests;

public class LogicalOperatorTests
{
    private sealed class TestLogicalOperator : LogicalOperator
    {
        public TestLogicalOperator(string op, params IEnumerable<Predicates> predicates) : base(op, predicates)
        {
        }
    }

    [Fact]
    public void LogicalOperator_OperatorNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(() =>
            new TestLogicalOperator(null!, [new Parameter("P1", 1, null)]));

    [Fact]
    public void LogicalOperator_OperatorWhitespace_ThrowsArgumentException() =>
        _ = Assert.Throws<ArgumentException>(() =>
            new TestLogicalOperator(" ", [new Parameter("P1", 1, null)]));

    [Fact]
    public void LogicalOperator_EmptyPredicates_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentException>(() =>
            new TestLogicalOperator("AND", []));

    [Fact]
    public void LogicalOperator_SinglePredicate_ToSql_DoesNotAddParensOrOperator()
    {
        TestLogicalOperator op = new("AND",
        [
            new Parameter("P1", 1, null),
        ]);

        string expected = "@P1_1";
        string actual = op.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LogicalOperator_MultiplePredicates_ToSql_AddsParensAndOperator()
    {
        TestLogicalOperator op = new("AND",
        [
            new Parameter("P1", 1, null),
            new Parameter("P2", 2, null),
        ]);

        string expected = "(@P1_1 AND @P2_2)";
        string actual = op.ToSqlFragments().ToSql(new SqlServerDialect());

        Assert.Equal(expected, actual);
    }
}