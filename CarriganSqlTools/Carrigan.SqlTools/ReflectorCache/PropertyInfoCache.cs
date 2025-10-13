using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: proof read documentation for entire class
/// <summary>
/// This class is a wrapper for a dictionary, where the key is a <see cref="PropertyName"/>
///  that corresponds to <see cref="PropertyInfo"/>'s Name property.
/// Note: the key for the wrapped dictionary is actually the property's name, 
/// since ProperrtyInfo does not implement the interfaces required for hashing.
/// It was meant to serve as a cache for the corresponding values for Attributes related
/// to <see cref="PropertyInfo"/>, such as <see cref="ColumnTag"/>, <see cref="TableTag"/>, <see cref="AliasTag"/>, etc, 
/// however, currently it is only serving as a cache for <see cref="ColumnInfo"/> which 
/// serves as a container for those fields.
/// <typeparamref name="typeT"/>
/// </summary>
/// <typeparam name="typeT">The type from which the type is being looked up.</typeparam>
/// <typeparam name="valueT">The datatype being cached.</typeparam>
internal class PropertyInfoCache<typeT, valueT> 
{
    /// <summary>
    /// Read only dictionary used as the core of the cache.
    /// </summary>
    private readonly IReadOnlyDictionary<PropertyName, valueT> _cache;

    /// <summary>
    /// This is the class constructor for PropertyInfoCache.
    /// </summary>
    /// <param name="data">An enumeration of tuples consisting of a property info and a value</param>
    internal PropertyInfoCache(IEnumerable<Tuple<PropertyInfo, valueT>> data) =>
        _cache = new ReadOnlyDictionary<PropertyName, valueT>
        (
            new Dictionary<PropertyName, valueT> 
            (
                data.Select
                (
                    tuple => new KeyValuePair<PropertyName, valueT>
                    (
                        new PropertyName(tuple.Item1.Name),
                        tuple.Item2
                    )
                )
            )
        );

    //TODO: make sure this can return null
    /// <summary>
    /// Returns the value for <paramref name="key"/> if present; otherwise <c>null</c>.
    /// Also returns <c>null</c> when the stored value is <c>null</c>.
    /// </summary>
    /// <param name="key">the property to look up</param>
    /// <exception cref="InvalidPropertyException{typeT}">This exception indicates that the property was invalid</exception>
    internal valueT Get(PropertyName key)
    {
        if (_cache.TryGetValue(key, out valueT? value))
            return value;
        else
            throw new InvalidPropertyException<typeT>(key);
    }

    //TODO: make sure this can return nulls
    /// <summary>
    /// Returns the values for <paramref name="keys"/> if present; 
    /// otherwise if any one of them doesn't exists, throw a <see cref="InvalidPropertyException{typeT}"/>
    /// Also returns <c>null</c> when the stored value is <c>null</c>.
    /// </summary>
    /// <param name="keys">the properties to look up</param>
    /// <returns></returns>
    /// <exception cref="InvalidPropertyException{typeT}">This exception indicates that one or more properties were invalid</exception>
    internal IEnumerable<valueT> GetMany(params IEnumerable<PropertyName> keys) 
    {
        IEnumerable<PropertyName> invalids = keys.Where(key => Exists(key) is false);

        if (invalids.Any())
            throw new InvalidPropertyException<typeT>(invalids);
        else
            return keys.Select(key => Get(key));
    }

    /// <summary>All values (some entries may be <c>null</c> by design).</summary>
    internal IEnumerable<valueT> Values => 
        _cache.Values;

    /// <summary>
    /// Determines if all of the property names exist in the cache
    /// </summary>
    /// <param name="propertyNames">The property names to test</param>
    /// <returns>true if all items in the enumeration exist. Else false.</returns>
    internal bool Exists(params IEnumerable<PropertyName> propertyNames) =>
        propertyNames.All(propertyName => _cache.ContainsKey(propertyName));

    /// <summary>
    /// Gets an <see cref="InvalidPropertyException{typeT}"/> with the property names in the message, or <c>null</c>.
    /// </summary>
    /// <param name="propertyNames">The property names to test</param>
    /// <returns>An cref="InvalidPropertyException{typeT}"/> if any invalid property names exist, else <c>null</c>.</returns>
    internal InvalidPropertyException<typeT>? GetExceptionForInvalidProperties(params IEnumerable<PropertyName> propertyNames)
    {
        IEnumerable<PropertyName> invalidPropertyNames = propertyNames.Where(propertyName => _cache.ContainsKey(propertyName) is false);
        if (invalidPropertyNames.Any())
            return new(invalidPropertyNames);
        else
            return null;
    }
}
