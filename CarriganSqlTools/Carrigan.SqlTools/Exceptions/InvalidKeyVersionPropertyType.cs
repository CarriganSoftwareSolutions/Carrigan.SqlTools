using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation
/// <summary>
/// Thrown when the class has properties flagged for encryption and the specified key version property is not an int.
/// </summary>
public class InvalidKeyVersionPropertyType<T> : Exception
{
    /// <summary>
    /// Constructor for InvalidKeyVersionPropertyType
    /// Thrown when a property marked as a key version property is not of type int.
    /// </summary>
    /// <param name="propertyNames">Invalid property names to include in exception message.</param>
    /// 
    internal InvalidKeyVersionPropertyType(params IEnumerable<PropertyName> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }

    /// <summary>
    /// Builds the exception message from a collection of invalid PropertyName values.
    /// </summary>
    /// <param name="invalidPropertyNames">Invalid columns to include in exception message.</param>
    /// <returns>An exception message from a collection of invalid PropertyName values</returns>
    private static string CreateMessage(IEnumerable<PropertyName> invalidPropertyNames) =>
        $"{nameof(T)} has encrypted columns and the key version property, "
            + invalidPropertyNames
                .Select(property => $"{property.ToString() ?? "<null>"}")
                .JoinAnd()
            + ", is not of type int.";
}
