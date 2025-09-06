using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Predicates;

public class Columns  <T> : PredicatesBase, IColumnValue
{
    public string Name { get; }
    public ColumnTag ColumnTag { get; }
    public TableTag TableTag { get; }

    public Columns(string column)
    {
        if (column.IsNullOrEmpty())
            throw new ArgumentException($"{nameof(column)} requires a value.", column);
        else if (SqlToolsReflectorCache<T>.ColumnNamesHashSet.Contains(column) is false)
            throw new ArgumentException($"{column} is not the valid name of a column in table, {SqlToolsReflectorCache<T>.TableName}.", nameof(column));


        TableTag = new TableTag(SqlToolsReflectorCache<T>.TableSchema ?? string.Empty, SqlToolsReflectorCache<T>.TableName ?? string.Empty);
        ColumnTag = new ColumnTag(TableTag, column);
        Name = column;
    }

    internal override IEnumerable<Parameters> Parameter =>
        [];

    internal override IEnumerable<IColumnValue> Column =>
        [this];

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        ColumnTag;

    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        [];

    public static IEnumerable<Columns<T>> Get(params IEnumerable<string> columnNames)
    {
        IEnumerable<string> invalid = columnNames.Where(columnName => SqlToolsReflectorCache<T>.ColumnNamesHashSet.Contains(columnName) is false);
        if (invalid.Any())
            throw SqlIdentifierException.FromInvalidColumnNames<T>(invalid);
        return columnNames.Select(column => new Columns<T>(column));
    }

    public static IEnumerable<Columns<T>> Get() =>
        SqlToolsReflectorCache<T>.ColumnNames.Select(column => new Columns<T>(column));
}
