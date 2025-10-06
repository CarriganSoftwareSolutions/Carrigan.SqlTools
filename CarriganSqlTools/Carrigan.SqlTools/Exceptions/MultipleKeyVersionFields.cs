using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation
/// <summary>
/// Thrown when multiple key version fields exist
/// </summary>
internal class MultipleKeyVersionFields<T> : Exception
{
    /// <summary>
    /// Constructor for InvalidKeyVersionFieldType
    /// Thrown when multiple key version fields exist
    /// </summary>
    /// <param name="propertyNames">Invalid property names to include in exception message.</param>
    /// 
    internal MultipleKeyVersionFields(params IEnumerable<PropertyName> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }

    /// <summary>
    /// Builds the exception message from a collection of invalid PropertyName values.
    /// </summary>
    /// <param name="invalidPropertyNames">Invalid columns to include in exception message.</param>
    /// <returns>An exception message from a collection of invalid PropertyName values</returns>
    private static string CreateMessage(IEnumerable<PropertyName> invalidPropertyNames) =>
        $"The {typeof(T)} class has multiple key version properties, which is not allowed:"
            + invalidPropertyNames
                .Select(property => $"{property.ToString() ?? "<null>"}")
                .JoinAnd();
}
