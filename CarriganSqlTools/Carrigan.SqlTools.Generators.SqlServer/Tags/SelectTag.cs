using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a dialect package's concrete SELECT projection tag for a single column.
/// </summary>
public sealed class SelectTag : SelectTagBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided property and optional alias names.
    /// </summary>
    /// <param name="propertyName">The property/column name to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    public SelectTag(PropertyName propertyName, AliasName? aliasName = null) : base(propertyName, aliasName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided property and optional alias names.
    /// </summary>
    /// <param name="propertyName">The property/column name to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    [ExternalOnly]
    public SelectTag(string propertyName, string? aliasName = null) : this(new PropertyName(propertyName), AliasName.New(aliasName))
    {
    }


    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the specified tag.
    /// </summary>
    /// <param name="selectTag">The select tag to append.</param>
    /// <returns>A new <see cref="SelectTags"/> collection containing both tags.</returns>
    public SelectTags Append(SelectTagBase selectTag) =>
        new SelectTags(this).Append(selectTag);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and a tag for the specified property.
    /// </summary>
    public SelectTags Append<T>(PropertyName propertyName, AliasName? aliasName = null) where T : class =>
        new SelectTags(this).Append<T>(propertyName, aliasName);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and a tag for the specified property.
    /// </summary>
    [ExternalOnly]
    public SelectTags Append<T>(string propertyName, string? aliasName = null) where T : class =>
        Append<T>(new PropertyName(propertyName), AliasName.New(aliasName));

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the supplied tags.
    /// </summary>
    public SelectTags Concat(SelectTagsBase selectTags) =>
        new SelectTags(this).Concat(selectTags);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the supplied tags.
    /// </summary>
    public SelectTags Concat(params IEnumerable<SelectTagBase> selectTags) =>
        new SelectTags(this).Concat(selectTags);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and tags for the specified properties.
    /// </summary>
    public SelectTags Concat<T>(params IEnumerable<PropertyName> properties) where T : class =>
        new SelectTags(this).Concat<T>(properties);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and tags for the specified properties.
    /// </summary>
    [ExternalOnly]
    public SelectTags Concat<T>(params IEnumerable<string> properties) where T : class =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using reflection-resolved column and alias tags.
    /// </summary>
    /// <param name="columnTag">The fully qualified column identifier to project.</param>
    /// <param name="aliasTag">An optional alias to use for this projection.</param>
    internal SelectTag(ColumnTag columnTag, AliasTag? aliasTag = null) : base(columnTag, aliasTag)
    {
    }

    /// <summary>
    /// Creates a new <see cref="SelectTag"/> instance with the same column as the current instance but without any alias.
    /// </summary>
    public override SelectTag WithNoAlias() =>
        new(ColumnTag);
}
