
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.ObjectModel;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: proof read documentation
//TODO: Unit tests
/// <summary>
/// This class is a wrapper for a dictionary, where the key is a
/// <see cref="PropertyInfo"/>.
/// Note: the key for the wrapped dictionary is actually the property name, 
/// since ProperrtyInfo does not implement the interfaces required for hashing.
/// It is meant to serve as a cache for the corresponding values for Attributes related
/// to PropertyInfo, such as Column, Table, Alias, etc.
/// </summary>
/// <typeparam name="valueT">The datatype being cached.</typeparam>
internal class PropertyInfoCache<valueT> 
{
    //TODO: documentation
    /// <summary>
    /// Read only dictionary used as the core of the cache.
    /// </summary>
    private readonly IReadOnlyDictionary<PropertyName, valueT> _cache;

    //TODO: documentation
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
    internal valueT Get(PropertyInfo key)
    {
        if (_cache.TryGetValue(new PropertyName(key.Name), out valueT? value))
            return value;
        else
            throw new Exception($"Value not found in {typeof(valueT).Name} cache for the property named {key.Name}.");
    }

    /// <summary>All values (some entries may be <c>null</c> by design).</summary>
    internal IEnumerable<valueT> Values => 
        _cache.Values; 
}
