using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.Invocation;

//TODO: Update Documentation

/// <summary>
/// This class is serves as a cache for reflection operations commonly used by this library.
/// </summary>
/// <typeparam name="T">The type that are using with reflection and want to cache.</typeparam>
internal static class InvocationReflectorCache<T>
{
    //This library is essentially a static means of accessing lazy loaded reflection operations.
    internal static Type Type =>
        SqlToolsReflectorCache<T>.Type;

    internal static readonly ResultColumnCache<T> ResultColumnCache;
    static InvocationReflectorCache() =>
        ResultColumnCache = new
                (
                    //yes, we get this is caching from a lower level of caching
                    //yes, this seems redundant, but I want to cache the reflection operations at various levels
                    //I do this because the application I developed this library for was one of several libraries accessing the lower cache and doing different things with it.
                    ReflectorCache<T>
                        .WriteablePublicInstanceProperties
                        .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
                        .Select(property => new Tuple<ResultColumnName, PropertyInfo>(GetResultColumnName(property), property))
                );

    private static ResultColumnName GetResultColumnName(PropertyInfo propertyInfo) =>
        new
        (
            propertyInfo.GetCustomAttribute<AliasAttribute>()?.Name?.ToString().GetValueOrNull()
                ?? propertyInfo.GetCustomAttribute<IdentifierAttribute>()?.Name?.ToString().GetValueOrNull()
                ?? propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name?.GetValueOrNull()
                ?? propertyInfo.Name
        );
}
