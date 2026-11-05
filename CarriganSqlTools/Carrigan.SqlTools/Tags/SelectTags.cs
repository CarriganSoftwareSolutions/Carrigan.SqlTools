using Carrigan.Core.Enums;
using Carrigan.Core.Exceptions;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a collection of <see cref="SelectTag"/> items and provides common collection
/// behavior for dialect-specific select-tag containers.
/// </summary>
public class SelectTags : ISqlFragment, IEnumerable<SelectTag>
{
    /// <summary>
    /// The immutable-style backing sequence of SELECT projection tags represented by this collection.
    /// </summary>
    internal readonly IEnumerable<SelectTag> _selectTags;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTags"/> class.
    /// </summary>
    /// <param name="selectTags">The select tags to include in this instance.</param>
    public SelectTags(params IEnumerable<SelectTag> selectTags) =>
        _selectTags = selectTags.Materialize(NullOptionsEnum.ArgumentNullException);

    /// <summary>
    /// Projects the result columns in this collection as columns qualified by the supplied table alias.
    /// </summary>
    /// <param name="tableAlias">The table alias used to qualify the projected result columns.</param>
    /// <returns>
    /// This instance when <paramref name="tableAlias"/> is <see langword="null"/>; otherwise, a new collection
    /// containing non-aliased select tags whose column expressions reference the result column names through the table alias.
    /// </returns>
    internal SelectTags FromTableAlias(TableAlias? tableAlias) =>
        tableAlias is null
            ? this
            : new(_selectTags.Select(selectTag => selectTag.FromTableAlias(tableAlias)));

    /// <summary>
    /// Indicates whether this instance contains any select tags.
    /// </summary>
    public bool Any() =>
        _selectTags.Any();

    /// <summary>
    /// Indicates whether this instance contains no select tags.
    /// </summary>
    public bool IsEmpty() =>
        Any() is false;

    /// <summary>
    /// Indicates whether any select tags in this instance contain column expressions.
    /// </summary>
    /// <returns>
    /// <c>true</c> if any select tag contains column expressions; otherwise, <c>false</c>.
    /// </returns>
    public bool HasColumns() =>
        _selectTags.Any(select => select.HasColumns());

    /// <summary>
    /// Indicates whether any select tags in this instance contain aggregate expressions.
    /// </summary>
    /// <returns>
    /// <c>true</c> if any select tag contains aggregate expressions; otherwise, <c>false</c>.
    /// </returns>
    public bool HasAggregates() =>
        _selectTags.Any(select => select.HasAggregates());

    /// <summary>
    /// Gets all non-aggregate column expressions referenced by the contained select tags.
    /// </summary>
    internal IEnumerable<SqlExpression> NonAggregateColumnExpressions =>
        _selectTags
            .SelectMany(selectTag => selectTag.NonAggregateColumnExpressions);

    /// <summary>
    /// Gets all distinct <see cref="TableTagBase"/> values referenced by the contained select tags.
    /// </summary>
    internal IEnumerable<TableTagBase> GetTableTags() =>
        _selectTags
            .SelectMany(static select => select.TableTags)
            .Distinct();

    /// <summary>
    /// Creates a new collection with the supplied item appended.
    /// </summary>
    /// <param name="selectTag">The select tag to append.</param>
    /// <returns>A new collection containing the existing tags followed by <paramref name="selectTag"/>.</returns>
    public SelectTags Append(SelectTag selectTag) =>
        new(_selectTags.Append(selectTag));

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    /// <returns>A new collection containing the existing tags followed by the supplied tags.</returns>
    public SelectTags Concat(SelectTags selectTags) =>
        new(_selectTags.Concat(selectTags._selectTags));

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    /// <returns>A new collection containing the existing tags followed by the supplied tags.</returns>
    public SelectTags Concat(params IEnumerable<SelectTag> selectTags) =>
        new(_selectTags.Concat(selectTags));

    /// <summary>
    /// Returns all <see cref="SelectTag"/> items contained in this instance.
    /// </summary>
    public IEnumerable<SelectTag> All() =>
        _selectTags;

    /// <summary>
    /// Returns an enumerator for the contained select tags.
    /// </summary>
    /// <returns>An enumerator for the contained select tags.</returns>
    public IEnumerator<SelectTag> GetEnumerator() =>
        _selectTags.GetEnumerator();

    /// <summary>
    /// Returns an enumerator for the contained select tags.
    /// </summary>
    /// <returns>
    /// An enumerator for the contained select tags.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A flattened sequence of SQL fragments that render this tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        _selectTags.JoinFragments(ISqlFragment.CommaSpace).SelectMany(fragment => fragment.Flatten(dialect));

    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>The SQL fragment parameters required to render this tag.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        _selectTags.SelectMany(select => select.GetSqlFragmentParameters(dialect));

    /// <summary>
    /// Returns the SQL text for all select tags represented by this instance as a comma-separated list.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render the fragment.</param>
    public string ToSql(ISqlDialects dialect) =>
        Flatten(dialect).ToSql(dialect);


    /// <summary>
    /// Creates a SELECT tag collection containing a single projection tag.
    /// </summary>
    /// <param name="selectTag">The SELECT projection tag to place in the collection.</param>
    /// <returns>A new collection containing <paramref name="selectTag"/>.</returns>
    public static implicit operator SelectTags(SelectTag selectTag) =>
        new(selectTag);


    public SelectTag GetByExpression(SqlExpression sqlExpression) =>
        _selectTags
            .Where(select => select.SqlExpression == sqlExpression)
            .Single();

    [ExternalOnly]
    public SelectTag? GetByColumnAlias(string columnAlias) =>
        GetByColumnAlias(new ColumnAlias(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(columnAlias)));

    public SelectTag? GetByColumnAlias(ColumnAlias columnAlias) =>
        GetByColumnAlias(new ColumnAliasTag(columnAlias ?? throw new ArgumentNullException(nameof(columnAlias))));

    protected SelectTag? GetByColumnAlias(ColumnAliasTag columnAliasTag) =>
        _selectTags
            .Where(select => select.AliasTag == columnAliasTag)
            .SingleOrDefault();

    [ExternalOnly]
    public SelectTag? GetByResultColumn(string resultColumnName) =>
        GetByResultColumn(new ResultColumnName(ArgumentNullOrWhiteSpaceException.ThrowIfNullOrWhiteSpace(resultColumnName)));

    public SelectTag? GetByResultColumn(ResultColumnName resultColumnName) =>
        _selectTags
            .Where(select => select.ResultColumnName == (resultColumnName ?? throw new ArgumentNullException(nameof(resultColumnName))))
            .SingleOrDefault();
}
