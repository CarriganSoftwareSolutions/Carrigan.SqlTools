using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Exceptions;

//TODO: Proof Read Documentation. entire class

//TODO: Create example for readme.md file.
//TODO: Unit tests?
/// <summary>
/// The InvalidPropertyException is thrown when a property is passed in that does exist
/// in the target class <see cref="T"/>, or the property does not meet the criteria to
/// be used as a class.
/// </summary>
/// <typeparam name="T">Type T for which the property was to be used.</typeparam>
public class InvalidPropertyException<T> : Exception
{
    /// <summary>
    /// This is the constructor for InvalidPropertyException.
    /// The InvalidPropertyException is thrown when a property is passed in that does exist
    /// in the target class <see cref="T"/>, or the property does not meet the criteria to
    /// be used as a class.
    /// </summary>
    /// <param name="propertyNames">The names of the invalid properties.</param>
    internal InvalidPropertyException(params IEnumerable<PropertyName> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }
    /// <summary>
    /// Create a message for the <see cref="InvalidPropertyException{T}"/>
    /// </summary>
    /// <param name="propertyNames">The names of the invalid properties.</param>
    /// <returns>An <see cref="InvalidPropertyException{T}"/> message.</returns>
    private static string CreateMessage(IEnumerable<PropertyName> propertyNames) =>
        $"Property names for {SqlToolsReflectorCache<T>.Type.Name}, do not exist, are invalid or do qualify: " +
            propertyNames
                .Select(property => (string)property) //TODO: simplify this?
                .JoinAnd();
}
