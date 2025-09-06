using Carrigan.Core.Extensions;

namespace SqlTools.Sets;

public class SetColumns<T>
{
    public IEnumerable<string> ColumnNames { get; private set; }
    public SetColumns(params IEnumerable<string> columnNames)
    {
        IEnumerable<string> invalid = columnNames.Where(columnName => SqlToolsReflectorCache<T>.ColumnNamesHashSet.Contains(columnName) is false);
        if (invalid.Any())
            throw new ArgumentException($"{invalid.JoinAnd()} are not valid columns in table, {SqlToolsReflectorCache<T>.TableName}.", nameof(columnNames));
        ColumnNames = columnNames;
    }
    public void AddColumn(string columnName)
    {
        if(SqlToolsReflectorCache<T>.ColumnNamesHashSet.Contains(columnName) is false)
            throw new ArgumentException($"{columnName} is not valid column in table, {SqlToolsReflectorCache<T>.TableName}.", nameof(columnName));
        ColumnNames = ColumnNames.Append(columnName);
    }
}
