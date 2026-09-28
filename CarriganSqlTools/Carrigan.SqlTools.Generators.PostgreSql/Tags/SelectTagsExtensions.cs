using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Provides extension methods for working with collections of SelectTag objects.
/// </summary>
public static class SelectTagsExtensions
{

    /// <summary>
    /// Returns a new <see cref="SelectTags"/> containing the current items plus a select tag for the specified property.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTags">
    /// The SelectTags object to append new SelectTags to.
    /// </param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    public static SelectTags Append<T>(this SelectTags selectTags, PropertyName propertyName, AliasName? aliasName = null) where T : class =>
        selectTags.Append(SelectTagGenerator.Get<T>(propertyName, aliasName));

    /// <summary>
    /// Returns a new <see cref="SelectTags"/> containing the current items plus a select tag for the specified property.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTags">
    /// The SelectTags object to append new SelectTags to.
    /// </param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    [ExternalOnly]
    public static SelectTags Append<T>(this SelectTags selectTags, string propertyName, string? aliasName = null) where T : class =>
        selectTags.Append<T>(new PropertyName(propertyName), AliasName.New(aliasName));



    /// <summary>
    /// Returns a new <see cref="SelectTags"/> containing the current items plus select tags for the specified properties.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTags">
    /// The SelectTags object to append new SelectTags to.
    /// </param>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    public static SelectTags Concat<T>(this SelectTags selectTags, params IEnumerable<PropertyName> propertyNames) where T : class =>
        selectTags.Concat(propertyNames.Select(propertyName => new SelectTag<T>(propertyName)));

    /// <summary>
    /// Returns a new <see cref="SelectTags"/> containing the current items plus select tags for the specified properties.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="selectTags">
    /// The SelectTags object to append new SelectTags to.
    /// </param>
    /// <param name="propertyNames">The C# property names representing SQL columns or parameters.</param>
    [ExternalOnly]
    public static SelectTags Concat<T>(this SelectTags selectTags, params IEnumerable<string> propertyNames) where T : class =>
        selectTags.Concat<T>(propertyNames.Select(name => new PropertyName(name)));
}
