using Carrigan.Core.Extensions;
using SqlTools.Exceptions;
using System.Reflection;

namespace SqlTools.Tags;

public class TableTag : IComparable<TableTag>, IEquatable<TableTag>, IEqualityComparer<TableTag>
{
    private string _tableTag;

    internal TableTag(string? schemaName, string tableName)
    {
        if (tableName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(tableName), $"{nameof(tableName)} requires a value.");
        else
            _tableTag = schemaName.IsNullOrEmpty() ? $"[{tableName}]" : $"[{schemaName}].[{tableName}]";

        if (SqlIdentifierPattern.Fails(tableName))
            throw new SqlNamePatternException(this);
        if(schemaName.IsNotNullOrWhiteSpace() && SqlIdentifierPattern.Fails(schemaName))
            throw new SqlNamePatternException(this);
    }

    public static implicit operator string(TableTag tableTag) =>
        tableTag._tableTag;

    public override string ToString() =>
        _tableTag;

    public int CompareTo(TableTag? other)
    {
        if (other is null) return 1; 
        return string.Compare(_tableTag, other._tableTag, StringComparison.Ordinal);
    }

    public bool Equals(TableTag? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(_tableTag, other._tableTag, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TableTag);
    }

    public override int GetHashCode()
    {
        return _tableTag.GetHashCode(StringComparison.Ordinal);
    }

    public bool Equals(TableTag? x, TableTag? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x._tableTag, y._tableTag, StringComparison.Ordinal);
    }

    public int GetHashCode(TableTag obj)
    {
        return obj._tableTag.GetHashCode(StringComparison.Ordinal);
    }
    public static bool operator ==(TableTag? left, TableTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(TableTag? left, TableTag? right) => !(left == right);

    public static TableTag Get(Type value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Construct the generic type: SqlToolsReflectorCache<value>
        Type cacheType = typeof(SqlToolsReflectorCache<>).MakeGenericType(value);

        // Get the static property 'TableTag' on the constructed type.
        PropertyInfo tableTagProperty = cacheType.GetProperty("TableTag", BindingFlags.Public | BindingFlags.Static) ?? throw new InvalidOperationException($"The property 'TableTag' was not found on type '{cacheType.FullName}'.");

        // Retrieve the value of the TableTag property.
        return (TableTag)tableTagProperty.GetValue(null) ?? throw new InvalidOperationException($"The property 'TableTag' on type '{cacheType.FullName}' returned null.");
    }
}

