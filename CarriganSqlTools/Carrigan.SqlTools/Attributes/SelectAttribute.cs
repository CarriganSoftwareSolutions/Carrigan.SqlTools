using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.RegularExpressions;

namespace Carrigan.SqlTools.Attributes;
[AttributeUsage(AttributeTargets.Property)]
//TODO: Proof read documentation for entire class
public class SelectAttribute : Attribute
{
    /// <summary>
    /// Public getter to indicate the Type of the data model for the represented <c>COLUMN</c>.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Public getter to name of the property.
    /// </summary>
    public PropertyName PropertyName { get; }

    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="type">c# data model <see cref="Type"/></param>
    /// <param name="propertyName">property name of property in <see cref="Type"/> <see <paramref name="type"/></param>
    /// <exception cref="InvalidSqlIdentifierException">If <see cref="PropertyName"/> or <see cref="PropertyName"/> have an invalid Sql Identifier</exception>
    public SelectAttribute(Type type, PropertyName propertyName)
    {
        Type = type;
        PropertyName = propertyName;
    }

}
