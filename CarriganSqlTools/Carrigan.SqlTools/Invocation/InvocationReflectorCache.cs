using Carrigan.Core.ReflectionCaching;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.Invocation;

internal static class InvocationReflectorCache<T>
{
    internal static Type Type { get; }
    internal static Dictionary<string, PropertyInfo> Properties => _LazyProperties.Value;

    private static readonly Lazy<Dictionary<string, PropertyInfo>> _LazyProperties;
    static InvocationReflectorCache()
    {
        Type = typeof(T);


        _LazyProperties = new Lazy<Dictionary<string, PropertyInfo>>
            (() => new Dictionary<string, PropertyInfo>
                (
                    [.. ReflectorCache<T>
                        .WriteablePublicInstanceProperties
                        .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)
                        .Select(property => new KeyValuePair<string, PropertyInfo>(property.Name, property))]
                )
            ); 
    }
}
