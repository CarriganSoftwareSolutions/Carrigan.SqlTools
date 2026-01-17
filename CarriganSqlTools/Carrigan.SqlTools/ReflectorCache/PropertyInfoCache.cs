using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//IGNORE SPELLING: materializers

/// <summary>
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
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="data"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="data"/> contains disallowed <c>null</c> values.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when duplicate <see cref="ResultColumnName"/> keys are provided.
    /// </exception>
    internal PropertyInfoCache(IEnumerable<Tuple<ResultColumnName, PropertyInfo>> data)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        IEnumerable<Tuple<ResultColumnName, PropertyInfo>> materialized = data.Materialize(NullOptionsEnum.Exception);

        Dictionary<ResultColumnName, PropertyInfo> cache = [];
        foreach (Tuple<ResultColumnName, PropertyInfo> tuple in materialized)
        {
            ArgumentNullException.ThrowIfNull(tuple.Item1, nameof(data));
            ArgumentNullException.ThrowIfNull(tuple.Item2, nameof(data));

            ResultColumnName key = new(tuple.Item1);

            if (cache.ContainsKey(key))
                throw new ArgumentException($"Duplicate {nameof(ResultColumnName)} detected: {key}.", nameof(data));

            cache.Add(key, tuple.Item2);
        }

        _cache = new ReadOnlyDictionary<ResultColumnName, PropertyInfo>(cache);
    }

    /// <summary>
    /// Returns the <see cref="PropertyInfo"/> associated with <paramref name="resultColumnNameKey"/> if present;
    /// otherwise throws.
    /// </summary>
    /// <param name="resultColumnNameKey">
    /// The result column name used to reverse lookup the <see cref="PropertyInfo"/>.
    /// </param>
    /// <returns>The associated <see cref="PropertyInfo"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultColumnNameKey"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidResultColumnNameException{typeT}">
    /// Thrown when <paramref name="resultColumnNameKey"/> is not present in the cache.
    /// </exception>
    internal PropertyInfo Get(ResultColumnName resultColumnNameKey)
    {
        ArgumentNullException.ThrowIfNull(resultColumnNameKey, nameof(resultColumnNameKey));

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
    /// An enumeration of <see cref="PropertyInfo"/> values corresponding to <paramref name="resultColumnNameKeys"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultColumnNameKeys"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="resultColumnNameKeys"/> contains disallowed <c>null</c> values.
    /// </exception>
    /// <exception cref="InvalidResultColumnNameException{typeT}">
    /// Thrown when one or more provided result column names are not present in the cache.
    /// </exception>
    internal IEnumerable<PropertyInfo> GetMany(params IEnumerable<ResultColumnName> resultColumnNameKeys)
    {
        ArgumentNullException.ThrowIfNull(resultColumnNameKeys, nameof(resultColumnNameKeys));
        IEnumerable<ResultColumnName> keys = resultColumnNameKeys.Materialize(NullOptionsEnum.Exception);

        IEnumerable<ResultColumnName> invalids = keys.Where(key => _cache.ContainsKey(key) is false);

        if (invalids.Any())
            throw new InvalidResultColumnNameException<typeT>(invalids);
        else
            return keys.Select(Get);
    }

    /// <summary>
    /// Gets all cached <see cref="PropertyInfo"/> values.
    /// </summary>
    internal IEnumerable<PropertyInfo> Values =>
        _cache.Values;

    /// <summary>
    /// Creates an <see cref="InvalidResultColumnNameException{typeT}"/> that lists any invalid result column names,
    /// or returns <c>null</c> if all names are valid.
    /// </summary>
    /// <param name="resultColumnNames">The result column names to validate.</param>
    /// <returns>
    /// An <see cref="InvalidResultColumnNameException{typeT}"/> if any names are invalid; otherwise, <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultColumnNames"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="resultColumnNames"/> contains disallowed <c>null</c> values.
    /// </exception>
    internal InvalidResultColumnNameException<typeT>? GetExceptionForInvalidProperties(params IEnumerable<ResultColumnName> resultColumnNames)
    {
        ArgumentNullException.ThrowIfNull(resultColumnNames, nameof(resultColumnNames));
        IEnumerable<ResultColumnName> keys = resultColumnNames.Materialize(NullOptionsEnum.Exception);

        IEnumerable<ResultColumnName> invalidResultColumnNames =
            keys.Where(key => _cache.ContainsKey(key) is false);

        if (invalidResultColumnNames.Any())
            return new(invalidResultColumnNames);
        else
            return null;
    }
}
