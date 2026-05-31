using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a collection of <see cref="SelectTagBase"/> items and provides common collection
/// behavior for dialect-specific select-tag containers.
/// </summary>
public abstract class SelectTagsBase : ISqlFragment, IEnumerable<SelectTagBase>
{
    private readonly IEnumerable<SelectTagBase> _selectTags;

    /// <summary>
    /// Creates a new concrete instance using the provided select tags.
    /// </summary>
    /// <param name="selectTags">The select tags to initialize the new instance with.</param>
    /// <returns>A new concrete <see cref="SelectTagsBase"/> instance.</returns>
    protected abstract SelectTagsBase New(params IEnumerable<SelectTagBase> selectTags);

    /// <summary>
    /// Creates a dialect-specific select tag for the requested model property.
    /// </summary>
    protected abstract SelectTagBase NewSelectTag<T>(PropertyName propertyName, AliasName? aliasName = null) where T : class;

    /// <summary>
    /// Creates a dialect-specific select-tag collection for the requested model properties.
    /// </summary>
    protected abstract SelectTagsBase NewSelectTags<T>(params IEnumerable<PropertyName> properties) where T : class;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTagsBase"/> class.
    /// </summary>
    /// <param name="selectTags">The select tags to include in this instance.</param>
    protected SelectTagsBase(params IEnumerable<SelectTagBase> selectTags) =>
        _selectTags = selectTags.Materialize(NullOptionsEnum.Exception);

    /// <summary>
    /// Indicates whether this instance contains any select tags.
    /// </summary>
    public bool Any() =>
        _selectTags.Any();

    /// <summary>
    /// Indicates whether this instance contains no select tags.
    /// </summary>
    public bool Empty() =>
        Any() is false;

    /// <summary>
    /// Gets all distinct <see cref="TableTag"/> values referenced by the contained select tags.
    /// </summary>
    internal IEnumerable<TableTag> GetTableTags() =>
        _selectTags
            .Select(select => select.ColumnTag.TableTag)
            .Distinct();

    /// <summary>
    /// Returns a new <see cref="SelectTagsBase"/> containing the current items plus the specified select tag.
    /// </summary>
    public virtual SelectTagsBase Append(SelectTagBase selectTag) =>
        New(_selectTags.Append(selectTag));

    /// <summary>
    /// Returns a new <see cref="SelectTagsBase"/> containing the current items plus a select tag for the specified property.
    /// </summary>
    public virtual SelectTagsBase Append<T>(PropertyName propertyName, AliasName? aliasName = null) where T : class =>
        Append(NewSelectTag<T>(propertyName, aliasName));

    /// <summary>
    /// Returns a new <see cref="SelectTagsBase"/> containing the current items plus a select tag for the specified property.
    /// </summary>
    [ExternalOnly]
    public virtual SelectTagsBase Append<T>(string property, string? aliasName = null) where T : class =>
        Append<T>(new PropertyName(property), AliasName.New(aliasName));

    /// <summary>
    /// Returns a new <see cref="SelectTagsBase"/> with the provided select-tag collection concatenated.
    /// </summary>
    public virtual SelectTagsBase Concat(SelectTagsBase selectTags) =>
        New(_selectTags.Concat(selectTags.All()));

    /// <summary>
    /// Returns a new <see cref="SelectTagsBase"/> with the provided select tags concatenated.
    /// </summary>
    public virtual SelectTagsBase Concat(params IEnumerable<SelectTagBase> selectTags) =>
        New(_selectTags.Concat(selectTags));

    /// <summary>
    /// Returns a new <see cref="SelectTagsBase"/> containing the current items plus select tags for the specified properties.
    /// </summary>
    public virtual SelectTagsBase Concat<T>(params IEnumerable<PropertyName> properties) where T : class =>
        Concat(NewSelectTags<T>(properties));

    /// <summary>
    /// Returns a new <see cref="SelectTagsBase"/> containing the current items plus select tags for the specified properties.
    /// </summary>
    [ExternalOnly]
    public virtual SelectTagsBase Concat<T>(params IEnumerable<string> properties) where T : class =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Returns all <see cref="SelectTagBase"/> items contained in this instance.
    /// </summary>
    public IEnumerable<SelectTagBase> All() =>
        _selectTags;

    public IEnumerator<SelectTagBase> GetEnumerator() =>
        _selectTags.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public IEnumerable<ISqlFragment> Flatten()
    {
        yield return this;
    }

    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
        [];

    /// <summary>
    /// Returns the SQL text for all select tags represented by this instance as a comma-separated list.
    /// </summary>
    public string ToSql(ISqlDialects dialect) =>
        string.Join(", ", _selectTags.Select(selectTag => selectTag.ToSql(dialect)));
}
