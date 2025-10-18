using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Exceptions;

//TODO: Proof Read Documentation. entire class
/// <summary>
/// The InvalidPropertyException is thrown when a utilized property name does not exist 
/// in the target class <see cref="T"/>, or the property does not meet the criteria for usable properties.
/// Usable properties are public and readable properties for SQL generation. 
/// For invocation, usable properties should be public and writable.
/// It is recommended to model all properties as public with a simple getter and setter.
/// </summary>
/// <typeparam name="T">Type T for which the property was to be used.</typeparam>
public class InvalidPropertyException<T> : Exception
{
    /// <summary>
    /// This is the constructor for InvalidPropertyException.
    /// The InvalidPropertyException is thrown when a property is passed in that does exist
    /// in the target class <see cref="T"/>, or the property does not meet the criteria for usable properties.
    /// Usable properties are public and readable properties for SQL generation. 
    /// For invocation, usable properties should be public and writable.
    /// It is recommended to model all properties as public with a simple getter and setter.
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
        $"Property names for {SqlToolsReflectorCache<T>.Type.Name}, do not exist, are invalid or do not qualify: " +
            propertyNames
                .Select(property => (string)property)
                .JoinAnd();
}
