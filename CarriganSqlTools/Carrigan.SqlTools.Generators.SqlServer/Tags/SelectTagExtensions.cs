using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Provides dialect-specific convenience methods for extending a single <see cref="SelectTag"/> into a <see cref="SelectTags"/> collection.
/// </summary>
public static class SelectTagExtensions
{

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and a tag for the specified property.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTag">The select tag to append to.</param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    public static SelectTags Append<T>(this SelectTag selectTag, PropertyName propertyName, AliasName? aliasName = null) where T : class =>
        new SelectTags(selectTag).Append<T>(propertyName, aliasName);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and a tag for the specified property.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTag">The select tag to append to.</param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    [ExternalOnly]
    public static SelectTags Append<T>(this SelectTag selectTag, string propertyName, string? aliasName = null) where T : class =>
        selectTag.Append<T>(new PropertyName(propertyName), AliasName.New(aliasName));

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and tags for the specified properties.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTag">The select tag to append to.</param>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    public static SelectTags Concat<T>(this SelectTag selectTag, params IEnumerable<PropertyName> properties) where T : class =>
        new SelectTags(selectTag).Concat<T>(properties);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and tags for the specified properties.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTag">The select tag to append to.</param>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    [ExternalOnly]
    public static SelectTags Concat<T>(this SelectTag selectTag, params IEnumerable<string> properties) where T : class =>
        selectTag.Concat<T>(properties.Select(name => new PropertyName(name)));
}
