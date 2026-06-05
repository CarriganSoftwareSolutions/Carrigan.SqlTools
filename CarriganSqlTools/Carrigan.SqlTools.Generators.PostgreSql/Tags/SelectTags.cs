using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a dialect package's concrete collection of SELECT projection tags.
/// </summary>
public sealed class SelectTags : SelectTagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTags"/> class.
    /// </summary>
    /// <param name="selectTags">The select tags to include in this collection.</param>
    public SelectTags(params IEnumerable<SelectTagBase> selectTags) : base(selectTags)
    {
    }

    /// <summary>
    /// Creates a concrete SELECT projection collection from the supplied tags.
    /// </summary>
    /// <param name="selectTags">The SELECT projection tags to place in the new collection.</param>
    /// <returns>A new concrete SELECT tag collection.</returns>
    /// <summary>
    /// Creates a concrete SELECT projection collection from the supplied tags.
    /// </summary>
    /// <param name="selectTags">The SELECT projection tags to place in the new collection.</param>
    /// <returns>A new concrete SELECT tag collection.</returns>
    protected override SelectTags New(params IEnumerable<SelectTagBase> selectTags) =>
        new(selectTags);

    /// <summary>
    /// Creates a SELECT projection tag for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    /// <returns>A SELECT projection tag for the requested property and optional alias.</returns>
    protected override SelectTag NewSelectTag<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTagGenerator.Get<T>(propertyName, aliasName);

    /// <summary>
    /// Creates SELECT projection tags for multiple properties on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    /// <returns>A concrete SELECT tag collection containing the requested properties.</returns>
    protected override SelectTags NewSelectTags<T>(params IEnumerable<PropertyName> properties) =>
        SelectTagGenerator.GetMany<T>(properties);

    /// <summary>
    /// Creates a new collection with the supplied item appended.
    /// </summary>
    /// <param name="selectTag">The select tag to append.</param>
    /// <returns>A new collection containing the existing tags followed by <paramref name="selectTag"/>.</returns>
    public override SelectTags Append(SelectTagBase selectTag) =>
        new(All().Append(selectTag));

    /// <summary>
    /// Appends a SELECT projection tag for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    /// <returns>A new collection containing the additional projection tag.</returns>
    public override SelectTags Append<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        Append(SelectTagGenerator.Get<T>(propertyName, aliasName));

    /// <summary>
    /// Appends a SELECT projection tag for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="property">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    /// <returns>A new collection containing the additional projection tag.</returns>
    [ExternalOnly]
    public override SelectTags Append<T>(string property, string? aliasName = null) =>
        Append<T>(new PropertyName(property), AliasName.New(aliasName));

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    /// <returns>A new collection containing the existing tags followed by the supplied tags.</returns>
    public override SelectTags Concat(SelectTagsBase selectTags) =>
        new(All().Concat(selectTags.All()));

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    /// <returns>A new collection containing the existing tags followed by the supplied tags.</returns>
    public override SelectTags Concat(params IEnumerable<SelectTagBase> selectTags) =>
        new(All().Concat(selectTags));

    /// <summary>
    /// Appends SELECT projection tags for multiple properties on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    /// <returns>A new collection containing the additional projection tags.</returns>
    public override SelectTags Concat<T>(params IEnumerable<PropertyName> properties) =>
        Concat((SelectTagsBase)SelectTagGenerator.GetMany<T>(properties));

    /// <summary>
    /// Appends SELECT projection tags for multiple properties on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    /// <returns>A new collection containing the additional projection tags.</returns>
    [ExternalOnly]
    public override SelectTags Concat<T>(params IEnumerable<string> properties) =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Creates a SELECT tag collection containing a single projection tag.
    /// </summary>
    /// <param name="selectTag">The SELECT projection tag to place in the collection.</param>
    /// <returns>A new collection containing <paramref name="selectTag"/>.</returns>
    public static implicit operator SelectTags(SelectTag selectTag) =>
        new(selectTag);
}
