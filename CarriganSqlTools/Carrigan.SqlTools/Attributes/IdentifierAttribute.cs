using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// A data annotation to specify a name schema identifier to use with the SQL Generator
/// Note: this doe not override the EF used annotations or fluent name and schema.
/// This should have been part of Carrigan.SqlTools
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class IdentifierAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the name and schema.
    /// </summary>
    public string Name { get; }
    public string? Schema { get; }

    public IdentifierAttribute(string Name, string Schema = "")
    {
        bool validName = true;
        bool validSchema = true;
        if (Name.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Procedure name cannot be null or empty.", nameof(Name));
        }
        else if (SqlIdentifierPattern.Fails(Name))
        {
            validName = false;
        }

        if (Schema.IsNotNullOrEmpty() && SqlIdentifierPattern.Fails(Schema))
        {
            validSchema = false;
        }

        if(validName == false && validSchema == false)
        {
            throw new SqlNamePatternException(Name, Schema);
        }
        else if(validName == false)
        {
            throw new SqlNamePatternException(Name);
        }
        else if (validSchema == false)
        {
            throw new SqlNamePatternException(Schema);
        }
        this.Name = Name;
        this.Schema = Schema;
    }
}
