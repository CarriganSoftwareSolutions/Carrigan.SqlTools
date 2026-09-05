namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a WHERE clause contains an aggregate expression.
/// </summary>
/// <remarks>
/// Aggregate expressions cannot be evaluated by a WHERE clause because WHERE filters rows
/// before grouping and aggregation occur. Use a HAVING clause to filter aggregate results.
/// </remarks>
public sealed class AggregateExpressionInWhereClauseException : AggregateInconsistencyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateExpressionInWhereClauseException"/> class.
    /// </summary>
    internal AggregateExpressionInWhereClauseException() : base("WHERE clauses cannot contain aggregate expressions. Use HAVING to filter aggregate results.")
    {
    }
}