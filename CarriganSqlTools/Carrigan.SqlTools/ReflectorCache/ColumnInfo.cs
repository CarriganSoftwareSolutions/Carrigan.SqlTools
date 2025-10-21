using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

/// <summary>
/// Represents reflection-based metadata describing a single property
/// in a data model that maps to a SQL column.  
/// <para>
/// Each instance of <see cref="ColumnInfo"/> provides a cached snapshot of
/// identifiers, tags, and attribute-derived information for the associated property,
/// enabling efficient column and parameter resolution during SQL generation.
/// </para>
/// </summary>
/// <remarks>
/// Cached values include schema, table, and column identifiers as well as
/// metadata derived from custom attributes such as
/// <see cref="ColumnAttribute"/>, <see cref="IdentifierAttribute"/>,
/// <see cref="AliasAttribute"/>, <see cref="EncryptedAttribute"/>,
/// and <see cref="KeyVersionAttribute"/>.
/// </remarks>
public class ColumnInfo : IComparable<ColumnInfo>, IEquatable<ColumnInfo>, IEqualityComparer<ColumnInfo>
{
    /// <summary>
    /// The fully qualified <see cref="Tags.ColumnTag"/> that represents
    /// the SQL identifier for this column, including schema and table context.
    /// </summary>
    internal readonly ColumnTag ColumnTag;

    /// <summary>
    /// The <see cref="IdentifierTypes.ColumnName"/> derived from the property.
    /// This name must not be <c>null</c>, empty, or whitespace.
    /// </summary>
    internal readonly ColumnName ColumnName;

    /// <summary>
    /// The <see cref="System.Reflection.PropertyInfo"/> instance
    /// corresponding to the reflected property in the data model.
    /// </summary>
    internal readonly PropertyInfo PropertyInfo;

    /// <summary>
    /// The <see cref="IdentifierTypes.PropertyName"/> representing
    /// the property name within the data model.
    /// </summary>
    internal readonly PropertyName PropertyName;

    /// <summary>
    /// The <see cref="Tags.ParameterTag"/> associated with this property,
    /// used when generating parameterized SQL statements.
    /// </summary>
    internal readonly ParameterTag ParameterTag;

    /// <summary>
    /// The <see cref="IdentifierTypes.AliasName"/> defined for this property,
    /// if specified via an <see cref="AliasAttribute"/>.
    /// </summary>
    internal readonly AliasName? AliasName;

    /// <summary>
    /// The <see cref="Tags.SelectTag"/> used to represent this column
    /// in SELECT clauses, including alias handling.
    /// </summary>
    internal readonly SelectTag SelectTag;

    /// <summary>
    /// Indicates whether this property is part of the data model’s key definition.
    /// </summary>
    internal readonly bool IsKeyPart;

    /// <summary>
    /// Indicates whether this property is marked as encrypted using
    /// an <see cref="EncryptedAttribute"/>.
    /// </summary>
    internal readonly bool IsEncrypted;


    /// <summary>
    /// Indicates whether this property is designated as a key version property
    /// using a <see cref="KeyVersionAttribute"/>.
    /// </summary>
    internal readonly bool IsKeyVersionProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnInfo"/> class,
    /// caching the reflected column metadata for a specific property.
    /// </summary>
    /// <param name="schemaName">
    /// The <see cref="SchemaName"/> that identifies the schema for the data model.
    /// </param>
    /// <param name="tableName">
    /// The <see cref="TableName"/> that identifies the table associated with the data model.
    /// </param>
    /// <param name="propertyInfo">
    /// The <see cref="PropertyInfo"/> representing the reflected property.
    /// </param>
    /// <param name="keys">
    /// A collection of <see cref="System.Reflection.PropertyInfo"/> instances representing the key properties
    /// for the entity, used to determine the value of <see cref="IsKeyPart"/>.
    /// </param>
    internal ColumnInfo(SchemaName? schemaName, TableName tableName, PropertyInfo propertyInfo, IEnumerable<PropertyInfo> keys)
    {
        string? columnName = propertyInfo.GetCustomAttribute<IdentifierAttribute>()?.Name?.ToString().GetValueOrNull()
            ?? propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name?.GetValueOrNull()
            ?? propertyInfo.Name;

        string? parameterName = propertyInfo.GetCustomAttribute<ParameterAttribute>()?.Name?.GetValueOrNull() ?? columnName;

        AliasName? aliasName = propertyInfo.GetCustomAttribute<AliasAttribute>()?.Name;

        ColumnName = new ColumnName(columnName);
        ColumnTag = new(new(schemaName, tableName), ColumnName);
        PropertyInfo = propertyInfo;
        PropertyName = new(PropertyInfo.Name);
        ParameterTag = new ParameterTag(null, parameterName, null);
        AliasName = aliasName;
        SelectTag = new(ColumnTag, AliasTag.New(aliasName));

        IsKeyPart = keys.Contains(PropertyInfo);
        IsEncrypted = PropertyInfo.GetCustomAttribute<EncryptedAttribute>() != null;
        IsKeyVersionProperty = PropertyInfo.GetCustomAttribute<KeyVersionAttribute>() != null;
    }

