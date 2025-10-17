using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.SqlGenerators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Attribute for classes and properties that specifies the identifiers to use in generated SQL,
/// including table <see cref="Name"/>, column <see cref="Name"/>, and <see cref="Schema"/> names.
/// 
/// Behavior:
/// - If <see cref="IdentifierAttribute"/> is not defined, the SQL generator falls back to
///   <see cref="TableAttribute"/> and <see cref="ColumnAttribute"/> annotations.
/// - If those are also absent, it uses the class or property name.
/// - If no schema is defined, no schema is applied.
/// 
/// Notes:
/// - This attribute does **not** override <see cref="TableAttribute"/> or <see cref="ColumnAttribute"/> 
///   within Entity Framework. However, it takes precedence for SQL generation performed by the SQL generator.
/// - If you are already using <see cref="TableAttribute"/> and <see cref="ColumnAttribute"/> with
///   Entity Framework, it is best to continue using those—SQL generation will honor them.
/// - If you configure tables, columns, and schemas with fluent mappings, use <see cref="IdentifierAttribute"/> 
///   to ensure consistency with your fluent configuration.
/// - If you are not using Entity Framework, you can still use <see cref="TableAttribute"/> and
///   <see cref="ColumnAttribute"/> if preferred; the SQL generator will respect them.
/// 
/// Important:
/// The SQL generator does not create, modify, or delete database objects such as tables,
/// columns, or stored procedures. It only generates SQL for SELECT, DELETE, INSERT, UPDATE,
/// and stored procedure execution.
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
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IdentifierAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the table/column name.
    /// </summary>
    internal string Name { get; }
    /// <summary>
    /// Public getter to indicate the schema name.
    /// </summary>
    internal string? Schema { get; }

    internal MemberName MemberName { get; private set; }

    //TODO: documentation
    /// <summary>
    /// Public constructor
    /// </summary>
    /// <remarks>
    /// MemberName is set in the Initialize method.
    /// </remarks>
    /// <param name="Name">Sql Table/Column Identifier name</param>
    /// <param name="Schema">Sql Schema name</param>
    /// <exception cref="InvalidSqlIdentifierException">If <see cref="Name"/> or <see cref="Name"/> have an invalid Sql Identifier</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IdentifierAttribute(string Name, string? Schema = null)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        ArgumentNullException.ThrowIfNull(Name, nameof(Name));
        if (Name == string.Empty)
            throw new ArgumentException("name is an empty string", nameof(Name));
        if (Schema is not null && Schema == string.Empty)
            throw new ArgumentException("schema is an empty string", nameof(Schema));
        this.Name = Name;
        this.Schema = Schema;
    }


    //TODO: documentation
    public void Initialize(MemberInfo member) => 
        MemberName = new(member.GetQualifiedName());
}
