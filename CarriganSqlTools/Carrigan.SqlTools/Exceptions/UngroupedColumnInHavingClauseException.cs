namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a HAVING clause references a non-aggregate expression that is not represented by the GROUP BY clause.
/// </summary>
/// <remarks>
/// Non-aggregate portions of HAVING must be represented by the grouping keys. Columns contained within aggregate expressions
/// do not need to be included in the GROUP BY clause.
/// </remarks>
public sealed class UngroupedColumnInHavingClauseException : AggregateInconsistencyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UngroupedColumnInHavingClauseException"/> class.
    /// </summary>
    internal UngroupedColumnInHavingClauseException()
        : base("Non-aggregate expressions referenced by a HAVING clause must be represented by the GROUP BY clause.")
    {
    }
}
