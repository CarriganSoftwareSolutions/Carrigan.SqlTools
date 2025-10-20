using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.RegularExpressions;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies a parameter identifier for SQL generation, allowing customization of
/// the parameter <see cref="Name"/> that corresponds to a property.
/// </summary>
/// <remarks>
/// When applied to a property, the SQL generator uses the specified parameter name instead
/// of the property name when binding query parameters.  
/// If the attribute is not applied, the property name is used as the default parameter name.
///
/// <para><b>Usage notes:</b></para>
/// <list type="bullet">
///   <item>
///     <description>This attribute affects SQL parameter naming only; it does not modify database schema or metadata.</description>
///   </item>
///   <item>
///     <description>The specified <see cref="Name"/> must be a valid SQL identifier; otherwise,
///     an <see cref="InvalidSqlIdentifierException"/> is thrown at construction time.</description>
///   </item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class ParameterAttribute : Attribute
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
