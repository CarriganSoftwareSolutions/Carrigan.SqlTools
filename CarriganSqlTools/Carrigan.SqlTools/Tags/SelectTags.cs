using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a collection of <see cref="SelectTag"/> items, providing utilities to
/// append, concatenate, and render them for a SELECT list.
/// </summary>
public class SelectTags
{

    private readonly IEnumerable<SelectTag> _selectTags;

    /// <summary>
    /// Represents a collection of <see cref="SelectTag"/> items, providing utilities to
    /// append, concatenate, and render them for a SELECT list.
    /// </summary>
    public SelectTags(params IEnumerable<SelectTag> selectTags) =>
        _selectTags = selectTags;

    /// <summary>
    /// Indicates whether this instance contains any select tags.
    /// For <see cref="SelectTags"/>, this returns <c>true</c> when the underlying enumeration is not empty.
    /// </summary>
    /// <returns><c>true</c> if one or more items exist; otherwise, <c>false</c>.</returns>
    public bool Any() =>
        _selectTags.Any();

    /// <summary>
    /// Indicates whether this instance contains no select tags.
    /// For <see cref="SelectTags"/>, this returns <c>true</c> when the underlying enumeration is empty.
    /// </summary>
    /// <returns><c>true</c> if no items exist; otherwise, <c>false</c>.</returns>
    public bool Empty() =>
        Any() is false;

    /// <summary>
    /// Returns the SQL text for all select tags represented by this instance as a comma-separated list.
    /// </summary>
    /// <returns>A comma-separated list of the contained <see cref="SelectTag"/> SQL fragments.</returns>
    public string ToSql(ISqlDialects dialect) =>
        string.Join(", ", _selectTags.Select(selectTag => selectTag.ToSql(dialect)));

    /// <summary>
    /// Gets all distinct <see cref="TableTag"/> values referenced by the contained select tags.
    /// </summary>
    /// <returns>An enumeration of unique <see cref="TableTag"/> values.</returns>
    internal IEnumerable<TableTag> GetTableTags() =>
        _selectTags
            .Select(select => select.ColumnTag.TableTag)
            .Distinct();

    /// <summary>
    /// Returns a new <see cref="SelectTags"/> containing the current items plus the specified <paramref name="selectTag"/>.
    /// </summary>
    /// <param name="selectTag">The <see cref="SelectTag"/> to append.</param>
    /// <returns>A new <see cref="SelectTags"/> with <paramref name="selectTag"/> appended.</returns>
    public SelectTags Append(SelectTag selectTag) =>
        new(_selectTags.Append(selectTag));

    /// <summary>
    /// Returns a new <see cref="SelectTags"/> containing the current items plus a new <see cref="SelectTag"/>
    /// based on the provided <paramref name="property"/> and optional <paramref name="aliasName"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines <paramref name="property"/>.</typeparam>
    /// <param name="property">The property to project.</param>
    /// <param name="aliasName">An optional alias override; if provided, it must be a valid SQL identifier.</param>
    /// <returns>A new <see cref="SelectTags"/> with the additional projection.</returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="property"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    public SelectTags Append<T>(PropertyName property, AliasName? aliasName = null) =>
        Append (SelectTag.Get<T>(property, aliasName));


    /// <summary>
    /// Returns a new <see cref="SelectTags"/> containing the current items plus a new <see cref="SelectTag"/>
    /// based on the provided <paramref name="property"/> and optional <paramref name="aliasName"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines <paramref name="property"/>.</typeparam>
    /// <param name="property">The property name to project.</param>
    /// <param name="aliasName">An optional alias override; if provided, it must be a valid SQL identifier.</param>
    /// <returns>A new <see cref="SelectTags"/> with the additional projection.</returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="property"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/> is provided but fails SQL identifier validation.
    /// </exception>
    [ExternalOnly]
    public SelectTags Append<T>(string property, string? aliasName = null) =>
        Append<T>(new PropertyName(property), AliasName.New(aliasName));

    /// <summary>
    /// Return a new <see cref="SelectTags"/> with <paramref name="selectTags"/> concatenated.
    /// </summary>
    /// <param name="selectTags">Provided <see cref="IEnumerable{ISelectTags}"/></param>
    /// <returns>
    /// Return a new <see cref="SelectTags"/> with <paramref name="selectTags"/> concatenated.
    /// to the provided properties.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when the property name provided does not exist for T or is ineligible to model a column.
    /// </exception>
    public SelectTags Concat(params IEnumerable<SelectTags> selectTags) =>
        new (_selectTags.Concat(selectTags.SelectMany(selects => selects.All())));


