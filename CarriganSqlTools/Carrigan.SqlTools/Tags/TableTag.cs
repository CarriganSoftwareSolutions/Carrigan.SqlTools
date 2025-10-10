using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.SqlGenerators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a table identifier (“tag”) in the form <c>[Schema].[Table]</c>.
/// The <c>[Schema]</c> segment is included only if explicitly provided. Implements
/// comparison and equality for use in sorting and hashed collections.
/// </summary>
/// <example>
/// <para>
/// Using Table Attribute
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.SqlGenerators;
/// 
/// [Table("Phone", Schema = "schema")]
/// public class PhoneModel
/// {
///     [Key]
///     public int Id { get; set; }
///     public int CustomerId { get; set; }
///     [Column("Phone")]
///     public string? PhoneNumber { get; set; }
/// }
/// 
/// SqlGenerator<PhoneModel> phoneGenerator = new();
/// 
/// PhoneModel phone = new()
/// {
///     Id = 2718,
///     CustomerId = 3141,
///     PhoneNumber = "07700 900461"
/// };
/// SqlQuery query = phoneGenerator.UpdateById(phone);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [schema].[Phone] 
/// SET [CustomerId] = @CustomerId, [Phone] = @Phone 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Using Identifier Attribute
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.SqlGenerators;
/// 
/// [Identifier("Email", "schema")]
/// public class EmailModel
/// {
///     [PrimaryKey]
///     public int Id { get; set; }
///     public int CustomerId { get; set; }
///     [Identifier("Email")]
///     public string? EmailAddress { get; set; }
/// }
/// 
/// SqlGenerator<EmailModel> emailGenerator = new();
/// 
/// EmailModel email = new()
/// {
///     Id = 10,
///     CustomerId = 313,
///     EmailAddress = "Exterminate@Skaro.gov"
/// };
/// SqlQuery query = emailGenerator.UpdateById(email);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [schema].[Phone] 
/// SET [CustomerId] = @CustomerId, [Phone] = @Phone 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
public class TableTag : IComparable<TableTag>, IEquatable<TableTag>, IEqualityComparer<TableTag>
{
    private readonly string _tableTag;

    //TODO: Redo documentation
    /// <summary>
    /// Initializes a new instance of the <see cref="TableTag"/> class.
    /// </summary>
    /// <param name="schemaName">The optional schema name. If <c>null</c> or empty, only the table name is used.</param>
    /// <param name="tableName">The table name. Must not be <c>null</c> or empty.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="tableName"/> is <c>null</c> or empty.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="tableName"/> or a non-empty <paramref name="schemaName"/> fails SQL identifier validation.
    /// </exception>
    /// 
    //TODO: Make this use schema name and table name.
    internal TableTag(SchemaName? schemaName, TableName tableName) => 
        _tableTag = schemaName.IsNullOrEmpty() ? $"[{tableName}]" : $"[{schemaName}].[{tableName}]";

    //TODO: proof read documentation
    /// <summary>
    /// Initializes a new instance of the <see cref="TableTag"/> class.
    /// </summary>
    /// <remarks>
    /// An external only marked as internal can still be used by the unit tests class.
    /// This method is meant for unit tests only.
    /// </remarks>
    /// <param name="schemaName">The optional schema name. If <c>null</c> or empty, only the table name is used.</param>
    /// <param name="tableName">The table name. Must not be <c>null</c> or empty.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="tableName"/> is <c>null</c> or empty.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="tableName"/> or a non-empty <paramref name="schemaName"/> fails SQL identifier validation.
    /// </exception>
    [ExternalOnly]//An external only marked as internal can still be used by the unit tests class.
    internal TableTag(string? schemaName, string tableName) : this (SchemaName.New(schemaName), new (tableName))
    {
    }

