using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Base class for SQL aggregate expressions.
/// </summary>
public abstract class Aggregates : SqlExpression
{
    /// <summary>
    /// The aggregate function name to render.
    /// </summary>
    protected readonly string FunctionName;

    /// <summary>
    /// Initializes a new aggregate expression.
    /// </summary>
    /// <param name="functionName">The aggregate function name.</param>
    /// <param name="expressions">The expressions supplied to the aggregate function.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="functionName"/> or <paramref name="expressions"/> is <c>null</c>.
    /// </exception>
    protected Aggregates(string functionName, params IEnumerable<SqlExpression> expressions)
        : base(expressions, ToBaseString(functionName, expressions))
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(functionName, nameof(functionName));
        FunctionName = functionName;
    }

    /// <summary>
    /// Aggregate functions are valid aggregate SELECT expressions.
    /// </summary>
    /// <param name="groupBys">The optional <c>GROUP BY</c> clause.</param>
    /// <returns>Always <c>true</c>.</returns>
    public override bool IsAggregate(GroupBysBase? groupBys) =>
        true;

    /// <summary>
    /// Produces the SQL fragment represented by this aggregate expression.
    /// </summary>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText($"{FunctionName}(");

        if (ChildNodes.Any())
        {
            bool isFirstExpression = true;

            foreach (SqlExpression expression in ChildNodes)
            {
                if (!isFirstExpression)
                {
                    yield return new SqlFragmentText(", ");
                }

                foreach (ISqlFragment fragment in expression.ToSqlFragments(dialect))
                {
                    yield return fragment;
                }

                isFirstExpression = false;
            }
        }
        else
            yield return new SqlFragmentText("*");

        yield return new SqlFragmentText(")");
    }

    /// <summary>
    /// Returns an unquoted dialect neutral aggregate expression representation.
    /// </summary>
    private static string ToBaseString(string functionName, params IEnumerable<SqlExpression> expressions) =>
        expressions.Any()
            ? $"{functionName}({string.Join(", ", expressions)})"
            : $"{functionName}(*)";
}
