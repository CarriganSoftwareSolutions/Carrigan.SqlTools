using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Documentation, unit tests
/// <summary>
/// Thrown when the column tag comes from a table that is not included in the clauses.
/// </summary>
internal class InvalidKeyVersionFieldType : Exception
{
    /// <summary>
    /// Constructor for InvalidKeyVersionFieldType
    /// Thrown when a property marked as a key version field is not of type int.
    /// </summary>
    /// <param name="propertyNames">Invalid property names to include in exception message.</param>
    /// 
    internal InvalidKeyVersionFieldType(params IEnumerable<PropertyName> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }

    /// <summary>
    /// Builds the exception message from a collection of invalid PropertyName values.
    /// </summary>
    /// <param name="invalidPropertyNames">Invalid columns to include in exception message.</param>
    /// <returns>An exception message from a collection of invalid PropertyName values</returns>
    private static string CreateMessage(IEnumerable<PropertyName> invalidPropertyNames) =>
        $"The following key version properties are not of type int"
            + invalidPropertyNames
                .Select(property => $"{property.ToString() ?? "<null>"}")
                .JoinAnd();
}
