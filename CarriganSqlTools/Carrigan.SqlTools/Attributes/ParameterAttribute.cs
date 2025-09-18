using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// A data annotation to specify a parameter identifier to use with the SQL Generator
/// This lets you map properties in a class to a default parameter name, for generations that use default parameter names based on property info.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ParameterAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the name and schema.
    /// </summary>
    public string Name { get; }

    public ParameterAttribute(string Name)
    {
        if (Name.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("SQL parameter names name cannot be null or empty.", nameof(Name));
        }
        else if (SqlIdentifierPattern.Fails(Name))
        {
            throw new SqlNamePatternException(Name);
        }

        this.Name = Name;
    }
}
