using Carrigan.Core.Attributes;
using Carrigan.Core.ReflectionCaching;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.SqlServer;

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
                        .Where(static property => property.IsDefined(typeof(NotMappedAttribute), inherit: true) is false)]
            );

        _LazyEncryptedProperties = new Lazy<IEnumerable<PropertyInfo>>
            (() => [.. Properties
                    .Where(static property => property.IsDefined(typeof(EncryptedAttribute), inherit: true))]
            );

        _LazyKeyVersionProperty = new Lazy<PropertyInfo?>
            (() => Properties
                    .FirstOrDefault(static property => property.IsDefined(typeof(KeyVersionAttribute), inherit: true))
            );
    }
}
