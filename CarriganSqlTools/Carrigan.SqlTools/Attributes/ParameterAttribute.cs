using Carrigan.SqlTools.Exceptions;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies the SQL parameter identifier used by this library’s SQL generators for a property.
/// </summary>
/// <remarks>
/// In Carrigan.SqlTools, a <b>property</b> represents an SQL <b>parameter</b> when values are
/// bound to generated SQL statements. When this attribute is applied, the SQL generator
/// uses the specified parameter name instead of the CLR property name.
/// <para>
/// If this attribute is not applied, the property name is used as the default SQL parameter
/// identifier.
/// </para>
/// <para><b>Usage notes</b></para>
/// <list type="bullet">
///   <item>
///     <description>
///     This attribute affects SQL parameter naming only; it does not modify database schema,
///     column metadata, or stored procedure definitions.
///     </description>
///   </item>
///   <item>
///     <description>
///     The parameter name must be a non-null, non-empty string. Validation of SQL identifier
///     syntax is performed by the SQL generation pipeline, not by this attribute.
///     </description>
///   </item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ParameterAttribute : Attribute
{
    /// <summary>
    /// Gets the SQL parameter name that corresponds to the decorated property.
    /// </summary>
    internal string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
    /// </summary>
    /// <param name="Name">The SQL parameter name to use instead of the property name.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="Name"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="Name"/> is an empty string.</exception>
    /// <exception cref="InvalidSqlIdentifierException">Thrown when <paramref name="Name"/> is not a valid SQL identifier.</exception>
    public ParameterAttribute(string Name)
    {
        ArgumentNullException.ThrowIfNull(Name, nameof(Name));
        if (Name == string.Empty)
            throw new ArgumentException("name is an empty string", nameof(Name));
        this.Name = Name;
    }
}
