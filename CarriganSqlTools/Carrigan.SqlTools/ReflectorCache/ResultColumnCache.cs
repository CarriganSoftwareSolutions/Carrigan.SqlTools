using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: proof read documentation for entire class

//TODO: rename to PropertyInfo cache, and get rid of valueT
/// <summary>
/// This class is a wrapper for a dictionary, where the key is a <see cref="ResultColumnName"/>
///  that corresponds to <see cref="ResultColumnName"/>'s .
/// Note: the key for the wrapped dictionary is actually the <see cref="ResultColumnName"/>.
/// </summary>
/// <typeparam name="typeT">The type from which the type is being looked up.</typeparam>
internal class ResultColumnCache<typeT> 
{
    /// <summary>
    /// Read only dictionary used as the core of the cache.
    /// </summary>
    private readonly IReadOnlyDictionary<ResultColumnName, PropertyInfo> _cache;

    /// <summary>
    /// This is the class constructor for ResultColumnCache.
    /// </summary>
    /// <param name="data">An enumeration of tuples consisting of a <see cref="ResultColumnName"/> and a <see cref="PropertyInfo"/> value</param>
    internal ResultColumnCache(IEnumerable<Tuple<ResultColumnName, PropertyInfo>> data) =>
        _cache = new ReadOnlyDictionary<ResultColumnName, PropertyInfo>
        (
            new Dictionary<ResultColumnName, PropertyInfo> 
            (
                data.Select
                (
                    tuple => new KeyValuePair<ResultColumnName, PropertyInfo>
                    (
                        new ResultColumnName(tuple.Item1),
                        tuple.Item2
                    )
                )
            )
        );

    /// <summary>
    /// Returns the value for <paramref name="key"/> if present; otherwise <c>null</c>.
    /// Also returns <c>null</c> when the stored value is <c>null</c>.
    /// </summary>
    /// <param name="key">the select to look up from</param>
    /// <exception cref="InvalidPropertyException{typeT}">This exception indicates that the <see cref="ResultColumnName"/> that was invalid</exception>
    internal PropertyInfo Get(ResultColumnName key)
    {
        if (_cache.TryGetValue(key, out PropertyInfo? value))
            return value;
        else
            throw new InvalidResultColumnNameException<typeT>(key);
    }

    /// <summary>
    /// Returns the values for <paramref name="keys"/> if present; 
    /// otherwise if any one of them doesn't exists, throw a <see cref="InvalidPropertyException{typeT}"/>
    /// Also returns <c>null</c> when the stored value is <c>null</c>.
    /// </summary>
    /// <param name="keys">the ResultColumnName to look up from</param>
    /// <returns></returns>
    /// <exception cref="InvalidPropertyException{typeT}">This exception indicates that one or more properties were invalid</exception>
    internal IEnumerable<PropertyInfo> GetMany(params IEnumerable<ResultColumnName> keys) 
    {
        IEnumerable<ResultColumnName> invalids = keys.Where(key => Exists(key) is false);

        if (invalids.Any())
            throw new InvalidResultColumnNameException<typeT>(invalids);
        else
            return keys.Select(key => Get(key));
    }

    /// <summary>All values (some entries may be <c>null</c> by design).</summary>
    internal IEnumerable<PropertyInfo> Values => 
        _cache.Values;

    /// <summary>
    /// Determines if all of the <see cref="ResultColumnName"/> exist in the cache
    /// </summary>
    /// <param name="resultColumnNames">The <see cref="ResultColumnName"/>s to test</param>
    /// <returns>true if all items in the enumeration exist. Else false.</returns>
    internal bool Exists(params IEnumerable<ResultColumnName> resultColumnNames) =>
        resultColumnNames.All(ResultColumnName => _cache.ContainsKey(ResultColumnName));

    /// <summary>
    /// Gets an <see cref="InvalidResultColumnNameException{typeT}"/> with the <see cref="ResultColumnName"/>s in the message, or <c>null</c>.
    /// </summary>
    /// <param name="resultColumnNames">The <see cref="ResultColumnName"/>s to test</param>
    /// <returns>An <see cref="InvalidResultColumnNameException{typeT}"/> if any invalid result column names exist, else <c>null</c>.</returns>
    internal InvalidResultColumnNameException<typeT>? GetExceptionForInvalidProperties(params IEnumerable<ResultColumnName> resultColumnNames)
    {
        IEnumerable<ResultColumnName> invalidResultColumnNames = resultColumnNames.Where(ResultColumnName => _cache.ContainsKey(ResultColumnName) is false);
        if (invalidResultColumnNames.Any())
            return new(invalidResultColumnNames);
        else
            return null;
    }
}
