using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a fully qualified SQL column identifier (a “tag”) rendered as
/// <c>[Schema].[Table].[Column]</c> or <c>[Table].[Column]</c>.
/// </summary>
/// <remarks>
/// This type uses <see cref="StringWrapper"/> to provide consistent equality, ordering,
/// and hashing semantics (case-insensitive via <see cref="StringComparison.OrdinalIgnoreCase"/>).
/// <para>
/// Note: Inherited equality and ordering operations can throw <see cref="InvalidOperationException"/>
/// if this instance is compared against a different <see cref="StringWrapper"/> that uses a different
/// <see cref="StringComparison"/> mode.
/// </para>
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
/// UPDATE [schema].[Email]
/// SET [CustomerId] = @CustomerId, [Email] = @Email
/// WHERE [Id] = @Id;
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
/// UPDATE [schema].[Phone]
/// SET [CustomerId] = @CustomerId, [Phone] = @Phone
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
internal class ColumnTag : StringWrapper
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
        : base(CreateColumnTagString(tableTag, columnName), StringComparison.OrdinalIgnoreCase)
    {
        ColumnName = columnName;
        TableTag = tableTag;
    }

    /// <summary>
    /// Returns the SQL string representation of this <see cref="ColumnTag"/>,
    /// optionally including the table (and schema) prefix.
    /// </summary>
    /// <param name="useTableTag">
    /// <c>true</c> to include <c>[Schema].[Table].[Column]</c> or <c>[Table].[Column]</c>;
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
    /// Determines whether this <see cref="ColumnTag"/> represents an empty or whitespace column name.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the underlying <see cref="ColumnName"/> is empty or whitespace; otherwise, <c>false</c>.
    /// </returns>
    public new bool IsEmpty() =>
        ColumnName.IsNullOrWhiteSpace();

    private static string CreateColumnTagString(TableTag tableTag, ColumnName columnName) =>
        tableTag.ToString().IsNullOrEmpty() ? $"[{columnName}]" : $"{tableTag}.[{columnName}]";
}
