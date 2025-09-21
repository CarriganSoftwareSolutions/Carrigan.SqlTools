using Carrigan.Core.Extensions;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// This class represents the Column "Tag", or identifier. IE [Schema].[Table].[Column]
/// [Schema] is only included if you define the schema of the table.
/// Aside from various comparison and equality interfaces, this class is locked down to internal.
/// </summary>
public class ColumnTag : IComparable<ColumnTag>, IEquatable<ColumnTag>, IEqualityComparer<ColumnTag>
{
    private readonly string _columnTag;
    internal readonly string _columnName;
    internal readonly PropertyInfo _propertyInfo;
    internal readonly ParameterTag _parameterTag;
    internal ColumnTag(TableTag tableTag, string columnName, PropertyInfo propertyInfo, ParameterTag parameterTag)
    {
        if (columnName.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(columnName), $"{nameof(columnName)} requires a value.");
        else
        {
            _columnTag = tableTag.ToString().IsNullOrEmpty() ? $"[{columnName}]" : $"{tableTag}.[{columnName}]";
            _columnName = columnName;
            _propertyInfo = propertyInfo;
            _parameterTag = parameterTag;
        }
    }

    public static implicit operator string(ColumnTag value) => value._columnTag;

    public override string ToString() => this;

    public string ToString(bool useTableTag)
    {
        if (useTableTag)
            return ToString();
        else
            return $"[{_columnName}]";
    }

    public int CompareTo(ColumnTag? other)
    {
        if (other is null) return 1;
        return string.Compare(this, other, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(ColumnTag? other)
    {
        if (other is null) return false;
        return string.Equals(this, other, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) =>
        obj is ColumnTag ct && Equals(ct);

    public override int GetHashCode() =>
        _columnTag.GetHashCode();

    public bool Equals(ColumnTag? x, ColumnTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    public int GetHashCode(ColumnTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

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

    public bool IsEmpty() =>
        ToString().IsNullOrWhiteSpace();
}
