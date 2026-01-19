using Carrigan.Core.Attributes;
using Carrigan.Core.ReflectionCaching;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// reflection provider used when invoking new instances with values read from the database.
/// </summary>
/// <typeparam name="T">the datatype of the model</typeparam>
internal static class ClientReflectorCache<T>
{
    /// <summary>
    /// class type
    /// </summary>
    internal static Type Type { get; }
    /// <summary>
    /// Properties of the type
    /// </summary>
    internal static IEnumerable<PropertyInfo> Properties => _LazyProperties.Value;
    /// <summary>
    /// properties that are encrypted.
    /// </summary>
    internal static IEnumerable<PropertyInfo> EncryptedProperties => _LazyEncryptedProperties.Value;
    /// <summary>
    /// stores the encryption key version used to decrypt / decrypt a record
    /// </summary>
    internal static PropertyInfo? KeyVersionProperty => _LazyKeyVersionProperty.Value;

    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyEncryptedProperties;
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyProperties;
    private static readonly Lazy<PropertyInfo?> _LazyKeyVersionProperty;

    /// <summary>
    /// static constructor
    /// </summary>
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
