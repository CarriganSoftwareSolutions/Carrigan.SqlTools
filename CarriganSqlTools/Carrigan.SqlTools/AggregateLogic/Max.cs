using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>MAX</c> aggregate function.
/// </summary>
public sealed class Max : Aggregates
{
    /// <summary>
    /// Initializes a <c>MAX(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to evaluate.</param>
    public Max(SqlExpression expression) : base("MAX", expression)
    {
    }
}
