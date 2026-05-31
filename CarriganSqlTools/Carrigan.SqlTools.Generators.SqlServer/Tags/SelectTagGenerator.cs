using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Generates SQL Server select tags from reflection metadata.
/// </summary>
public static class SelectTagGenerator
{
    /// <summary>
    /// Creates a select tag for the specified property on <typeparamref name="T"/>.
    /// </summary>
    public static SelectTag Get<T>(PropertyName propertyName, AliasName? aliasName = null) where T : class =>
        SqlToolsReflectorCache<T>.CreateSelectTag
        (
            propertyName,
            DialectStatics.SupportedTypes,
            static (columnTag, aliasTag) => new SelectTag(columnTag, aliasTag),
            aliasName
        );

    /// <summary>
    /// Creates a select tag for the specified property on <typeparamref name="T"/>.
    /// </summary>
    [ExternalOnly]
    public static SelectTag Get<T>(string propertyName, string? aliasName = null) where T : class =>
        Get<T>(new PropertyName(propertyName), AliasName.New(aliasName));

    /// <summary>
    /// Creates select tags for the specified properties on <typeparamref name="T"/>.
    /// </summary>
    public static SelectTags GetMany<T>(params IEnumerable<PropertyName> properties) where T : class =>
        new
        (
            SqlToolsReflectorCache<T>.CreateSelectTags
            (
                DialectStatics.SupportedTypes,
                static (columnTag, aliasTag) => new SelectTag(columnTag, aliasTag),
                properties
            )
        );

    /// <summary>
    /// Creates select tags for the specified properties on <typeparamref name="T"/>.
    /// </summary>
    [ExternalOnly]
    public static SelectTags GetMany<T>(params IEnumerable<string> properties) where T : class =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Creates select tags for every supported mapped property on <typeparamref name="T"/>.
    /// </summary>
    public static SelectTags GetAll<T>() where T : class =>
        new
        (
            SqlToolsReflectorCache<T>.CreateAllSelectTags
            (
                DialectStatics.SupportedTypes,
                static (columnTag, aliasTag) => new SelectTag(columnTag, aliasTag)
            )
        );
}
