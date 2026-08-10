namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// The exception that is thrown when a numeric expression contains an underlying
/// value whose type is not numeric.
/// </summary>
public class NonNumericValueException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonNumericValueException"/> class.
    /// </summary>
    public NonNumericValueException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonNumericValueException"/> class
    /// with the specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public NonNumericValueException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonNumericValueException"/> class
    /// with the specified error message and inner exception.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    /// <param name="innerException">
    /// The exception that caused the current exception.
    /// </param>
    public NonNumericValueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonNumericValueException"/> class
    /// for the specified non-numeric type.
    /// </summary>
    /// <param name="type">
    /// The underlying type that was expected to be numeric.
    /// </param>
    public NonNumericValueException(Type? type)
        : base($"The underlying value type '{type?.FullName ?? "null"}' is not a numeric type.")
    {
    }
}