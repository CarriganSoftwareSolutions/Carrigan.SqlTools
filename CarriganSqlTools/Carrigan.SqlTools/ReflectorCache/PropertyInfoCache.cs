using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: proof read documentation for entire class

/// <summary>
/// This class is a wrapper for a dictionary, where the key is a <see cref="ResultColumnName"/>.
/// This class is used to reverse look up Property Information from a result column name.
/// </summary>
/// <typeparam name="typeT">The data mode from which the property information is being looked up.</typeparam>
internal class PropertyInfoCache<typeT> 
{
    /// <summary>
    /// Read only dictionary used as the core of the cache.
    /// </summary>
    private readonly IReadOnlyDictionary<ResultColumnName, PropertyInfo> _cache;

    /// <summary>
    /// This is the class constructor for PropertyInfoCache.
    /// </summary>
    /// <param name="data">An enumeration of tuples consisting of a <see cref="ResultColumnName"/> and a <see cref="PropertyInfo"/> value</param>
    internal PropertyInfoCache(IEnumerable<Tuple<ResultColumnName, PropertyInfo>> data) =>
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
    /// Returns the <see cref="PropertyInfo"/> associated with <paramref name="resultColumnNameKey"/> if present; otherwise <c>null</c>.
    /// Also returns <c>null</c> when the stored value is <c>null</c>.
    /// </summary>
    /// <param name="resultColumnNameKey">the result column used to reverse look up the <see cref="PropertyInfo"/> from</param>
    /// <exception cref="InvalidPropertyException{typeT}">This exception indicates that the <see cref="ResultColumnName"/> that was invalid</exception>
    internal PropertyInfo Get(ResultColumnName resultColumnNameKey)
    {
        if (_cache.TryGetValue(resultColumnNameKey, out PropertyInfo? value))
            return value;
        else
            throw new InvalidResultColumnNameException<typeT>(resultColumnNameKey);
    }

    /// <summary>
    /// Returns the <see cref="PropertyInfo"/> for <paramref name="resultColumnNameKeys"/> if present; 
    /// otherwise if any one of them doesn't exists, throw a <see cref="InvalidPropertyException{typeT}"/>
    /// </summary>
    /// <param name="resultColumnNameKeys">the ResultColumnName to look up from</param>
    /// <returns></returns>
    /// <exception cref="InvalidPropertyException{typeT}">This exception indicates that one or more properties were invalid</exception>
    internal IEnumerable<PropertyInfo> GetMany(params IEnumerable<ResultColumnName> resultColumnNameKeys) 
    {
        IEnumerable<ResultColumnName> invalids = resultColumnNameKeys.Where(key => Exists(key) is false);

        if (invalids.Any())
            throw new InvalidResultColumnNameException<typeT>(invalids);
        else
            return resultColumnNameKeys.Select(key => Get(key));
    }

    /// <summary>All <see cref="PropertyInfo"/> values.</summary>
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
