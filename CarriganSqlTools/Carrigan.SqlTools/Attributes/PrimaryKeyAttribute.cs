using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Identifies a property as the primary key used by the SQL generator when constructing
/// SQL statements such as <c>UPDATE</c>, <c>DELETE</c>, or <c>SELECT ... WHERE [Id] = @Id</c>.
/// </summary>
/// <remarks>
/// <para>
/// The SQL generator recognizes properties marked with <see cref="KeyAttribute"/> from
/// <see cref="System.ComponentModel.DataAnnotations"/>, but if any property is annotated
/// with <see cref="PrimaryKeyAttribute"/>, that marking takes precedence and overrides all
/// <see cref="KeyAttribute"/> definitions for SQL generation purposes.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not
/// influence Entity Framework Core or any other ORM behavior.
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
/// SqlQuery query = emailGenerator.UpdateById(email);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [schema].[Email] 
/// SET [CustomerId] = @CustomerId, [Email] = @Email 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public  class PrimaryKeyAttribute : Attribute
{
}
