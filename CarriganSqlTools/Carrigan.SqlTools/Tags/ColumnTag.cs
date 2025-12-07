using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a fully qualified SQL column identifier(or “tag”) in the form<c>[Schema].[Table].[Column]</c>.
/// The<c>[Schema]</c> segment is included only if the schema is explicitly defined for the table.
/// Implements comparison and equality for use in sorting and hashed collections.
/// </summary>
/// <remarks>
/// This type is produced from reflection metadata (e.g., a data model’s property) and provides the SQL-safe
/// identifier used when constructing SQL queries. It is commonly combined with <see cref="Tags.TableTag"/>
/// and <see cref="IdentifierTypes.ColumnName"/>.
/// </remarks>
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
///     EmailAddress = "Exterminate@GenericTinCanLand.gov"
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
/// <example>
/// <para>
/// Using Column Attribute
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
internal class ColumnTag : IComparable<ColumnTag>, IEquatable<ColumnTag>, IEqualityComparer<ColumnTag>
{
    /// <summary>
    /// The SQL string representation of the column tag.
    /// </summary>
    private readonly string _columnTag;

    /// <summary>
    /// The <see cref="IdentifierTypes.ColumnName"/> representing the column’s name.
    /// </summary>
    internal readonly ColumnName ColumnName;
    /// <summary>
    /// The <see cref="Tags.TableTag"/> representing the table containing the column.
    /// </summary>
    internal readonly TableTag TableTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnTag"/> class that represents a fully
    /// qualified SQL column identifier in the form <c>[Schema].[Table].[Column]</c>.
    /// </summary>
    /// <param name="tableTag">The <see cref="IdentifierTypes.TableTag"/> representing the table containing the column.</param>
    /// <param name="columnName">The <see cref="Tags.ColumnName"/> representing the column’s name.</param>
    internal ColumnTag(TableTag tableTag, ColumnName columnName)
    {
        _columnTag = tableTag.ToString().IsNullOrEmpty() ? $"[{columnName}]" : $"{tableTag}.[{columnName}]";
        ColumnName = columnName;
        TableTag = tableTag;
    }

    /// <summary>
    /// Implicitly converts a <see cref="ColumnTag"/> to its SQL string representation,
    /// e.g., <c>[Schema].[Table].[Column]</c>.
    /// </summary>
    /// <param name="value">The <see cref="ColumnTag"/> to convert.</param>
    /// <returns>The fully qualified SQL column identifier string.</returns>
    public static implicit operator string(ColumnTag value)
        => value._columnTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ColumnTag"/> instance,
    /// equivalent to the implicit conversion to <see cref="string"/>.
    /// </summary>
    /// <returns>The fully qualified SQL column identifier string.</returns>
    public override string ToString()
        => this;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ColumnTag"/>,
    /// optionally including the table (and schema) prefix.
    /// </summary>
    /// <param name="useTableTag">
    /// <c>true</c> to include <c>[Schema].[Table].[Column]</c>;
    /// <c>false</c> to return only <c>[Column]</c>.
    /// </param>
    /// <returns>A SQL string representing the column identifier.</returns>
    public string ToString(bool useTableTag) 
    {
        if (useTableTag)
            return ToString();
        else
            return $"[{ColumnName}]";
    }

    /// <summary>
    /// Compares this <see cref="ColumnTag"/> to another instance and returns a value
    /// that indicates their relative sort order.
    /// </summary>
    /// <param name="other">The <see cref="ColumnTag"/> to compare with this instance.</param>
    /// <returns>
    /// A signed integer that indicates the relative order:
    /// <c>0</c> if equal;
    /// less than 0 if this instance precedes <paramref name="other"/>;
    /// greater than 0 if it follows.
    /// </returns>
    /// <remarks>Comparison is case-insensitive using <see cref="StringComparison.OrdinalIgnoreCase"/>.</remarks>
    public int CompareTo(ColumnTag? other)
    {
        if (other is null) return 1;
        return string.Compare(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current <see cref="ColumnTag"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The other <see cref="ColumnTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if both represent the same column (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(ColumnTag? other)
    {
        if (other is null) return false;
        return string.Equals(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to this <see cref="ColumnTag"/>.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is a <see cref="ColumnTag"/> representing the same column;
    /// otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is ColumnTag ct && Equals(ct);

    /// <summary>
    /// Returns a hash code for this <see cref="ColumnTag"/> consistent with
    /// case-insensitive equality semantics.
    /// </summary>
    /// <returns>An integer hash code based on <see cref="_columnTag"/> (ordinal ignore case).</returns>
    public override int GetHashCode() =>
        _columnTag.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ColumnTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="ColumnTag"/> to compare.</param>
    /// <param name="y">The second <see cref="ColumnTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same column (case-insensitive); otherwise, <c>false</c>.</returns>
    public bool Equals(ColumnTag? x, ColumnTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ColumnTag"/> instance,
    /// consistent with case-insensitive equality semantics.
    /// </summary>
    /// <param name="obj">The <see cref="ColumnTag"/> for which to compute a hash code.</param>
    /// <returns>An integer hash code for <paramref name="obj"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c>.</exception>
    public int GetHashCode(ColumnTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ColumnTag"/> instances represent the same column.
    /// </summary>
    /// <param name="left">The first <see cref="ColumnTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ColumnTag"/> to compare.</param>
    /// <returns><c>true</c> if both represent the same column (case-insensitive); otherwise, <c>false</c>.</returns>
    public static bool operator ==(ColumnTag? left, ColumnTag? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="ColumnTag"/> instances represent different columns.
    /// </summary>
    /// <param name="left">The first <see cref="ColumnTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ColumnTag"/> to compare.</param>
    /// <returns><c>true</c> if they differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(ColumnTag? left, ColumnTag? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether this <see cref="ColumnTag"/> is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the SQL representation of this column is <c>null</c>, empty, or whitespace;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool IsEmpty() =>
        ToString().IsNullOrWhiteSpace();
}
