using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies a parameter identifier for use with the SQL generator.
/// Enables mapping of class properties to custom parameter <see cref="Name"/> when
/// generating SQL based on property information. If not provided,
/// the parameter name defaults to the property name.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ParameterAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the name and schema.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The constructor for <see cref="ParameterAttribute"/>
    /// </summary>
    /// <param name="Name">Parameter name</param>
    /// <exception cref="SqlNamePatternException">Throws an  exception if the <see cref="Name"/> is an invalid SQL identifier.</exception>
    public ParameterAttribute(string Name)
    {
        if (SqlIdentifierPattern.Fails(Name))
        {
            throw new SqlNamePatternException(Name);
        }

        this.Name = Name;
    }
}
