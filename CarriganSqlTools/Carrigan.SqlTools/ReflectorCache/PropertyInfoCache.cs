using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//IGNORE SPELLING: materializers

/// Provides a read-only, dictionary-backed cache keyed by <see cref="ResultColumnName"/>
/// to perform reverse look ups from an ADO.NET result column name to the corresponding
/// reflected <see cref="PropertyInfo"/> on the data model.
/// <para>
/// This enables materializers and invocation classes to map result sets back to properties
/// without recomputing reflection each time.
/// </para>
/// </summary>
/// <typeparam name="typeT">
/// The model/entity type from which the property information is being looked up.
/// </typeparam>
internal class PropertyInfoCache<typeT> 
{
    /// <summary>
    /// The read-only dictionary that serves as the core cache.
    /// Keys are <see cref="ResultColumnName"/> values; values are <see cref="PropertyInfo"/>.
    /// </summary>
    private readonly IReadOnlyDictionary<ResultColumnName, PropertyInfo> _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyInfoCache{typeT}"/> class.
    /// </summary>
    /// <param name="data">
    /// An enumeration of tuples mapping a <see cref="ResultColumnName"/> to a <see cref="PropertyInfo"/>.
    /// </param>
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
    /// Returns the <see cref="PropertyInfo"/> associated with
    /// <paramref name="resultColumnNameKey"/> if present; otherwise throws.
    /// </summary>
    /// <param name="resultColumnNameKey">
    /// The result column name used to reverse look up the <see cref="PropertyInfo"/>.
    /// </param>
    /// <returns>The associated <see cref="PropertyInfo"/>.</returns>
    /// <exception cref="InvalidResultColumnNameException{typeT}">
    /// Thrown when <paramref name="resultColumnNameKey"/> is not present in the cache.
    /// </exception>
    internal PropertyInfo Get(ResultColumnName resultColumnNameKey)
    {
        if (_cache.TryGetValue(resultColumnNameKey, out PropertyInfo? value))
            return value;
        else
            throw new InvalidResultColumnNameException<typeT>(resultColumnNameKey);
    }

    /// <summary>
    /// Returns the <see cref="PropertyInfo"/> values for all provided result column names,
    /// or throws if any name is not present in the cache.
    /// </summary>
    /// <param name="resultColumnNameKeys">The result column names to look up.</param>
    /// <returns>
    /// An enumeration of <see cref="PropertyInfo"/> values corresponding
    /// to <paramref name="resultColumnNameKeys"/>.
    /// </returns>
    /// <exception cref="InvalidResultColumnNameException{typeT}">
    /// Thrown when one or more provided result column names are not present in the cache.
    /// </exception>
    internal IEnumerable<PropertyInfo> GetMany(params IEnumerable<ResultColumnName> resultColumnNameKeys) 
    {
        IEnumerable<ResultColumnName> invalids = resultColumnNameKeys.Where(key => Exists(key) is false);

        if (invalids.Any())
            throw new InvalidResultColumnNameException<typeT>(invalids);
        else
            return resultColumnNameKeys.Select(key => Get(key));
    }

    /// <summary>
    /// Gets all cached <see cref="PropertyInfo"/> values.
    /// </summary>
    internal IEnumerable<PropertyInfo> Values => 
        _cache.Values;

    /// <summary>
    /// Determines whether all specified <see cref="ResultColumnName"/> values exist in the cache.
    /// </summary>
    /// <param name="resultColumnNames">The result column names to test.</param>
    /// <returns>
    /// <c>true</c> if every name exists; otherwise, <c>false</c>.
    /// </returns>
    internal bool Exists(params IEnumerable<ResultColumnName> resultColumnNames) =>
        resultColumnNames.All(ResultColumnName => _cache.ContainsKey(ResultColumnName));

    /// <summary>
    /// Creates an <see cref="InvalidResultColumnNameException{typeT}"/> that lists any
    /// invalid result column names, or returns <c>null</c> if all names are valid.
    /// </summary>
    /// <param name="resultColumnNames">The result column names to validate.</param>
    /// <returns>
    /// An <see cref="InvalidResultColumnNameException{typeT}"/> if any names are invalid; otherwise, <c>null</c>.
    /// </returns>
    internal InvalidResultColumnNameException<typeT>? GetExceptionForInvalidProperties(params IEnumerable<ResultColumnName> resultColumnNames)
    {
        IEnumerable<ResultColumnName> invalidResultColumnNames = resultColumnNames.Where(ResultColumnName => _cache.ContainsKey(ResultColumnName) is false);
        if (invalidResultColumnNames.Any())
            return new(invalidResultColumnNames);
        else
            return null;
    }
}
