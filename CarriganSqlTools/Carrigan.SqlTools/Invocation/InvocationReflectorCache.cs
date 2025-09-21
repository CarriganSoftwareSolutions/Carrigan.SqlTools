using Carrigan.Core.ReflectionCaching;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.Invocation;

//TODO: I still need to update this to take advantage to the new Attributes

/// <summary>
/// This class is serves as a cache for reflection operations commonly used by this library.
/// </summary>
/// <typeparam name="T">The type that are using with reflection and want to cache.</typeparam>
internal static class InvocationReflectorCache<T>
{
    //This library is essentially a static means of accessing lazy loaded reflection operations.
    internal static Type Type { get; } 
    internal static Dictionary<string, PropertyInfo> Properties => _LazyProperties.Value;

    private static readonly Lazy<Dictionary<string, PropertyInfo>> _LazyProperties;
    static InvocationReflectorCache()
    {
        Type = typeof(T);


        _LazyProperties = new Lazy<Dictionary<string, PropertyInfo>>
            (() => new Dictionary<string, PropertyInfo>
                (
                    //yes, we get this is caching from a lower level of caching
                    //yes, this seems redundant, but I want to cache the reflection operations at various levels
                    //I do this because the application I developed this library for was one of several libraries accessing the lower cache and doing different things with it.
                    [.. ReflectorCache<T> 
                        .WriteablePublicInstanceProperties
                        .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
                        .Select(property => new KeyValuePair<string, PropertyInfo>(property.Name, property))]
                )
            ); 
    }
}
