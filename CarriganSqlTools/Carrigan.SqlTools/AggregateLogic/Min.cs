using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>MIN</c> aggregate function.
/// </summary>
public sealed class Min : Aggregates
{
    /// <summary>
    /// Initializes a <c>MIN(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to evaluate.</param>
    public Min(SqlExpression expression) : base("MIN", expression)
    {
    }
}
