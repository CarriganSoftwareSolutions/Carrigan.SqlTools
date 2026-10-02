namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when grouped-query semantics require a non-aggregate SELECT expression to be represented by the GROUP BY clause.
/// </summary>
public sealed class MixedAggregateSelectException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MixedAggregateSelectException"/> class.
    /// </summary>
    internal MixedAggregateSelectException()
        : base("When a SELECT uses aggregate expressions, GROUP BY, or HAVING, every non-aggregate SELECT expression must be represented by the GROUP BY clause.")
    {
    }
}
