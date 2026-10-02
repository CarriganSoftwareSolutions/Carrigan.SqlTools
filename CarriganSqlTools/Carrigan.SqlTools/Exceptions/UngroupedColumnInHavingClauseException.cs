namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a HAVING clause references a non-aggregate column that is not included in the GROUP BY clause.
/// </summary>
/// <remarks>
/// Non-aggregate columns referenced by HAVING must be grouped. Columns contained within aggregate expressions
/// do not need to be included in the GROUP BY clause.
/// </remarks>
public sealed class UngroupedColumnInHavingClauseException : AggregateInconsistencyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UngroupedColumnInHavingClauseException"/> class.
    /// </summary>
    internal UngroupedColumnInHavingClauseException()
        : base("Non-aggregate columns referenced by a HAVING clause must be included in the GROUP BY clause.")
    {
    }
}
