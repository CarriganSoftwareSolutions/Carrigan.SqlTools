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

    protected override SelectTags New(params IEnumerable<SelectTagBase> selectTags) =>
        new(selectTags);

    protected override SelectTag NewSelectTag<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTagGenerator.Get<T>(propertyName, aliasName);

    protected override SelectTags NewSelectTags<T>(params IEnumerable<PropertyName> properties) =>
        SelectTagGenerator.GetMany<T>(properties);

    public override SelectTags Append(SelectTagBase selectTag) =>
        new(All().Append(selectTag));

    public override SelectTags Append<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        Append(SelectTagGenerator.Get<T>(propertyName, aliasName));

    [ExternalOnly]
    public override SelectTags Append<T>(string property, string? aliasName = null) =>
        Append<T>(new PropertyName(property), AliasName.New(aliasName));

    public override SelectTags Concat(SelectTagsBase selectTags) =>
        new(All().Concat(selectTags.All()));

    public override SelectTags Concat(params IEnumerable<SelectTagBase> selectTags) =>
        new(All().Concat(selectTags));

    public override SelectTags Concat<T>(params IEnumerable<PropertyName> properties) =>
        Concat((SelectTagsBase)SelectTagGenerator.GetMany<T>(properties));

    [ExternalOnly]
    public override SelectTags Concat<T>(params IEnumerable<string> properties) =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    public static implicit operator SelectTags(SelectTag selectTag) =>
        new(selectTag);
}
