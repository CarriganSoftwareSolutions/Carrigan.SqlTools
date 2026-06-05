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
    protected override SelectTags New(params IEnumerable<SelectTagBase> selectTags) =>
        new(selectTags);

    /// <summary>
    /// Creates a dialect-specific SELECT projection tag for the supplied model property.
    /// </summary>
    /// <typeparam name="T">The model type that contains the selected property.</typeparam>
    /// <param name="propertyName">The property name to select.</param>
    /// <param name="aliasName">The optional result-column alias.</param>
    /// <returns>A SELECT projection tag for the supplied property.</returns>
    protected override SelectTag NewSelectTag<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTagGenerator.Get<T>(propertyName, aliasName);

    /// <summary>
    /// Creates a dialect-specific SELECT projection collection for the supplied model properties.
    /// </summary>
    /// <typeparam name="T">The model type that contains the selected properties.</typeparam>
    /// <param name="properties">The property names to select.</param>
    /// <returns>A SELECT projection collection for the supplied properties.</returns>
    protected override SelectTags NewSelectTags<T>(params IEnumerable<PropertyName> properties) =>
        SelectTagGenerator.GetMany<T>(properties);

    /// <summary>
    /// Returns a new collection with the supplied SELECT projection tag appended.
    /// </summary>
    /// <param name="selectTag">The SELECT projection tag to append.</param>
    /// <returns>A new SELECT projection collection.</returns>
    public override SelectTags Append(SelectTagBase selectTag) =>
        new(All().Append(selectTag));

    /// <summary>
    /// Returns a new collection with a SELECT projection for the supplied model property appended.
    /// </summary>
    /// <typeparam name="T">The model type that contains the selected property.</typeparam>
    /// <param name="propertyName">The property name to select.</param>
    /// <param name="aliasName">The optional result-column alias.</param>
    /// <returns>A new SELECT projection collection.</returns>
    public override SelectTags Append<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        Append(SelectTagGenerator.Get<T>(propertyName, aliasName));

    /// <summary>
    /// Returns a new collection with a SELECT projection for the supplied model property appended.
    /// </summary>
    /// <typeparam name="T">The model type that contains the selected property.</typeparam>
    /// <param name="property">The property name to select.</param>
    /// <param name="aliasName">The optional result-column alias.</param>
    /// <returns>A new SELECT projection collection.</returns>
    [ExternalOnly]
    public override SelectTags Append<T>(string property, string? aliasName = null) =>
        Append<T>(new PropertyName(property), AliasName.New(aliasName));

    /// <summary>
    /// Returns a new collection containing the current SELECT projection tags followed by another collection's tags.
    /// </summary>
    /// <param name="selectTags">The SELECT projection collection to concatenate.</param>
    /// <returns>A new SELECT projection collection.</returns>
    public override SelectTags Concat(SelectTagsBase selectTags) =>
        new(All().Concat(selectTags.All()));

    /// <summary>
    /// Returns a new collection containing the current SELECT projection tags followed by the supplied tags.
    /// </summary>
    /// <param name="selectTags">The SELECT projection tags to concatenate.</param>
    /// <returns>A new SELECT projection collection.</returns>
    public override SelectTags Concat(params IEnumerable<SelectTagBase> selectTags) =>
        new(All().Concat(selectTags));

    /// <summary>
    /// Returns a new collection containing the current SELECT projection tags followed by projections for the supplied model properties.
    /// </summary>
    /// <typeparam name="T">The model type that contains the selected properties.</typeparam>
    /// <param name="properties">The property names to select.</param>
    /// <returns>A new SELECT projection collection.</returns>
    public override SelectTags Concat<T>(params IEnumerable<PropertyName> properties) =>
        Concat((SelectTagsBase)SelectTagGenerator.GetMany<T>(properties));

    /// <summary>
    /// Returns a new collection containing the current SELECT projection tags followed by projections for the supplied model properties.
    /// </summary>
    /// <typeparam name="T">The model type that contains the selected properties.</typeparam>
    /// <param name="properties">The property names to select.</param>
    /// <returns>A new SELECT projection collection.</returns>
    [ExternalOnly]
    public override SelectTags Concat<T>(params IEnumerable<string> properties) =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Converts a single SELECT projection tag into a SELECT projection collection.
    /// </summary>
    /// <param name="selectTag">The SELECT projection tag to wrap.</param>
    public static implicit operator SelectTags(SelectTag selectTag) =>
        new(selectTag);

}
