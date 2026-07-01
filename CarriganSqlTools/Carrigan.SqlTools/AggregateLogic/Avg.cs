using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>AVG</c> aggregate function.
/// </summary>
public class Avg : Aggregates
{
    /// <summary>
    /// Initializes an <c>AVG(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to average.</param>
    public Avg(SqlExpression expression) : base("AVG", expression)
    {
    }
}
