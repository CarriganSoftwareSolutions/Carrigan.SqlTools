using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.SqlGenerators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies SQL identifiers to use during SQL generation for an entity type or property,
/// including the logical <see cref="Name"/> (table or column) and optional <see cref="Schema"/>.
/// </summary>
/// <remarks>
/// <para><b>Resolution behavior</b></para>
/// <list type="bullet">
///   <item>
///     <description>If <see cref="IdentifierAttribute"/> is present, the SQL generator uses its values.</description>
///   </item>
///   <item>
///     <description>If absent, the generator falls back to <see cref="TableAttribute"/> (type) and
///     <see cref="ColumnAttribute"/> (property).</description>
///   </item>
///   <item>
///     <description>If those are also absent, the CLR type or property name is used.</description>
///   </item>
///   <item>
///     <description>If <see cref="Schema"/> is not specified, no schema prefix is applied.</description>
///   </item>
/// </list>
/// <para><b>Notes</b></para>
/// <list type="bullet">
///   <item>
///     <description>This attribute does <b>not</b> override Entity Framework mappings; it only affects
///     SQL emitted by this library’s generators (e.g., <see cref="SqlGenerator{T}"/>).</description>
///   </item>
///   <item>
///     <description>If you already use <see cref="TableAttribute"/> / <see cref="ColumnAttribute"/>,
///     the generator honors them. Use <see cref="IdentifierAttribute"/> to align with fluent mappings
///     or to provide identifiers when EF isn’t used.</description>
///   </item>
/// </list>
/// <para><b>Important</b></para>
/// This library does not create/alter database objects. It generates SQL for SELECT/INSERT/UPDATE/DELETE
/// and stored procedure execution only.
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
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IdentifierAttribute : Attribute
{
    /// <summary>
    /// Gets the logical identifier name (table or column).
    /// </summary>
    internal string Name { get; }
    /// <summary>
    /// Gets the schema name, if specified; otherwise <c>null</c>.
    /// </summary>
    internal string? Schema { get; }

    /// <summary>
    /// Gets the fully qualified CLR member name (type or property) this attribute is attached to.
    /// Set during <see cref="Initialize(MemberInfo)"/>.
    /// </summary>
    internal MemberName MemberName { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifierAttribute"/> class.
    /// </summary>
    /// <remarks>
    /// <see cref="MemberName"/> cannot be set in the constructor; it is populated later by
    /// <see cref="Initialize(MemberInfo)"/>, which requires the constructed instance and reflection context.
    /// </remarks>
    /// <param name="Name">The SQL table or column identifier to use.</param>
    /// <param name="Schema">The optional SQL schema name.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="Name"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="Name"/> is an empty string, or when a non-<c>null</c>
    /// <paramref name="Schema"/> is an empty string.
    /// </exception>
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


    /// <summary>
    /// Populates <see cref="MemberName"/> after construction using reflection metadata.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This cannot be performed inside the constructor because certain reflection data (e.g.,
    /// the fully qualified member name from <c>MemberInfoExtensions.GetQualifiedName</c>) is only
    /// reliably available after construction.
    /// </para>
    /// </remarks>
    public void Initialize(MemberInfo member) => 
        MemberName = new(member.GetQualifiedName());
}
