using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.Tests.TestComparers;

/// <summary>
/// Compares two <see cref="ColumnInfo"/> instances by ALL of their internal readonly properties:
/// ColumnTag, ColumnName, PropertyInfo, PropertyName, ParameterTag, SelectTag,
/// IsKeyPart, IsEncrypted, IsKeyVersionProperty.
/// </summary>
public class ColumnInfoAllPropertiesComparer : IEqualityComparer<ColumnInfo>
{
    public static readonly ColumnInfoAllPropertiesComparer Instance = new();

    private ColumnInfoAllPropertiesComparer() { }

    public bool Equals(ColumnInfo? left, ColumnInfo? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        // Use each property's native equality (IEquatable<T> or reference equality)
        return EqualityComparer<ColumnTag>.Default.Equals(left.ColumnTag, right.ColumnTag)
            && EqualityComparer<ColumnName>.Default.Equals(left.ColumnName, right.ColumnName)
            && EqualityComparer<PropertyInfo>.Default.Equals(left.PropertyInfo, right.PropertyInfo)
            && EqualityComparer<PropertyName>.Default.Equals(left.PropertyName, right.PropertyName)
            && EqualityComparer<ParameterTag>.Default.Equals(left.ParameterTag, right.ParameterTag)
            && EqualityComparer<SelectTag>.Default.Equals(left.SelectTag, right.SelectTag)
            && EqualityComparer<AliasName>.Default.Equals(left.AliasName, right.AliasName)
            && left.IsKeyPart == right.IsKeyPart
            && left.IsEncrypted == right.IsEncrypted
            && left.IsKeyVersionProperty == right.IsKeyVersionProperty
            && left.SqlType.Type == right.SqlType.Type
            && left.SqlType.TypeDeclaration == right.SqlType.TypeDeclaration;
    }

    public int GetHashCode(ColumnInfo obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        // Include ALL properties in the hash. HashCode handles nulls and custom comparer.
        HashCode hashCode = new ();
        hashCode.Add(obj.ColumnTag, EqualityComparer<ColumnTag>.Default);
        hashCode.Add(obj.ColumnName, EqualityComparer<ColumnName>.Default);
        hashCode.Add(obj.PropertyInfo, EqualityComparer<PropertyInfo>.Default);
        hashCode.Add(obj.PropertyName, EqualityComparer<PropertyName>.Default);
        hashCode.Add(obj.ParameterTag, EqualityComparer<ParameterTag>.Default);
        hashCode.Add(obj.SelectTag, EqualityComparer<SelectTag>.Default);
        hashCode.Add(obj.IsKeyPart);
        hashCode.Add(obj.IsEncrypted);
        hashCode.Add(obj.IsKeyVersionProperty);
        hashCode.Add(obj.SqlType.Type.ToString());
        hashCode.Add(obj.SqlType.TypeDeclaration);
        return hashCode.ToHashCode();
    }
}
