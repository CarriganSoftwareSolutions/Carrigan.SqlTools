using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: proof read documentation for entire class
//TODO: Unit tests for entire class
/// <summary>
/// This class is a wrapper for a dictionary, where the key is a
/// <see cref="PropertyInfo"/>.
/// Note: the key for the wrapped dictionary is actually the property name, 
/// since ProperrtyInfo does not implement the interfaces required for hashing.
/// It is meant to serve as a cache for the corresponding values for Attributes related
/// to PropertyInfo, such as Column, Table, Alias, etc.
/// I later added support for to lookup using a <see cref="PropertyName"/> as well.
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
    /// <summary>
    /// This is the class constructor for PropertyInfoCache.
    /// </summary>
    /// <param name="data">An enumeration of tuples consisting of a property info and a value</param>
    internal PropertyInfoCache(IEnumerable<Tuple<PropertyName, valueT>> data) =>
        _cache = new ReadOnlyDictionary<PropertyName, valueT>
        (
            new Dictionary<PropertyName, valueT>
            (
                data.Select
                (
                    tuple => new KeyValuePair<PropertyName, valueT>
                    (
                        new PropertyName(tuple.Item1),
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
    internal valueT Get(PropertyInfo key) =>
        Get(new PropertyName(key.Name));

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
    internal IEnumerable<valueT> GetMany(IEnumerable<PropertyName> keys) 
    {
        IEnumerable<PropertyName> invalids = keys.Where(key => Exists(key) is false);

        if (invalids.Any())
            throw new InvalidPropertyException<typeT>(invalids);
        else
            return keys.Select(key => Get(key));
    }

    //TODO: make sure this can return nulls
    /// <summary>
    /// Returns the values for <paramref name="keys"/> if present; 
    /// otherwise if any one of them doesn't exists, throw a <see cref="InvalidPropertyException{typeT}"/>
    /// Also returns <c>null</c> when the stored value is <c>null</c>.
    /// <param name="keys">the properties to look up</param>
    /// <exception cref="InvalidPropertyException{typeT}">This exception indicates that one or more properties were invalid</exception>
    internal IEnumerable<valueT> GetMany(IEnumerable<PropertyInfo> keys) =>
        GetMany(keys.Select(key => new PropertyName(key.Name)));

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
    /// Determines if all of the properties exist in the cache
    /// </summary>
    /// <param name="propertyNames">The properties to test</param>
    /// <returns>true if all items in the enumeration exist. Else false.</returns>
    internal bool Exists(params IEnumerable<PropertyInfo> properties) =>
        Exists(properties.Select(property => new PropertyName(property.Name)));

    /// <summary>
    /// Gets an <see cref="InvalidPropertyException{typeT}"/> with the property names in the message, or <c>null</c>.
    /// </summary>
    /// <param name="propertyNames">The property names to test</param>
    /// <returns>An cref="InvalidPropertyException{typeT}"/> if any invalid property names exist, else <c>null</c>.</returns>
    internal InvalidPropertyException<typeT>? GetExceptionForInvalidPropertyNames(params IEnumerable<PropertyName> propertyNames)
    {
        IEnumerable<PropertyName> invalidPropertyNames = propertyNames.Where(propertyName => _cache.ContainsKey(propertyName) is false);
        if (invalidPropertyNames.Any())
            return new(invalidPropertyNames);
        else
            return null;
    }

    /// <summary>
    /// Gets an <see cref="InvalidPropertyException{typeT}"/> with the name of each property in the message, or <c>null</c>.
    /// </summary>
    /// <param name="propertyNames">The properties to test</param>
    /// <returns>An cref="InvalidPropertyException{typeT}"/> if any invalid property names exist, else null.</returns>
    internal InvalidPropertyException<typeT>? GetExceptionForInvalidProperties(params IEnumerable<PropertyInfo> properties) =>
        GetExceptionForInvalidPropertyNames(properties.Select(property => new PropertyName(property.Name)));

    //TODO: Documentation
    internal PropertyInfoCache<typeT, valueT> GetSubCache(IEnumerable<PropertyName> keys)
    {
        IEnumerable<PropertyName> invalids = keys.Where(key => Exists(key) is false);

        if (invalids.Any())
            throw new InvalidPropertyException<typeT>(invalids);
        else
            return new (keys.Select(property => new Tuple<PropertyName, valueT>(property, Get(property))));
    }
}
