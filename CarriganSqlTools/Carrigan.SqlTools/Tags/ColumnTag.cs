using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// This class represents the Column "Tag", or identifier. IE [Schema].[Table].[Column]
/// [Schema] is only included if you define the schema of the table.
/// Aside from various comparison and equality interfaces, this class is locked down to internal.
/// </summary>
public class ColumnTag : IComparable<ColumnTag>, IEquatable<ColumnTag>, IEqualityComparer<ColumnTag>
{
    private string _columnTag;
    internal ColumnTag(TableTag tableTag,  string columnName)
    {
        if (columnName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(columnName), $"{nameof(columnName)} requires a value.");
        else
            _columnTag = tableTag.ToString().IsNullOrEmpty() ? $"[{columnName}]" : $"{tableTag}.[{columnName}]";
    }

    internal ColumnTag(string? schemaName, string? tableName,  string columnName)
    {
        if (columnName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(columnName), $"{nameof(columnName)} requires a value.");
        if (tableName.IsNotNullOrEmpty())
            _columnTag = _columnTag = new ColumnTag(new TableTag(schemaName, tableName), columnName);
        else
            _columnTag = $"[{columnName}]";
    }

    public static implicit operator string(ColumnTag value) => value._columnTag;

    public override string ToString() => _columnTag;

    public int CompareTo(ColumnTag? other)
    {
        if (other is null) return 1;
        return string.Compare(_columnTag, other._columnTag, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(ColumnTag? other)
    {
        if (other is null) return false;
        return string.Equals(_columnTag, other._columnTag, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is ColumnTag ct && Equals(ct);
    }

    public override int GetHashCode()
    {
        return _columnTag.GetHashCode();
    }

    public bool Equals(ColumnTag? x, ColumnTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    public int GetHashCode(ColumnTag obj)
    {
        return obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();
    }
    public static bool operator ==(ColumnTag? left, ColumnTag? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(ColumnTag? left, ColumnTag? right)
    {
        return !(left == right);
    }
}
