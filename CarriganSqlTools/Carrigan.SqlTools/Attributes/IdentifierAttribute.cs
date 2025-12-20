using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies SQL identifier metadata used by this library’s SQL generators for a data model type or property.
/// </summary>
/// <remarks>
/// In Carrigan.SqlTools, a <b>class</b> represents an SQL <b>table</b> (or stored procedure),
/// and a <b>property</b> represents an SQL <b>column</b> (or stored procedure parameter).
/// <para>
/// When applied:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///     On a class: provides the identifier metadata used as the SQL table name (for CRUD generation) and
///     the stored procedure name (for procedure execution), plus an optional schema.
///     </description>
///   </item>
///   <item>
///     <description>
///     On a property: provides the identifier metadata used as the SQL column name. If no
///     <see cref="ParameterAttribute"/> is present, the resolved column name is also used as the default
///     SQL parameter name.
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
///     The <see cref="Schema"/> value is used only when this attribute is applied to a class.
///     When applied to a property, <see cref="Schema"/> is ignored by the current reflection cache.
///     </description>
///   </item>
/// </list>
/// <para><b>Notes</b></para>
/// <list type="bullet">
///   <item>
///     <description>
///     This attribute does <b>not</b> modify or override Entity Framework mappings. It affects only SQL
///     emitted by this library’s generators.
///     </description>
///   </item>
///   <item>
///     <description>
///     This attribute enforces only null and empty-string validation. SQL identifier rules
///     (e.g., whitespace rules, invalid characters, reserved words, and length constraints)
///     are validated by the SQL generator.
///     </description>
///   </item>
/// </list>
/// <para><b>Important</b></para>
/// This library does not create, alter, or migrate database objects. It generates SQL text for SELECT, INSERT,
/// UPDATE, DELETE, and stored procedure execution only.
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Attributes;
/// using Carrigan.SqlTools.SqlGenerators;
/// using Carrigan.SqlTools.SqlQueries;
///
/// [Identifier("Email", "schema")]
/// internal class EmailModel
/// {
///     [PrimaryKey]
///     public int Id { get; set; }
///
///     public int CustomerId { get; set; }
///
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
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class IdentifierAttribute : Attribute
{
    /// <summary>
    /// Gets the identifier name applied to the decorated type or property.
    /// </summary>
    internal string Name { get; }

    /// <summary>
    /// Gets the schema name, if specified; otherwise <c>null</c>.
    /// </summary>
    /// <remarks>
    /// This value is used only when the attribute is applied to a class.
    /// </remarks>
    internal string? Schema { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifierAttribute"/> class.
    /// </summary>
    /// <param name="Name">The SQL identifier to use for the decorated type or property.</param>
    /// <param name="Schema">The optional SQL schema name (type-level only).</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="Name"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="Name"/> is an empty string, or when a non-<c>null</c>
    /// <paramref name="Schema"/> is an empty string.
    /// </exception>
    public IdentifierAttribute(string Name, string? Schema = null)
    {
        ArgumentNullException.ThrowIfNull(Name, nameof(Name));
        if (Name == string.Empty)
            throw new ArgumentException("name is an empty string.", nameof(Name));

        if (Schema is not null && Schema == string.Empty)
            throw new ArgumentException("schema is an empty string.", nameof(Schema));

        this.Name = Name;
        this.Schema = Schema;
    }
}
