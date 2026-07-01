namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a SELECT projection mixes aggregate and non-aggregate expressions.
/// </summary>
public sealed class MixedAggregateSelectException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MixedAggregateSelectException"/> class.
    /// </summary>
    internal MixedAggregateSelectException()
        : base("SELECT projections must not mix aggregate and non-aggregate expressions.")
    {
    }
}
