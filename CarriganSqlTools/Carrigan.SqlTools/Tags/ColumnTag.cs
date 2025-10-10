using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

//TODO: Rework all documentation, examples

/// <summary>
/// Represents a column identifier, or “tag,” in the form <c>[Schema].[Table].[Column]</c>.
/// The <c>[Schema]</c> segment is included only if the table’s schema is explicitly defined.
/// Aside from implementing various comparison and equality interfaces,
/// this class is intended for internal use only.
/// </summary>
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
internal class ColumnTag : IComparable<ColumnTag>, IEquatable<ColumnTag>, IEqualityComparer<ColumnTag>
{
    /// <summary>
    /// A string that represent the <see cref="ColumnTag"/> .
    /// </summary>
    private readonly string _columnTag;

    private readonly ColumnName _columnName;
    internal readonly TableTag TableTag;

    //TODO: redo documentation
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnTag"/> class,
    /// which represents a fully qualified SQL column identifier
    /// in the form <c>[Schema].[Table].[Column]</c>.
    /// </summary>
    /// <param name="tableTag">
    /// The <see cref="TableTag"/> that identifies the table containing the column.
    /// </param>
    /// <param name="columnName">
    /// The name of the column.
    /// </param>
    internal ColumnTag(TableTag tableTag, ColumnName columnName)
    {
        _columnTag = tableTag.ToString().IsNullOrEmpty() ? $"[{columnName}]" : $"{tableTag}.[{columnName}]";
        _columnName = columnName;
        TableTag = tableTag;
    }

    /// <summary>
    /// Implicitly converts a <see cref="ColumnTag"/> to its SQL string representation
    /// in the form <c>[Schema].[Table].[Column]</c>.
    /// </summary>
    /// <param name="value">The <see cref="ColumnTag"/> to convert.</param>
    /// <returns>
    /// A SQL string that fully qualifies the column name, including schema and table if defined.
    /// </returns>
    public static implicit operator string(ColumnTag value)
        => value._columnTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ColumnTag"/> instance,
    /// equivalent to the result of the implicit conversion to <see cref="string"/>.
    /// </summary>
    /// <returns>
    public override string ToString()
        => this;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ColumnTag"/>,
    /// optionally including the table (and schema) prefix.
    /// </summary>
    /// <param name="useTableTag">
    /// <c>true</c> to include the full table and schema prefix (e.g., <c>[Schema].[Table].[Column]</c>);
    /// <c>false</c> to return only the column name (e.g., <c>[Column]</c>).
    /// </param>
    /// <returns>
    /// A SQL string representing the column, formatted according to <paramref name="useTableTag"/>.
    /// </returns>
    public string ToString(bool useTableTag) 
    {
        if (useTableTag)
            return ToString();
        else
            return $"[{_columnName}]";
    }

    /// <summary>
    /// Compares this <see cref="ColumnTag"/> to another instance and returns a value
    /// that indicates their relative sort order.
    /// </summary>
    /// <param name="other">
    /// The <see cref="ColumnTag"/> to compare with the current instance.
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
    public int CompareTo(ColumnTag? other)
    {
        if (other is null) return 1;
        return string.Compare(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current <see cref="ColumnTag"/> is equal to another
    /// <see cref="ColumnTag"/> instance.
    /// </summary>
    /// <param name="other">
    /// The <see cref="ColumnTag"/> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>true</c> if the two <see cref="ColumnTag"/> instances represent the same column,
    /// ignoring case; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </remarks>
    public bool Equals(ColumnTag? other)
    {
        if (other is null) return false;
        return string.Equals(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// <see cref="ColumnTag"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is a <see cref="ColumnTag"/> and
    /// represents the same column (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and delegates to
    /// <see cref="Equals(ColumnTag?)"/>.
    /// </remarks>
    public override bool Equals(object? obj) =>
        obj is ColumnTag ct && Equals(ct);

    /// <summary>
    /// Serves as the default hash function for the <see cref="ColumnTag"/> class.
    /// </summary>
    /// <returns>
    /// An integer hash code for this <see cref="ColumnTag"/>, computed in a manner
    /// consistent with the case-insensitive comparison used in <see cref="Equals(ColumnTag?)"/>.
    /// </returns>
    public override int GetHashCode() =>
        _columnTag.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="ColumnTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="ColumnTag"/> to compare.</param>
    /// <param name="y">The second <see cref="ColumnTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="x"/> and <paramref name="y"/> represent the same column;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="ColumnTag.Equals(ColumnTag?)"/> for the actual comparison logic.
    /// </remarks>
    public bool Equals(ColumnTag? x, ColumnTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="ColumnTag"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="ColumnTag"/> for which to compute a hash code.</param>
    /// <returns>
    /// An integer hash code for <paramref name="obj"/>, computed in a manner consistent
    /// with the case-insensitive comparison defined in <see cref="Equals(ColumnTag?, ColumnTag?)"/>.
    /// </returns>
    public int GetHashCode(ColumnTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    public static bool operator ==(ColumnTag? left, ColumnTag? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="ColumnTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="ColumnTag"/> to compare.</param>
    /// <param name="right">The second <see cref="ColumnTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> represent the same column;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and equivalent to calling
    /// <see cref="Equals(ColumnTag?, ColumnTag?)"/>.
    /// </remarks>
    public static bool operator !=(ColumnTag? left, ColumnTag? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether this <see cref="ColumnTag"/> is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the SQL representation of this column is <c>null</c>, empty,
    /// or consists only of white space; otherwise, <c>false</c>.
    /// </returns>
    public bool IsEmpty() =>
        ToString().IsNullOrWhiteSpace();
}
