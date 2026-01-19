using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when materializing a record into a model instance fails.
/// </summary>
public sealed class RecordMaterializationException : SqlToolsSqlServerException
{
    public Type ModelType { get; }
    public IEnumerable<string> ColumnNames { get; }

    public RecordMaterializationException(Type modelType, IEnumerable<string> columnNames, Exception innerException)
        : base(BuildMessage(modelType, columnNames), innerException)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(columnNames);
        ArgumentNullException.ThrowIfNull(innerException);

        ModelType = modelType;
        ColumnNames = columnNames.Materialize(Core.Enums.NullOptionsEnum.FilteredOut);
    }

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
