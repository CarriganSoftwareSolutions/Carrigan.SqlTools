using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using System.Numerics;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a fully qualified SQL column identifier (a “tag”) rendered as
/// <c>[Schema].[Table].[Column]</c> or <c>[Table].[Column]</c>.
/// </summary>
/// <remarks>
/// Equality and hashing are based on the schema, table-name, and column-name components using
/// case-insensitive identifier semantics. An empty schema is treated the same as no schema.
/// </remarks>
/// <example>
/// <para>
/// Using Identifier Attribute
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Attributes;
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
/// --PostgreSql
/// UPDATE "schema"."Email" 
/// SET "CustomerId" = $1, 
///     "Email" = $2 
/// WHERE "Id" = $3;
/// 
/// --SqlServer
/// UPDATE [schema].[Email] 
/// SET [CustomerId] = @CustomerId_1, 
///     [Email] = @Email_2 
/// WHERE [Id] = @Id_3;
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Using Column Attribute
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.SqlGenerators;
/// using System.ComponentModel.DataAnnotations;
/// using System.ComponentModel.DataAnnotations.Schema;
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
/// --PostgreSql
/// UPDATE "schema"."Phone"
/// SET "CustomerId" = $1, 
///     "Phone" = $2 
/// WHERE "Id" = $3;
/// 
/// --SqlServer
/// "UPDATE [schema].[Phone] 
/// SET [CustomerId] = @CustomerId_1, 
///     [Phone] = @Phone_2
/// WHERE [Id] = @Id_3;
/// ]]></code>
/// </example>
internal class ColumnTag : IEquatable<ColumnTag>, IEqualityOperators<ColumnTag, ColumnTag, bool>, ISqlFragment, IWhiteSpace, IEmpty
{
    /// <summary>
    /// The <see cref="IdentifierTypes.ColumnName"/> representing the column’s name.
    /// </summary>
    internal readonly ColumnName ColumnName;

    /// <summary>
    /// The <see cref="Tags.TableTag"/> representing the table containing the column.
    /// </summary>
    internal readonly TableTag TableTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnTag"/> class.
    /// </summary>
    /// <param name="tableTag">The <see cref="Tags.TableTag"/> representing the table containing the column.</param>
    /// <param name="columnName">The <see cref="IdentifierTypes.ColumnName"/> representing the column’s name.</param>
    internal ColumnTag(TableTag tableTag, ColumnName columnName)
    {
        ColumnName = columnName;
        TableTag = tableTag;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnTag"/> class for an unqualified column name.
    /// </summary>
    /// <param name="columnName">The <see cref="IdentifierTypes.ColumnName"/> representing the column’s name.</param>
    internal ColumnTag(ColumnName columnName)
    {
        ColumnName = columnName;
        TableTag = new(SchemaName.New(null), new TableName(null));
    }

    /// <summary>
    /// Returns the qualified column name represented by this tag.
    /// </summary>
    public override string ToString() =>
        TableTag.IsNotEmpty() ? $"{TableTag}.{ColumnName}" : ColumnName.ToString();

    /// <summary>
    /// Determines whether this instance identifies the same column as another <see cref="ColumnTag"/>.
    /// </summary>
    public bool Equals(ColumnTag? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        if (TableTag.SchemaName.IsNotNullOrEmpty() != other.TableTag.SchemaName.IsNotNullOrEmpty())
            return false;

        if (TableTag.SchemaName.IsNotNullOrEmpty() &&
            string.Equals(TableTag.SchemaName!.ToString(), other.TableTag.SchemaName!.ToString(), StringComparison.OrdinalIgnoreCase) == false)
            return false;

        return string.Equals(TableTag.TableName.ToString(), other.TableTag.TableName.ToString(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(ColumnName.ToString(), other.ColumnName.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object identifies the same column as this instance.
    /// </summary>
    public override bool Equals(object? obj) =>
        Equals(obj as ColumnTag);

    /// <summary>
    /// Returns a hash code for this column tag.
    /// </summary>
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(TableTag.SchemaName.IsNotNullOrEmpty() ? TableTag.SchemaName!.ToString() : string.Empty, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(TableTag.TableName.ToString(), StringComparer.OrdinalIgnoreCase);
        hashCode.Add(ColumnName.ToString(), StringComparer.OrdinalIgnoreCase);
        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Determines whether two column tags identify the same column.
    /// </summary>
    public static bool operator ==(ColumnTag? left, ColumnTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two column tags identify different columns.
    /// </summary>
    public static bool operator !=(ColumnTag? left, ColumnTag? right) =>
        !(left == right);

    /// <summary>
    /// Indicates whether the column name is empty or consists only of whitespace characters.
    /// </summary>
    public bool IsWhiteSpace() =>
        ColumnName.IsWhiteSpace();

    /// <summary>
    /// Indicates whether the column name contains at least one non-whitespace character.
    /// </summary>
    public bool IsNotWhiteSpace() =>
        IsWhiteSpace() == false;

    /// <summary>
    /// Indicates whether the column name is empty.
    /// </summary>
    public bool IsEmpty() =>
        ColumnName.IsEmpty();

    /// <summary>
    /// Indicates whether the column name is not empty.
    /// </summary>
    public bool IsNotEmpty() =>
        IsEmpty() == false;


    /// <summary>
    /// Initializes a new instance of the <see cref="Flatten"/> class.
    /// </summary>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {
        yield return this;
    }

    /// <summary>
    /// Returns an empty enumeration, as <see cref="ColumnTag"/> does not contain any parameters.
    /// </summary>
    /// <returns>an empty enumeration </returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        [];

    /// <summary>
    /// Renders the column tag as a SQL identifier, including the table tag if specified.
    /// </summary>
    /// <param name="dialect">a sql dialect</param>
    /// <returns>the rendered SQL identifier</returns>
    public string ToSql(ISqlDialects dialect) =>
        ToSql(dialect, true);

    /// <summary>
    /// Renders the column tag as a SQL identifier, optionally including the table tag.
    /// </summary>
    /// <param name="dialect">a sql dialect</param>
    /// <param name="useTableTag">a value indicating whether to include the table tag</param>
    /// <returns>the rendered SQL identifier</returns>
    public string ToSql(ISqlDialects dialect, bool useTableTag) =>
        dialect.RenderColumn(TableTag, ColumnName, useTableTag);
}
