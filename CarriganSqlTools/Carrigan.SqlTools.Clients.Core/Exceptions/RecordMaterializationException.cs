using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.Clients.Core.Exceptions;

/// <summary>
/// Thrown when materializing a record into a model instance fails.
/// </summary>
public sealed class RecordMaterializationException : SqlToolsQueryException
{
    /// <summary>
    /// Gets the model type being materialized from a data-reader record.
    /// </summary>
    public Type ModelType { get; }
    /// <summary>
    /// Gets the result-column names available when materialization failed.
    /// </summary>
    public IEnumerable<string> ColumnNames { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordMaterializationException"/> class.
    /// </summary>
    /// <param name="modelType">The model type being read or materialized.</param>
    /// <param name="columnNames">The SQL column names involved in materialization.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public RecordMaterializationException(Type modelType, IEnumerable<string> columnNames, Exception innerException)
        : base(BuildMessage(modelType, columnNames), innerException)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(columnNames);
        ArgumentNullException.ThrowIfNull(innerException);

        ModelType = modelType;
        ColumnNames = columnNames.Materialize(NullOptionsEnum.FilteredOut);
    }

    /// <summary>
    /// Builds the exception message based on the model type and column names.
    /// </summary>
    /// <param name="modelType">The model type being read or materialized.</param>
    /// <param name="columnNames">The SQL column names involved in materialization.</param>
    /// <returns>The constructed exception message.</returns>
    private static string BuildMessage(Type modelType, IEnumerable<string> columnNames)
    {
        List<string> names = [];
        foreach (string name in columnNames)
        {
            names.Add(name);
        }

        return $"Failed to materialize '{modelType.Name}' from result row. Columns={names.Count}, ColumnNames='{string.Join(", ", names)}'.";
    }
}
