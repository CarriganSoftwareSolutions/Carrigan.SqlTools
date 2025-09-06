using Carrigan.Core.Attributes;
using Carrigan.Core.ReflectionCaching;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SqlTools.MsSqlClient;

internal static class ClientReflectorCache<T>
{
    internal static Type Type { get; }
    internal static IEnumerable<PropertyInfo> Properties => _LazyProperties.Value;
    internal static IEnumerable<PropertyInfo> EncryptedProperties => _LazyEncryptedProperties.Value;
    internal static PropertyInfo? KeyVersionProperty => _LazyKeyVersionProperty.Value;

    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyEncryptedProperties;
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyProperties;
    private static readonly Lazy<PropertyInfo?> _LazyKeyVersionProperty;
    static ClientReflectorCache()
    {
        Type = typeof(T);

        _LazyProperties = new Lazy<IEnumerable<PropertyInfo>>
            (() => [.. ReflectorCache<T>
                        .WriteablePublicInstanceProperties
                        .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null)]
            );

        _LazyEncryptedProperties = new Lazy<IEnumerable<PropertyInfo>>
            (() => Properties
                    .Where(property => property.GetCustomAttribute<EncryptedAttribute>() != null)
            );

        _LazyKeyVersionProperty = new Lazy<PropertyInfo?>
            (() => Properties
                    .Where(property => property.GetCustomAttribute<KeyVersionAttribute>() != null)
                    .FirstOrDefault()
            );
    }
}
