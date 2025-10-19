using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

/// <summary>
/// Provides a read-only, dictionary-backed cache keyed by <see cref="PropertyName"/>,
/// where the key corresponds to a reflected <see cref="PropertyInfo"/> name on the data model.
/// <para>
/// This wrapper enables fast look ups of reflection-derived metadata (e.g., column information)
/// by property name, while surfacing consistent exception semantics for invalid keys.
/// </para>
/// <remarks>
/// The underlying key is <see cref="PropertyName"/> (not <see cref="PropertyInfo"/>),
/// because <see cref="PropertyInfo"/> does not implement suitable hashing interfaces for use
/// as a dictionary key.
/// </remarks>
/// </summary>
/// <typeparam name="typeT">
/// The model/entity type whose properties the cache is keyed on (the “lookup source” type).
/// </typeparam>
/// <typeparam name="valueT">
/// The cached value type associated with each property (e.g., <c>ColumnInfo</c>).
/// </typeparam>
internal class ColumnInfoCache<typeT, valueT> 
{
    /// <summary>
    /// The read-only dictionary that serves as the core cache.
    /// Keys are <see cref="PropertyName"/> values derived from <see cref="PropertyInfo.Name"/>.
    /// </summary>
    private readonly IReadOnlyDictionary<PropertyName, valueT> _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnInfoCache{typeT, valueT}"/> class.
    /// </summary>
    /// <param name="data">
    /// An enumeration of tuples mapping a <see cref="PropertyInfo"/> to its cached value.
    /// The cache key is created from <see cref="PropertyInfo.Name"/>.
    /// </param>
    internal ColumnInfoCache(IEnumerable<Tuple<PropertyInfo, valueT>> data) =>
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
    /// Returns the cached value for <paramref name="propertyNameKey"/> if present;
    /// otherwise throws <see cref="InvalidPropertyException{typeT}"/>.
    /// </summary>
    /// <param name="propertyNameKey">The property name to look up.</param>
    /// <returns>The cached value associated with <paramref name="propertyNameKey"/>.</returns>
    /// <exception cref="InvalidPropertyException{typeT}">
    /// Thrown when <paramref name="propertyNameKey"/> does not exist in the cache.
    /// </exception>
    internal valueT Get(PropertyName propertyNameKey)
    {
        if (_cache.TryGetValue(propertyNameKey, out valueT? value))
            return value;
        else
            throw new InvalidPropertyException<typeT>(propertyNameKey);
    }

    /// <summary>
    /// Returns the cached values for the provided property names, or throws if any are invalid.
    /// </summary>
    /// <param name="propertyNameKeys">One or more property names to look up.</param>
    /// <returns>
    /// An enumeration of cached values corresponding to <paramref name="propertyNameKeys"/>.
    /// </returns>
    /// <exception cref="InvalidPropertyException{typeT}">
    /// Thrown when one or more property names are not present in the cache.
    /// </exception>
    internal IEnumerable<valueT> GetMany(params IEnumerable<PropertyName> propertyNameKeys) 
    {
        IEnumerable<PropertyName> invalids = propertyNameKeys.Where(key => Exists(key) is false);

        if (invalids.Any())
            throw new InvalidPropertyException<typeT>(invalids);
        else
            return propertyNameKeys.Select(key => Get(key));
    }

    /// <summary>
    /// Gets all cached values.
    /// </summary>
    internal IEnumerable<valueT> Values => 
        _cache.Values;

    /// <summary>
    /// Determines whether all specified property names exist in the cache.
    /// </summary>
    /// <param name="propertyNames">The property names to validate.</param>
    /// <returns>
    /// <c>true</c> if every name exists; otherwise, <c>false</c>.
    /// </returns>
    internal bool Exists(params IEnumerable<PropertyName> propertyNames) =>
        propertyNames.All(propertyName => _cache.ContainsKey(propertyName));

    /// <summary>
    /// Creates an <see cref="InvalidPropertyException{typeT}"/> that includes any invalid
    /// property names—if any are found—or returns <c>null</c> when all are valid.
    /// </summary>
    /// <param name="propertyNames">The property names to validate.</param>
    /// <returns>
    /// An <see cref="InvalidPropertyException{typeT}"/> if any names are invalid; otherwise, <c>null</c>.
    /// </returns>
    internal InvalidPropertyException<typeT>? GetExceptionForInvalidProperties(params IEnumerable<PropertyName> propertyNames)
    {
        IEnumerable<PropertyName> invalidPropertyNames = propertyNames.Where(propertyName => _cache.ContainsKey(propertyName) is false);
        if (invalidPropertyNames.Any())
            return new(invalidPropertyNames);
        else
            return null;
    }
}
