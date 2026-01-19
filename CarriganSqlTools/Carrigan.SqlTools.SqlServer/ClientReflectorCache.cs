using Carrigan.Core.Attributes;
using Carrigan.Core.ReflectionCaching;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Reflection provider used when invoking new instances with values read from the database.
/// </summary>
/// <typeparam name="T">The data type of the model.</typeparam>
internal static class ClientReflectorCache<T>
{
    /// <summary>
    /// Gets the model type.
    /// </summary>
    internal static Type Type { get; }
    /// <summary>
    /// Gets the writable public instance properties for <typeparamref name="T"/> that are not marked with <see cref="NotMappedAttribute"/>.
    /// </summary>
    internal static IEnumerable<PropertyInfo> Properties => _LazyProperties.Value;
    /// <summary>
    /// Gets the properties marked with <see cref="EncryptedAttribute"/>.
    /// </summary>
    internal static IEnumerable<PropertyInfo> EncryptedProperties => _LazyEncryptedProperties.Value;
    /// <summary>
    /// Gets the property marked with <see cref="KeyVersionAttribute"/> that stores the encryption key version used to decrypt and encrypt a record,
    /// or <see langword="null"/> when no such property is defined.
    /// </summary>
    internal static PropertyInfo? KeyVersionProperty => _LazyKeyVersionProperty.Value;

    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyEncryptedProperties;
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyProperties;
    private static readonly Lazy<PropertyInfo?> _LazyKeyVersionProperty;

    /// <summary>
    /// Initializes static members of the <see cref="ClientReflectorCache{T}"/> class.
    /// </summary>
    static ClientReflectorCache()
    {
        Type = typeof(T);

        _LazyProperties = new(() => [.. ReflectorCache<T>
                        .WriteablePublicInstanceProperties
                        .Where(static property => property.IsDefined(typeof(NotMappedAttribute), inherit: true) is false)]
            );

        _LazyEncryptedProperties = new(() => [.. Properties
                    .Where(static property => property.IsDefined(typeof(EncryptedAttribute), inherit: true))]
            );

        _LazyKeyVersionProperty = new(() => Properties
                    .FirstOrDefault(static property => property.IsDefined(typeof(KeyVersionAttribute), inherit: true))
            );
    }
}