    /// <summary>
    /// Returns a new <see cref="SelectTags"/> by concatenating projections for the specified <paramref name="properties"/> on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines the properties.</typeparam>
    /// <param name="properties">One or more property names to project.</param>
    /// <returns>
    /// A new <see cref="SelectTags"/> that contains all existing items plus the projections for <paramref name="properties"/>.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when any provided property is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    public SelectTags Concat<T>(params IEnumerable<PropertyName> properties) =>
        new
        (
            _selectTags.Concat
            (
                SqlToolsReflectorCache<T>
                    .GetColumnsFromProperties(properties)
                    .Select(column => column.SelectTag)
            )            
        );

    /// <summary>
    /// Returns a new <see cref="SelectTags"/> by concatenating projections for the specified <paramref name="properties"/> on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines the properties.</typeparam>
    /// <param name="properties">One or more property names to project.</param>
    /// <returns>
    /// A new <see cref="SelectTags"/> that contains all existing items plus the projections for <paramref name="properties"/>.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when any provided property is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    public SelectTags Concat<T>(params IEnumerable<string> properties) =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Creates a new <see cref="SelectTags"/> containing a single <see cref="SelectTag"/> built from
    /// the specified <paramref name="propertyName"/> and optional <paramref name="aliasName"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines <paramref name="propertyName"/>.</typeparam>
    /// <param name="propertyName">The property to project.</param>
    /// <param name="aliasName">An optional alias override; if provided, it must be a valid SQL identifier.</param>
    /// <returns>
    /// A new <see cref="SelectTags"/> containing a single projection for <paramref name="propertyName"/>.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/> is provided but fails SQL identifier validation.
    /// </exception>
    public static SelectTags Get<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        SqlToolsReflectorCache<T>
            .GetSelectTag(propertyName, aliasName);

    /// <summary>
    /// Creates a new <see cref="SelectTags"/> containing a single <see cref="SelectTag"/> built from
    /// the specified <paramref name="propertyName"/> and optional <paramref name="aliasName"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines <paramref name="propertyName"/>.</typeparam>
    /// <param name="propertyName">The property name to project.</param>
    /// <param name="aliasName">An optional alias override; if provided, it must be a valid SQL identifier.</param>
    /// <returns>
    /// A new <see cref="SelectTags"/> containing a single projection for <paramref name="propertyName"/>.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/> is provided but fails SQL identifier validation.
    /// </exception>
    [ExternalOnly]
    public static SelectTags Get<T>(string propertyName, string? aliasName = null) =>
        Get<T>(new PropertyName(propertyName), AliasName.New(aliasName));

    /// <summary>
    /// Creates a new <see cref="SelectTags"/> containing the existing <see cref="SelectTag"/> values
    /// that correspond to the specified <paramref name="properties"/> on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines the properties.</typeparam>
    /// <param name="properties">One or more property names to project.</param>
    /// <returns>
    /// A new <see cref="SelectTags"/> for the projections of <paramref name="properties"/>.
    /// </returns>
    public static SelectTags GetMany<T>(params IEnumerable<PropertyName> properties) =>
        new
        (
            SqlToolsReflectorCache<T>
                    .GetColumnsFromProperties(properties)
                    .Select(column => column.SelectTag)
        );

    /// <summary>
    /// Creates a new <see cref="SelectTags"/> containing the existing <see cref="SelectTag"/> values
    /// that correspond to the specified <paramref name="properties"/> on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines the properties.</typeparam>
    /// <param name="properties">One or more property names to project.</param>
    /// <returns>
    /// A new <see cref="SelectTags"/> for the projections of <paramref name="properties"/>.
    /// </returns>
    [ExternalOnly]
    public static SelectTags GetMany<T>(params IEnumerable<string> properties) =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Returns all <see cref="SelectTag"/> items contained in this instance.
    /// </summary>
    /// <returns>An enumeration of <see cref="SelectTag"/> values.</returns>
    public IEnumerable<SelectTag> All() => 
        _selectTags;


    public static implicit operator SelectTags(SelectTag join) => new(join);
}
