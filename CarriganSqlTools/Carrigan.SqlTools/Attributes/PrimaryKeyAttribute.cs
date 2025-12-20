using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Defines primary key metadata for the decorated property.
/// </summary>
/// <remarks>
/// <para>
/// In Carrigan.SqlTools, a property represents an SQL column in the data model. When this attribute is
/// applied, it marks that column as part of the primary key used by the SQL generator when constructing
/// SQL statements such as <c>UPDATE</c>, <c>DELETE</c>, and “By Id” <c>SELECT</c> operations.
/// </para>
/// <para>
/// Composite primary keys are supported by applying <see cref="PrimaryKeyAttribute"/> to multiple properties.
/// </para>
/// <para>
/// The SQL generator also recognizes properties marked with <see cref="KeyAttribute"/> from
/// <see cref="System.ComponentModel.DataAnnotations"/>. However, if any property is annotated with
/// <see cref="PrimaryKeyAttribute"/>, that marking takes precedence and <see cref="KeyAttribute"/> is ignored
/// for SQL generation purposes.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity
/// Framework Core or any other ORM behavior.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Attributes;
/// using Carrigan.SqlTools.SqlGenerators;
/// using Carrigan.SqlTools.SqlQueries;
///
/// internal class EmailModel
/// {
///     [PrimaryKey]
///     public int Id { get; set; }
///
///     public int CustomerId { get; set; }
///
///     public string? EmailAddress { get; set; }
/// }
///
/// SqlGenerator<EmailModel> emailGenerator = new();
/// EmailModel email = new() { Id = 10, CustomerId = 313, EmailAddress = "test@example.com" };
///
/// SqlQuery query = emailGenerator.UpdateById(email);
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PrimaryKeyAttribute : Attribute
{
}
