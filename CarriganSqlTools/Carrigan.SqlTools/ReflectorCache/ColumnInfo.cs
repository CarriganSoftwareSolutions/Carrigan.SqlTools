using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Tags;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: Rework all documentation, examples, and unit tests.

/// <summary>
/// THis represents a variety property information associate with a column.
/// This class caches various informational items about the column:
/// <see cref="ColumnTag"/>
/// <see cref="ColumnName"/>
/// <see cref="PropertyInfo"/>
/// <see cref="PropertyName"/>
/// <see cref="ParameterTag"/>
/// <see cref="SelectTag"/>
/// <c>IsKeyPart</c>
/// <c>IsEncrypted</c>
/// <c>IsKeyVersionField</c>
/// This class is intended for internal use only, but to due to accessibility errors, 
/// I had to expose most of it as public.
/// </summary>
public class ColumnInfo : IComparable<ColumnInfo>, IEquatable<ColumnInfo>, IEqualityComparer<ColumnInfo>
{
    /// <summary>
    /// A string that represent the <see cref="Tags.ColumnTag"/> .
    /// </summary>
    internal readonly ColumnTag ColumnTag;

    /// <summary>
    /// The name of the column. Must not be <c>null</c>, empty, or white space.
    /// </summary>
    internal readonly ColumnName ColumnName;

    /// <summary>
    /// <see cref="PropertyInfo"/> associated with the column in the data model.
    /// </summary>
    internal readonly PropertyInfo PropertyInfo;

    /// <summary>
    /// <see cref="PropertyName"/> associated with the column in the data model.
    /// </summary>
    internal readonly PropertyName PropertyName;

    /// <summary>
    /// The <see cref="Tags.ParameterTag"/> used to represent the column as a SQL parameter
    /// </summary>
    internal readonly ParameterTag ParameterTag;

    //TODO: documentation, unit test
    internal readonly SelectTag SelectTag;

    internal readonly bool IsKeyPart;

    internal readonly bool IsEncrypted;

    internal readonly bool IsKeyVersionField;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnInfo"/> class,
    /// which represents a variety property information associate with a column.
    /// This class caches various informational items about the column:
    /// <see cref="ColumnTag"/>
    /// <see cref="ColumnName"/>
    /// <see cref="PropertyInfo"/>
    /// <see cref="PropertyName"/>
    /// <see cref="ParameterTag"/>
    /// <see cref="SelectTag"/>
    /// <see cref="IsKeyPart"/>
    /// <see cref="IsEncrypted"/>
    /// <see cref="IsKeyVersionField"/>
    /// This class is intended for internal use only, but to due to accessibility errors, 
    /// I had to expose most of it as public.
    /// </summary>
    /// <param name="schemaName">
    /// The <see cref="SchemaName"/> that identifies the table containing the column.
    /// </param>
    /// <param name="tableName">
    /// The <see cref="TableName"/> that identifies the table containing the column.
    /// </param>
    /// <param name="propertyInfo">
    /// The <see cref="PropertyInfo"/> associated with the column in the data model.
    /// </param>
    /// <param name="keys">
    /// An Enumeration of properties that make up the Key for a record, used to determine value of IsKeyPart
    /// </param>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="columnTag"/> fails to meet the SQL identifier naming rules.
    /// </exception>
    /// <exception cref="InvalidKeyVersionFieldType">
    /// Thrown when the  <see cref="KeyVersionAttribute"/> is set, and the key field is not of type <see cref="int"/>
    /// </exception>
    internal ColumnInfo(SchemaName? schemaName, TableName tableName, PropertyInfo propertyInfo, IEnumerable<PropertyInfo> keys)
    {
        string? columnName = propertyInfo.GetCustomAttribute<IdentifierAttribute>()?.Name?.GetValueOrNull()
            ?? propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name?.GetValueOrNull()
            ?? propertyInfo.Name;

        string? parameterName = propertyInfo.GetCustomAttribute<ParameterAttribute>()?.Name?.GetValueOrNull() ?? columnName;

        string? aliasName = propertyInfo.GetCustomAttribute<AliasAttribute>()?.Name;

        ColumnName = new ColumnName(columnName);
        ColumnTag = new(new(schemaName, tableName), ColumnName);
        PropertyInfo = propertyInfo;
        PropertyName = new(PropertyInfo.Name);
        ParameterTag = new ParameterTag(null, parameterName, null);
        SelectTag = new (ColumnTag, aliasName is not null ? new AliasTag( new AliasName(aliasName)): null);

        IsKeyPart = keys.Contains(PropertyInfo);
        IsEncrypted = PropertyInfo.GetCustomAttribute<EncryptedAttribute>() != null;
        IsKeyVersionField = PropertyInfo.GetCustomAttribute<KeyVersionAttribute>() != null;
    }

