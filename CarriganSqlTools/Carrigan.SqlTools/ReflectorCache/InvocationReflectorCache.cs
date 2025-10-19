using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.ReflectorCache;

//TODO: Update Documentation

/// <summary>
/// This class serves as a cache for reflection operations commonly used by this library.
/// </summary>
/// <typeparam name="T">The type that are using with reflection and want to cache.</typeparam>
internal static class InvocationReflectorCache<T>
{
    //This library is essentially a static means of accessing reflection operations.
    internal static Type Type =>
        SqlToolsReflectorCache<T>.Type;

    internal static readonly PropertyInfoCache<T> PropertyInfoCache;
    static InvocationReflectorCache() =>
        PropertyInfoCache = new
                (
                    //yes, we get this is caching from a lower level of caching
                    //yes, this seems redundant, but I want to cache the reflection operations at various levels
                    //I do this because the application I developed this library for was one of several libraries accessing the lower cache and doing different things with it.
                    ReflectorCache<T>
                        .WriteablePublicInstanceProperties
                        .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
                        .Select(property => new Tuple<ResultColumnName, PropertyInfo>(GetResultColumnName(property), property))
                );

    /// <summary>
    /// Get the <see cref="ResultColumnName"/> for a given property.
    /// </summary>
    /// <param name="propertyInfo">A <see cref="PropertyInfo"/> representing the property being looked up.</param>
    /// <returns>
    /// the <see cref="ResultColumnName"/> for a given property.
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
