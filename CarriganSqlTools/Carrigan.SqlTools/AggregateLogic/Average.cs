using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Convenience alias for SQL's <c>AVG</c> aggregate function.
/// </summary>
public sealed class Average : Avg
{
    /// <summary>
    /// Initializes an <c>AVG(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to average.</param>
    public Average(SqlExpression expression) : base(expression)
    {
    }
}
