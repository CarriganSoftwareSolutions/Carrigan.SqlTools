using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when invoking a model instance from a result row fails.
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

        string preview = BuildPreview(names);
        return $"Failed to materialize '{modelType.Name}' from result row. Columns={names.Count}{preview}.";
    }

    private static string BuildPreview(List<string> names)
    {
        if (names.Count == 0)
            return string.Empty;

        int max = Math.Min(10, names.Count);
        List<string> previewNames = [];
        for (int i = 0; i < max; i++)
        {
            previewNames.Add(names[i]);
        }

        return $", Preview='{string.Join(", ", previewNames)}'";
    }
}
