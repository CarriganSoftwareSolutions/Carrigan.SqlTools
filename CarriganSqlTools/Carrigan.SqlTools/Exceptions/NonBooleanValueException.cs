namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// The exception that is thrown when a Boolean expression contains an underlying
/// value whose type is not Boolean.
/// </summary>
public class NonBooleanValueException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NonBooleanValueException"/> class.
    /// </summary>
    public NonBooleanValueException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonBooleanValueException"/> class
    /// with the specified error message.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    public NonBooleanValueException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonBooleanValueException"/> class
    /// with the specified error message and inner exception.
    /// </summary>
    /// <param name="message">
    /// The message that describes the error.
    /// </param>
    /// <param name="innerException">
    /// The exception that caused the current exception.
    /// </param>
    public NonBooleanValueException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonBooleanValueException"/> class
    /// for the specified non-Boolean type.
    /// </summary>
    /// <param name="type">
    /// The underlying type that was expected to be Boolean.
    /// </param>
    public NonBooleanValueException(Type? type) : base($"The underlying value type '{type?.FullName ?? "null"}' is not a Boolean type.")
    {
    }
}