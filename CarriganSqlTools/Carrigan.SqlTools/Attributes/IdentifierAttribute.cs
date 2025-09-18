using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// A data annotation to specify a name schema identifier to use with the SQL Generator
/// Note: this doe not override the effects of name and schema annotation used with Entity Framework
/// Note: it will override name and schema annotation in terms of the SQL Generator
/// It is recommended to use one of the other annotation type depending.
/// Use the Microsoft provided annotations, if you need annotations for Entity Framework
/// Use the annotations provided with the SQL generator if you are using Entity Frame work,but are using fluent to define identities.
/// Use either one if you aren't using Entity Framework at all.
/// Use neither if you want to just use the class/property names as the sql identity.
/// Note: The SQL generator doesn't generate SQL to create, modify or delete tables, procedures or columns.
/// The SQL generator only generates SQL to Select, Delete, Insert Update or exec a procedure.
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
            throw new ArgumentException("SQL identity names cannot be null or empty.", nameof(Name));
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
