namespace Carrigan.SqlTools.Clients.Core.Exceptions;

/// <summary>
/// Thrown when reading a record from a data reader fails.
/// </summary>
public sealed class DataReaderFailedException : SqlToolsQueryException
{
    /// <summary>
    /// Gets the model type being read from the data reader when the failure occurred.
    /// </summary>
    public Type ModelType { get; }
    /// <summary>
    /// Gets the column ordinal being read, when the ordinal was known.
    /// </summary>
    public int? Ordinal { get; }
    /// <summary>
    /// Gets the column name being read, when the column name was known.
    /// </summary>
    public string? ColumnName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataReaderFailedException"/> class.
    /// </summary>
    /// <param name="modelType">The model type being read or materialized.</param>
    /// <param name="ordinal">The zero-based data reader ordinal being read, when available.</param>
    /// <param name="columnName">The SQL column name to apply.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public DataReaderFailedException(Type modelType, int? ordinal, string? columnName, Exception innerException)
        : base(BuildMessage(modelType, ordinal, columnName), innerException)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(innerException);

        ModelType = modelType;
        Ordinal = ordinal;
        ColumnName = columnName;
    }

    /// <summary>
    /// Builds the exception message based on the model type and column details.
    /// </summary>
    /// <param name="modelType">The model type being read or materialized.</param>
    /// <param name="ordinal">The zero-based data reader ordinal being read, when available.</param>
    /// <param name="columnName">The SQL column name to apply.</param>
    /// <returns>The constructed exception message.</returns>

    private static string BuildMessage(Type modelType, int? ordinal, string? columnName)
    {
        string ordinalDisplay = ordinal is null ? string.Empty : $", Ordinal={ordinal.Value}";
        string columnDisplay = string.IsNullOrWhiteSpace(columnName) ? string.Empty : $", Column='{columnName}'";
        return $"Failed to read record for '{modelType.Name}'{ordinalDisplay}{columnDisplay}.";
    }
}
