using Carrigan.SqlTools.SqlGenerators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies SQL identifier metadata used by this library’s SQL generators for a data model type or property.
/// </summary>
/// <remarks>
/// In Carrigan.SqlTools, a <b>class</b> represents an SQL <b>table</b> (or stored procedure),
/// and a <b>property</b> represents an SQL <b>column</b> (or stored procedure parameter).
/// When applied:
/// <list type="bullet">
///   <item>
///     <description>
///     On a class: provides the identifier metadata used as the SQL table (or procedure) name, and optional schema.
///     </description>
///   </item>
///   <item>
///     <description>
///     On a property: provides the identifier metadata used as the SQL column (or parameter) name.
///     </description>
///   </item>
/// </list>
/// <para><b>Resolution behavior</b></para>
/// <list type="bullet">
///   <item>
///     <description>
///     If <see cref="IdentifierAttribute"/> is present, the SQL generator uses its explicit identifier values.
///     </description>
///   </item>
///   <item>
///     <description>
///     If absent, the generator falls back to
///     <see cref="TableAttribute"/> (for types) and
///     <see cref="ColumnAttribute"/> (for properties), if present.
///     </description>
///   </item>
///   <item>
///     <description>
///     If neither <see cref="IdentifierAttribute"/> nor EF mapping attributes are present,
///     the CLR type or property name is used as the SQL identifier.
///     </description>
///   </item>
///   <item>
///     <description>
///     If <see cref="Schema"/> is <c>null</c>, no schema prefix is emitted in the generated SQL.
///     </description>
///   </item>
/// </list>
/// <para><b>Notes</b></para>
/// <list type="bullet">
///   <item>
///     <description>
///     This attribute does <b>not</b> modify or override Entity Framework mappings. It affects only SQL
///     emitted by this library’s generators (for example,
///     <see cref="Carrigan.SqlTools.SqlGenerators.SqlGenerator{T}"/>).
///     </description>
///   </item>
/// </list>
/// <para><b>Important</b></para>
/// This library does not create, alter, or migrate database objects. It generates SQL text for SELECT, INSERT,
/// UPDATE, DELETE, and stored procedure execution only.
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
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
public sealed class IdentifierAttribute : Attribute
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
    /// Initializes a new instance of the <see cref="IdentifierAttribute"/> class.
    /// </summary>
    /// <param name="Name">The SQL table or column identifier to use.</param>
    /// <param name="Schema">The optional SQL schema name.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="Name"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="Name"/> is an empty string, or when a non-<c>null</c>
    /// <paramref name="Schema"/> is an empty string.
    /// </exception>
    public IdentifierAttribute(string Name, string? Schema = null)
    {
        ArgumentNullException.ThrowIfNull(Name, nameof(Name));
        if (Name == string.Empty)
            throw new ArgumentException("Name cannot be an empty string", nameof(Name));
        if (Schema is not null && Schema == string.Empty)
            throw new ArgumentException("Schema cannot be an empty string", nameof(Schema));
        this.Name = Name;
        this.Schema = Schema;
    }
}
