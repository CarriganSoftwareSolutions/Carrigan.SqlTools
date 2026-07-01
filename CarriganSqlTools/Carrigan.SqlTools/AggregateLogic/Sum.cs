using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>SUM</c> aggregate function.
/// </summary>
public sealed class Sum : Aggregates
{
    /// <summary>
    /// Initializes a <c>SUM(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to sum.</param>
    public Sum(SqlExpression expression) : base("SUM", expression)
    {
    }
}
