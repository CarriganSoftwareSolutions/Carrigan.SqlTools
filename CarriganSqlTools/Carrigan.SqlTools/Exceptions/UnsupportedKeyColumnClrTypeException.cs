namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Represents an error that occurs when one or more key columns use CLR types unsupported by the current SQL dialect.
/// </summary>
public sealed class UnsupportedKeyColumnClrTypeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedKeyColumnClrTypeException"/> class.
    /// </summary>
    public UnsupportedKeyColumnClrTypeException()
        : base("One or more key columns contain unsupported CLR types.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedKeyColumnClrTypeException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public UnsupportedKeyColumnClrTypeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedKeyColumnClrTypeException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public UnsupportedKeyColumnClrTypeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedKeyColumnClrTypeException"/> class.
    /// </summary>
    /// <param name="unsupportedTypes">The unsupported CLR key-column types.</param>
    public UnsupportedKeyColumnClrTypeException(IEnumerable<Type> unsupportedTypes)
        : base(BuildMessage(unsupportedTypes)) => UnsupportedTypes = [.. unsupportedTypes.Distinct()];

    /// <summary>
    /// Gets the UnsupportedTypes value.
    /// </summary>
    public IReadOnlyCollection<Type> UnsupportedTypes { get; } = [];

    private static string BuildMessage(IEnumerable<Type> unsupportedTypes)
    {
        Type[] types = [.. unsupportedTypes.Distinct()];

        return types.Length == 0
            ? "One or more key columns contain unsupported CLR types."
            : $"Key columns contain unsupported CLR types: {string.Join(", ", types.Select(type => type.FullName ?? type.Name))}.";
    }
}
