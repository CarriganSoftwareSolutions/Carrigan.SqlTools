using Carrigan.Core.Enums;
using Carrigan.Core.Exceptions;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using System.Collections;
using System.Collections.ObjectModel;

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

    internal readonly TableAlias? _tableAlias;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTags"/> class.
    /// </summary>
    /// <param name="selectTags">The select tags to include in this instance.</param>
    public SelectTags(params IEnumerable<SelectTag> selectTags) =>
        _selectTags = selectTags.Materialize(NullOptionsEnum.ArgumentException);

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTags"/> class.
    /// </summary>
    /// <param name="tableAlias">
    /// The table alias used to qualify the projected result columns.
    /// </param>
    /// <param name="selectTags">The select tags to include in this instance.</param>
    public SelectTags(string tableAlias, params IEnumerable<SelectTag> selectTags) : this(TableAlias.New(tableAlias), selectTags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTags"/> class.
    /// </summary>
    /// <param name="tableAlias">
    /// The table alias used to qualify the projected result columns.
    /// </param>
    /// <param name="selectTags">The select tags to include in this instance.</param>
    public SelectTags(TableAlias? tableAlias, params IEnumerable<SelectTag> selectTags) : this(selectTags) =>
        _tableAlias = tableAlias;


    /// <summary>
    /// Projects the result columns in this collection as columns qualified by the supplied table alias.
    /// </summary>
    /// <returns>
    /// This instance when it <see cref="TableAlias"/> is <see langword="null"/>; otherwise, a new collection
    /// containing non-aliased select tags whose column expressions reference the result column names through the table alias.
    /// </returns>
    internal SelectTags ResultReferences() =>
        _tableAlias is null
            ? this
            : new(_selectTags.Select(selectTag => selectTag.SelectThis(_tableAlias)));

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
    //TODo: now that columns can be references a table alias / coloumn alias, we need to make sure this doesn't break the logic to determine if a a column is valid when looking at aggregatres
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
    /// Gets all distinct <see cref="TableTag"/> values referenced by the contained select tags.
    /// </summary>
    internal IEnumerable<TableTag> GetTableTags() =>
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

    /// <summary>
    /// Creates a SELECT tag collection containing multiple projection tags.
    /// </summary>
    /// <param name="selectTags">The SELECT projection tag to place in the collection.</param>
    /// <returns>A new collection containing <paramref name="selectTags"/>.</returns>
    public static implicit operator SelectTags(SelectTag[] selectTags) =>
        new(selectTags);

    /// <summary>
    /// Creates a SELECT tag collection containing multiple projection tags.
    /// </summary>
    /// <param name="selectTags">The SELECT projection tag to place in the collection.</param>
    /// <returns>A new collection containing <paramref name="selectTags"/>.</returns>
    public static implicit operator SelectTags(List<SelectTag> selectTags) =>
        new(selectTags);

    /// <summary>
    /// Creates a SELECT tag collection containing multiple projection tags.
    /// </summary>
    /// <param name="selectTags">The SELECT projection tag to place in the collection.</param>
    /// <returns>A new collection containing <paramref name="selectTags"/>.</returns>
    public static implicit operator SelectTags(Collection<SelectTag> selectTags) =>
        new(selectTags);


    public SelectTag GetByExpression(IEnumerable<SqlExpression> sqlExpressions) =>
        _selectTags
            .Materialize(NullOptionsEnum.ArgumentException)
            .Where(select => sqlExpressions.Contains(select.SqlExpression))
            .Single();

    [ExternalOnly]
    public IEnumerable<SelectTag> GetByColumnAlias(IEnumerable<string> columnAliases) =>
        GetByColumnAlias
        (
            columnAliases
                .Materialize(NullOrWhiteSpaceOptionsEnum.ArgumentException)
                .Select(columnAlias => new ColumnAlias(columnAlias))
        );
    protected IEnumerable<SelectTag> GetByColumnAlias(IEnumerable<ColumnAlias> columnAliasTags) =>
        _selectTags
            .Where(select => select.ColumnAlias.IsNotNullOrWhiteSpace())
            .Where(select => columnAliasTags.Contains(select.ColumnAlias));

    [ExternalOnly]
    public IEnumerable<SelectTag> GetByResultColumn(IEnumerable<string> resultColumnNames) =>
        GetByResultColumn
        (
            resultColumnNames
                .Materialize(NullOrWhiteSpaceOptionsEnum.ArgumentException)
                .Select(resultColumnName => new ResultColumnName(resultColumnName))
        );

    public IEnumerable<SelectTag> GetByResultColumn(IEnumerable<ResultColumnName> resultColumnName) =>
        _selectTags
            .Where(select => resultColumnName.Contains(select.ResultColumnName));
}
