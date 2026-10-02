namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a GROUP BY clause contains an aggregate expression.
/// </summary>
/// <remarks>
/// GROUP BY establishes groups before aggregate expressions are evaluated, so aggregate expressions cannot be used as grouping keys.
/// </remarks>
public sealed class AggregateExpressionInGroupByClauseException : AggregateInconsistencyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateExpressionInGroupByClauseException"/> class.
    /// </summary>
    internal AggregateExpressionInGroupByClauseException()
        : base("GROUP BY clauses cannot contain aggregate expressions.")
    {
    }
}
