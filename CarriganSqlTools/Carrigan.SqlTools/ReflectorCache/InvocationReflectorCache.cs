using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

/// <summary>
/// Provides a cache of reflection data used during invocation/materialization,
/// on top of the broader <see cref="SqlToolsReflectorCache{T}"/>.
/// <para>
/// This cache maps result-set column names to writable properties so that
/// ADO.NET result columns can be efficiently bound back to the model.
/// </para>
/// </summary>
/// <typeparam name="T">
/// The entity/data model type whose properties are being reflected and cached.
/// </typeparam>
internal static class InvocationReflectorCache<T>
{
    /// <summary>
    /// Gets the CLR <see cref="System.Type"/> for <typeparamref name="T"/>.
    /// </summary>
    internal static Type Type =>
        SqlToolsReflectorCache<T>.Type;

    /// <summary>
    /// Reverse-lookup cache from <see cref="ResultColumnName"/> to <see cref="PropertyInfo"/>.
    /// Populated with writable, public, instance properties that are not marked with <see cref="NotMappedAttribute"/>.
    /// </summary>
    internal static readonly PropertyInfoCache<T> PropertyInfoCache;

    /// <summary>
    /// Static constructor initializes the reverse-lookup cache used when
    /// mapping ADO.NET result columns back to model properties.
    /// <para>
    /// Note: This builds on lower-level reflection caches (<see cref="ReflectorCache{T}"/>)
    /// to avoid recomputation across different layers of the library.
    /// </para>
    /// </summary>
    static InvocationReflectorCache() =>
        PropertyInfoCache = new
                (
                    // Although this consumes a lower-level cache, we keep a dedicated layer here
                    // to isolate invocation/materialization concerns and minimize repeated work.
                    ReflectorCache<T>
                        .WriteablePublicInstanceProperties
                        .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
                        .Select(property => new Tuple<ResultColumnName, PropertyInfo>(GetResultColumnName(property), property))
                );

    /// <summary>
    /// Computes the <see cref="ResultColumnName"/> for a given property based on
    /// attribute overrides, falling back to the property name.
    /// </summary>
    /// <param name="propertyInfo">
    /// The reflected <see cref="PropertyInfo"/> for the property being resolved.
    /// </param>
    /// <returns>
    /// The <see cref="ResultColumnName"/> that should be used to match result-set columns.
    /// </returns>
    private static ResultColumnName GetResultColumnName(PropertyInfo propertyInfo) =>
        new
        (
            propertyInfo.GetCustomAttribute<AliasAttribute>()?.Name?.ToString().GetValueOrNull()
                ?? propertyInfo.GetCustomAttribute<IdentifierAttribute>()?.Name?.ToString().GetValueOrNull()
                ?? propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name?.GetValueOrNull()
                ?? propertyInfo.Name
        );
}
