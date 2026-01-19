namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when reading a record from a data reader fails.
/// </summary>
public sealed class DataReaderFailedException : SqlToolsSqlServerException
{
    public Type ModelType { get; }
    public int? Ordinal { get; }
    public string? ColumnName { get; }

    public DataReaderFailedException(Type modelType, int? ordinal, string? columnName, Exception innerException)
        : base(BuildMessage(modelType, ordinal, columnName), innerException)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(innerException);

        ModelType = modelType;
        Ordinal = ordinal;
        ColumnName = columnName;
    }

    private static string BuildMessage(Type modelType, int? ordinal, string? columnName)
    {
        string ordinalDisplay = ordinal is null ? string.Empty : $", Ordinal={ordinal.Value}";
        string columnDisplay = string.IsNullOrWhiteSpace(columnName) ? string.Empty : $", Column='{columnName}'";
        return $"Failed to read record for '{modelType.Name}'{ordinalDisplay}{columnDisplay}.";
    }
}