    /// <summary>
    /// Implicitly converts a <see cref="ColumnInfo"/> to its SQL string representation
    /// in the form <c>[Schema].[Table].[Column]</c> using the <see cref="ColumnTag"/> property.
    /// </summary>
    /// <param name="value">The <see cref="ColumnInfo"/> to convert.</param>
    /// <returns>
    /// A SQL string that fully qualifies the column name, including schema and table if defined.
    /// </returns>
    public static implicit operator string(ColumnInfo value)
        => value.ColumnTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ColumnInfo"/> instance,
    /// equivalent to the result of the implicit conversion to <see cref="string"/>.
    /// </summary>
    /// <returns>
    public override string ToString()
        => this;

    /// <summary>
    /// Compares this <see cref="ColumnInfo"/> to another instance and returns a value
    /// that indicates their relative sort order, using the <see cref="ColumnTag"/> property.
    /// </summary>
    /// <param name="other">
    /// The <see cref="ColumnInfo"/> to compare with the current instance.
    /// </param>
    /// <returns>
    /// A signed integer that indicates the relative order of the two objects:
    /// <c>0</c> if they are equal, a negative value if this instance precedes
    /// <paramref name="other"/>, and a positive value if this instance follows
    /// <paramref name="other"/>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// If <paramref name="other"/> is <c>null</c>, this instance is considered greater.
    /// </remarks>
    public int CompareTo(ColumnInfo? other)
    {
        if (other is null) return 1;
        return string.Compare(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current <see cref="ColumnInfo"/> is equal to another
    /// <see cref="ColumnInfo"/> instance, using the <see cref="ColumnTag"/> property.
    /// </summary>
    /// <param name="other">
    /// The <see cref="ColumnInfo"/> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>true</c> if the two <see cref="ColumnInfo"/> instances represent the same column,
    /// ignoring case; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </remarks>
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
    /// Serves as the default hash function for the <see cref="ColumnInfo"/> class,
    /// using the <see cref="ColumnTag"/> property. 
    /// </summary>
    /// <returns>
    /// An integer hash code for this <see cref="ColumnInfo"/>, computed in a manner
    /// consistent with the case-insensitive comparison used in <see cref="Equals(ColumnInfo?)"/>.
    /// </returns>
    public override int GetHashCode() =>
        ColumnTag.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ColumnInfo"/> instances are equal,
    /// using the <see cref="ColumnTag"/> property.
    /// </summary>
    /// <param name="x">The first <see cref="ColumnInfo"/> to compare.</param>
    /// <param name="y">The second <see cref="ColumnInfo"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="x"/> and <paramref name="y"/> represent the same column;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="ColumnInfo.Equals(ColumnInfo?)"/> for the actual comparison logic.
    /// </remarks>
    public bool Equals(ColumnInfo? x, ColumnInfo? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ColumnInfo"/> instance,
    /// using the <see cref="ColumnTag"/> property.
    /// </summary>
    /// <param name="obj">The <see cref="ColumnInfo"/> for which to compute a hash code.</param>
    /// <returns>
    /// An integer hash code for <paramref name="obj"/>, computed in a manner consistent
    /// with the case-insensitive comparison defined in <see cref="Equals(ColumnInfo?, ColumnInfo?)"/>.
    /// </returns>
    public int GetHashCode(ColumnInfo obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    public static bool operator ==(ColumnInfo? left, ColumnInfo? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="ColumnInfo"/> instances are equal,
    /// using the <see cref="ColumnTag"/> property
    /// </summary>
    /// <param name="left">The first <see cref="ColumnInfo"/> to compare.</param>
    /// <param name="right">The second <see cref="ColumnInfo"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> represent the same column;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and equivalent to calling
    /// <see cref="Equals(ColumnInfo?, ColumnInfo?)"/>.
    /// </remarks>
    public static bool operator !=(ColumnInfo? left, ColumnInfo? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether the <see cref="ColumnTag"/> is empty, thus 
    /// the entire class is empty. This should never happen?
    /// </summary>
    /// <returns>
    /// <c>true</c> if the SQL representation of this column is <c>null</c>, empty,
    /// or consists only of white space; otherwise, <c>false</c>.
    /// </returns>
    public bool IsEmpty() =>
        ToString().IsNullOrWhiteSpace();
}