    /// <summary>
    /// Implicitly converts a <see cref="ColumnInfo"/> to its fully qualified SQL
    /// string representation in the format <c>[Schema].[Table].[Column]</c>.
    /// </summary>
    /// <param name="value">The <see cref="ColumnInfo"/> to convert.</param>
    /// <returns>
    /// A SQL-formatted string representing the column name, including schema
    /// and table context where applicable.
    /// </returns>
    public static implicit operator string(ColumnInfo value)
        => value.ColumnTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ColumnInfo"/> instance,
    /// equivalent to its implicit conversion to <see cref="string"/>.
    /// </summary>
    /// <returns>
    /// The SQL-formatted identifier string for this column.
    /// </returns>
    public override string ToString()
        => this;

    /// <summary>
    /// Compares this <see cref="ColumnInfo"/> to another instance to determine
    /// their relative sort order, using the <see cref="ColumnTag"/> as the comparison key.
    /// </summary>
    /// <param name="other">The <see cref="ColumnInfo"/> to compare with the current instance.</param>
    /// <returns>
    /// A signed integer indicating the relative order of the two objects:
    /// <c>0</c> if equal, a negative value if this instance precedes
    /// <paramref name="other"/>, or a positive value if it follows.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </remarks>
    public int CompareTo(ColumnInfo? other)
    {
        if (other is null) return 1;
        return string.Compare(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether this instance and another <see cref="ColumnInfo"/> represent
    /// the same column, using a case-insensitive comparison of their <see cref="ColumnTag"/> values.
    /// </summary>
    /// <param name="other">The <see cref="ColumnInfo"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if both represent the same column; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(ColumnInfo? other)
    {
        if (other is null) return false;
        return string.Equals(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// <see cref="ColumnInfo"/> instance, using the <see cref="ColumnTag"/> property
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is a <see cref="ColumnInfo"/> and
    /// represents the same column (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and delegates to
    /// <see cref="Equals(ColumnInfo?)"/>.
    /// </remarks>
    public override bool Equals(object? obj) =>
        obj is ColumnInfo ct && Equals(ct);


    /// <summary>
    /// Returns a hash code for this instance, consistent with
    /// the case-insensitive equality comparison on <see cref="ColumnTag"/>.
    /// </summary>
    public override int GetHashCode() =>
        ColumnTag.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ColumnInfo"/> instances represent the same column.
    /// </summary>
    public bool Equals(ColumnInfo? x, ColumnInfo? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ColumnInfo"/> instance.
    /// </summary>
    public int GetHashCode(ColumnInfo obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ColumnInfo"/> instances represent
    /// the same column, using case-insensitive comparison of their tags.
    /// </summary>
    public static bool operator ==(ColumnInfo? left, ColumnInfo? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="ColumnInfo"/> instances do not represent
    /// the same column.
    /// </summary>
    public static bool operator !=(ColumnInfo? left, ColumnInfo? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether this <see cref="ColumnInfo"/> instance represents
    /// an empty or invalid column (for debugging or validation purposes).
    /// </summary>
    /// <returns>
    /// <c>true</c> if the string representation of this column is <c>null</c>,
    /// empty, or whitespace; otherwise, <c>false</c>.
    /// </returns>
    internal bool IsEmpty() =>
        ToString().IsNullOrWhiteSpace();
}
