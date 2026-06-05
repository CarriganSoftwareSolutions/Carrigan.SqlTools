using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
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
internal class ColumnTag : StringWrapper, ISqlFragment
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
        : base($"{tableTag}.{columnName}", StringComparison.OrdinalIgnoreCase)
    {
        ColumnName = columnName;
        TableTag = tableTag;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnTag"/> class for an unqualified column name.
    /// </summary>
    /// <param name="columnName">The <see cref="IdentifierTypes.ColumnName"/> representing the column’s name.</param>
    internal ColumnTag(ColumnName columnName)
        : base(columnName, StringComparison.OrdinalIgnoreCase)
    {
        ColumnName = columnName;
        TableTag = new(SchemaName.New(null), new TableName(null));
    }

    /// <summary>
    /// Determines whether this <see cref="ColumnTag"/> represents an empty or whitespace column name.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the underlying <see cref="ColumnName"/> is empty or whitespace; otherwise, <c>false</c>.
    /// </returns>
    public new bool IsEmpty() =>
        ColumnName.IsNullOrWhiteSpace();


    /// <summary>
    /// Initializes a new instance of the <see cref="Flatten"/> class.
    /// </summary>
    public IEnumerable<ISqlFragment> Flatten()
    {
        yield return this;
    }

    /// <summary>
    /// Returns an empty enumeration, as <see cref="ColumnTag"/> does not contain any parameters.
    /// </summary>
    /// <returns>an empty enumeration </returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
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
