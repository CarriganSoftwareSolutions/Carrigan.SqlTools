using System.ComponentModel.DataAnnotations;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies which column or columns should be used when generating a query based on an Id property.
/// The SQL generator respects properties marked with <see cref="KeyAttribute"/>; however, if any
/// property is marked with the SQL generator's own <see cref="PrimaryKeyAttribute"/>, that
/// designation takes precedence and overrides all <see cref="KeyAttribute"/> markings for SQL
/// generation purposes.
///
/// This does not affect Entity Framework’s use of <see cref="KeyAttribute"/> and does not
/// override its behavior at runtime.
/// </summary>
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
