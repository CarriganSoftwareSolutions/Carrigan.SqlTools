namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Represents an error that occurs when one or more key columns use CLR types unsupported by the current SQL dialect.
/// </summary>
public sealed class UnsupportedKeyColumnClrTypeException : Exception
{
    public UnsupportedKeyColumnClrTypeException()
        : base("One or more key columns contain unsupported CLR types.")
    {
    }

    public UnsupportedKeyColumnClrTypeException(string message)
        : base(message)
    {
    }

    public UnsupportedKeyColumnClrTypeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public UnsupportedKeyColumnClrTypeException(IEnumerable<Type> unsupportedTypes)
        : base(BuildMessage(unsupportedTypes)) => UnsupportedTypes = unsupportedTypes.Distinct().ToArray();

    public IReadOnlyCollection<Type> UnsupportedTypes { get; } = [];

    private static string BuildMessage(IEnumerable<Type> unsupportedTypes)
    {
        Type[] types = unsupportedTypes.Distinct().ToArray();

        return types.Length == 0
            ? "One or more key columns contain unsupported CLR types."
            : $"Key columns contain unsupported CLR types: {string.Join(", ", types.Select(type => type.FullName ?? type.Name))}.";
    }
}