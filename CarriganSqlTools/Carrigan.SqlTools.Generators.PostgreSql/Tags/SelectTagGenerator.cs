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
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
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
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    [ExternalOnly]
    public static SelectTag Get<T>(string propertyName, string? aliasName = null) where T : class =>
        Get<T>(new PropertyName(propertyName), AliasName.New(aliasName));

    /// <summary>
    /// Creates select tags for the specified properties on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
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
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    [ExternalOnly]
    public static SelectTags GetMany<T>(params IEnumerable<string> properties) where T : class =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Creates select tags for every supported mapped property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
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
