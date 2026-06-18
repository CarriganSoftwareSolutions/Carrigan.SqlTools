using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class LogicalOperatorTests
{
    private static readonly SqlServerDialect Dialect = new();

    private sealed class TestLogicalOperator : LogicalOperator
    {
        public TestLogicalOperator(string op, params IEnumerable<Predicates> predicates) : base(op, predicates)
        {
        }
    }

    [Fact]
    public void LogicalOperator_OperatorNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(() =>
            new TestLogicalOperator(null!, [new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive))]));

    [Fact]
    public void LogicalOperator_OperatorWhitespace_ThrowsArgumentException() =>
        _ = Assert.Throws<ArgumentException>(() =>
            new TestLogicalOperator(" ", [new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive))]));

    [Fact]
    public void LogicalOperator_EmptyPredicates_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentException>(() =>
            new TestLogicalOperator("AND", []));

    [Fact]
    public void LogicalOperator_SinglePredicate_ToSql_DoesNotAddParenthesisOrOperator()
    {
        TestLogicalOperator op = new("AND",
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
        ]);

        string expected = "[LogicalPredicateTable].[IsActive]";
        string actual = op.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LogicalOperator_MultiplePredicates_ToSql_AddsParenthesisAndOperator()
    {
        TestLogicalOperator op = new("AND",
        [
            new BooleanColumn<LogicalPredicateTable>(nameof(LogicalPredicateTable.IsActive)),
            new IsNotNull(new Parameter(1, "P1")),
        ]);

        string expected = "([LogicalPredicateTable].[IsActive] AND (@P1_1 IS NOT NULL))";
        string actual = op.ToSqlFragments(Dialect).ToSql(Dialect);

        Assert.Equal(expected, actual);
    }
}
