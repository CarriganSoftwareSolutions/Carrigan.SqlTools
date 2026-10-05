namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when aggregate and non-aggregate expressions are used inconsistently within the same expression context.
/// </summary>
public class AggregateInconsistencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateInconsistencyException"/> class.
    /// </summary>
    internal AggregateInconsistencyException() : base("Aggregate and non-aggregate expressions cannot be mixed inconsistently within the same expression context.")
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateInconsistencyException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The message describing the aggregate-expression inconsistency.</param>
    protected AggregateInconsistencyException(string message) : base(message)
    {
    }
}

