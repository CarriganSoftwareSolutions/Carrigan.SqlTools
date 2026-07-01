using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.AggregateLogic;

/// <summary>
/// Represents SQL's <c>COUNT</c> aggregate function.
/// </summary>
public sealed class Count : Aggregates
{
    /// <summary>
    /// Initializes a <c>COUNT(*)</c> expression.
    /// </summary>
    public Count() : base("COUNT")
    {
    }

    /// <summary>
    /// Initializes a <c>COUNT(expression)</c> expression.
    /// </summary>
    /// <param name="expression">The expression to count.</param>
    public Count(SqlExpression expression) : base("COUNT", expression)
    {
    }
}