    /// <summary>
    /// Implicitly converts a <see cref="TableTag"/> to its SQL string representation,
    /// e.g., <c>[Schema].[Table]</c> or <c>[Table]</c>.
    /// </summary>
    /// <param name="tableTag">The <see cref="TableTag"/> to convert.</param>
    /// <returns>The SQL-formatted table identifier string.</returns>
    public static implicit operator string(TableTag tableTag) =>
        tableTag._tableTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="TableTag"/> instance.
    /// </summary>
    /// <returns>The SQL-formatted table identifier string.</returns>
    public override string ToString() =>
        _tableTag;

    /// <summary>
    /// Compares this instance to another <see cref="TableTag"/> and returns a value
    /// indicating the sort order.
    /// </summary>
    /// <param name="other">The other <see cref="TableTag"/> to compare.</param>
    /// <returns>
    /// A signed integer indicating relative order: 0 if equal; less than 0 if this instance
    /// precedes <paramref name="other"/>; greater than 0 if it follows.
    /// </returns>
    /// <remarks>Comparison is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
   public int CompareTo(TableTag? other)
    {
        if (other is null) return 1; 
        return string.Compare(_tableTag, other._tableTag, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether this <see cref="TableTag"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The other <see cref="TableTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same identifier; otherwise, <c>false</c>.</returns>
    /// <remarks>Equality is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public bool Equals(TableTag? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(_tableTag, other._tableTag, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="TableTag"/> equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as TableTag);

    /// <summary>
    /// Returns a hash code for this <see cref="TableTag"/> instance.
    /// </summary>
    /// <returns>An integer hash code computed using <see cref="StringComparison.Ordinal"/>.</returns>
    public override int GetHashCode() =>
        _tableTag.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two <see cref="TableTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="TableTag"/> to compare.</param>
    /// <param name="y">The second <see cref="TableTag"/> to compare.</param>
    /// <returns><c>true</c> if both are equal; otherwise, <c>false</c>.</returns>
    /// <remarks>Equality is case-sensitive and uses <see cref="StringComparison.Ordinal"/>.</remarks>
    public bool Equals(TableTag? x, TableTag? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;
        return string.Equals(x._tableTag, y._tableTag, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="TableTag"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="TableTag"/> for which to compute a hash code.</param>
    /// <returns>An integer hash code computed using <see cref="StringComparison.Ordinal"/>.</returns>
    /// <remarks>Equivalent to <see cref="Equals(TableTag?, TableTag?)"/>.</remarks>
    public int GetHashCode(TableTag obj) =>
        obj._tableTag.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two <see cref="TableTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="TableTag"/> to compare.</param>
    /// <param name="right">The second <see cref="TableTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same identifier; otherwise, <c>false</c>.</returns>
    /// <remarks>Equivalent to <see cref="Equals(TableTag?, TableTag?)"/>.</remarks>
    public static bool operator ==(TableTag? left, TableTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="TableTag"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="TableTag"/> to compare.</param>
    /// <param name="right">The second <see cref="TableTag"/> to compare.</param>
    /// <returns><c>true</c> if they differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(TableTag? left, TableTag? right) => !(left == right);

    /// <summary>
    /// Retrieves the <see cref="TableTag"/> associated with the specified entity type by using
    /// the internal SQL tools reflection cache.
    /// </summary>
    /// <param name="value">The entity type for which to retrieve the table tag.</param>
    /// <returns>The <see cref="TableTag"/> for the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the reflection cache does not expose a non-public static <c>Table</c> property,
    /// or when it returns <c>null</c>.
    /// </exception>
    internal static TableTag Get(Type value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Construct the generic type: SqlToolsReflectorCache<value>
        Type cacheType = typeof(SqlToolsReflectorCache<>).MakeGenericType(value);

        // Get the static property 'TableTag' on the constructed type.
        PropertyInfo tableTagProperty = cacheType.GetProperty("Table", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException($"The property 'TableTag' was not found on type '{cacheType.FullName}'.");

        // Retrieve the value of the TableTag property.
        return (TableTag?)tableTagProperty.GetValue(null) ?? throw new InvalidOperationException($"The property 'TableTag' on type '{cacheType.FullName}' returned null.");
    }
}

