
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.ObjectModel;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: documentation
//TODO: Unit tests
/// <typeparam name="keyT"></typeparam>
/// <typeparam name="valueT"></typeparam>
internal class PropertyInfoCache<valueT> 
{
    //TODO: documentation
    private readonly IReadOnlyDictionary<PropertyName, valueT> _cache;

    //TODO: documentation
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
