using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Tests.TestComparers;

/// <summary>
/// Compares two <see cref="ColumnInfo"/> instances by ALL of their internal readonly fields:
/// ColumnTag, ColumnName, PropertyInfo, PropertyName, ParameterTag, SelectTag,
/// IsKeyPart, IsEncrypted, IsKeyVersionField.
/// </summary>
public class ColumnInfoAllFieldsComparer : IEqualityComparer<ColumnInfo>
{
    public static readonly ColumnInfoAllFieldsComparer Instance = new();

    private ColumnInfoAllFieldsComparer() { }

    public bool Equals(ColumnInfo? left, ColumnInfo? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        // Use each field's native equality (IEquatable<T> or reference equality)
        return EqualityComparer<ColumnTag>.Default.Equals(left.ColumnTag, right.ColumnTag)
            && EqualityComparer<ColumnName>.Default.Equals(left.ColumnName, right.ColumnName)
            && EqualityComparer<PropertyInfo>.Default.Equals(left.PropertyInfo, right.PropertyInfo)
            && EqualityComparer<PropertyName>.Default.Equals(left.PropertyName, right.PropertyName)
            && EqualityComparer<ParameterTag>.Default.Equals(left.ParameterTag, right.ParameterTag)
            && EqualityComparer<SelectTag>.Default.Equals(left.SelectTag, right.SelectTag)
            && left.IsKeyPart == right.IsKeyPart
            && left.IsEncrypted == right.IsEncrypted
            && left.IsKeyVersionField == right.IsKeyVersionField;
    }

    public int GetHashCode(ColumnInfo obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        // Include ALL fields in the hash. HashCode handles nulls and custom comparer.
        HashCode hashCode = new ();
        hashCode.Add(obj.ColumnTag, EqualityComparer<ColumnTag>.Default);
        hashCode.Add(obj.ColumnName, EqualityComparer<ColumnName>.Default);
        hashCode.Add(obj.PropertyInfo, EqualityComparer<PropertyInfo>.Default);
        hashCode.Add(obj.PropertyName, EqualityComparer<PropertyName>.Default);
        hashCode.Add(obj.ParameterTag, EqualityComparer<ParameterTag>.Default);
        hashCode.Add(obj.SelectTag, EqualityComparer<SelectTag>.Default);
        hashCode.Add(obj.IsKeyPart);
        hashCode.Add(obj.IsEncrypted);
        hashCode.Add(obj.IsKeyVersionField);
        return hashCode.ToHashCode();
    }
}
