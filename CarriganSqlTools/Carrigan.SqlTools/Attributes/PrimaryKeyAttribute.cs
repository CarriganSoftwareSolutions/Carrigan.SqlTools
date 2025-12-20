using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Identifies a property as the primary key used by this library’s SQL generators.
/// </summary>
/// <remarks>
/// In Carrigan.SqlTools, a <b>property</b> represents an SQL <b>column</b>. When a property
/// is marked with <see cref="PrimaryKeyAttribute"/>, it is treated as the primary key column
/// for SQL generation scenarios such as <c>UPDATE</c>, <c>DELETE</c>, and
/// <c>SELECT ... WHERE</c> clauses.
/// <para>
/// The SQL generator also recognizes <see cref="KeyAttribute"/> from
/// <see cref="System.ComponentModel.DataAnnotations"/>. However, if one or more properties
/// are annotated with <see cref="PrimaryKeyAttribute"/>, those markings take precedence and
/// override any <see cref="KeyAttribute"/> definitions for SQL generation purposes.
/// </para>
/// <para>
/// This attribute affects only SQL text generation performed by Carrigan.SqlTools. It does
/// not influence Entity Framework Core behavior, database schema definitions, or ORM metadata.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// [Identifier("Email", "schema")]
/// internal class EmailModel
/// {
///     [PrimaryKey]
///     public int Id { get; set; }
///     public int CustomerId { get; set; }
///     [Identifier("Email")]
///     public string? EmailAddress { get; set; }
/// }
///
/// SqlGenerator<EmailModel> emailGenerator = new();
/// EmailModel email = new()
/// {
///     Id = 10,
///     CustomerId = 313,
///     EmailAddress = "Exterminate@GenericTinCanLand.gov"
/// };
///
/// SqlQuery query = emailGenerator.UpdateById(email);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [schema].[Email]
/// SET [CustomerId] = @CustomerId, [Email] = @Email
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public  sealed class PrimaryKeyAttribute : Attribute
{
}
