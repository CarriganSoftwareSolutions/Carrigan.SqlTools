using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;

namespace Carrigan.SqlTools.Tags;
//TODO: Proof read Documentation and unit test.

/// <summary>
/// Contains multiple <see cref="SelectTag"/>
/// </summary>
public class SelectTags : ISelectTags
{

    private readonly IEnumerable<SelectTag> _selectTags;

    /// <summary>
    /// A constructor that builds the instance with an enumeration of <see cref="SelectTag"/>
    /// </summary>
    /// <param name="selectTags"></param>
    public SelectTags(params IEnumerable<SelectTag> selectTags) =>
        _selectTags = selectTags;

    //Proof read documentation, unit tests
    /// <summary>
    /// Determines if this instance contains any actual SelectTags
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </summary>
    /// <returns>
    /// True if this instance contains any actual SelectTags
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </returns>
    public bool Any() =>
        _selectTags.Any();

    /// <summary>
    /// Determines if this instance contains no SelectTags
    /// For SelectTags, this will be true if the underlying Enumeration is empty.
    /// </summary>
    /// <returns>
    /// Determines if this instance contains no SelectTags
    /// For SelectTags, this will be true if the underlying Enumeration is empty.
    /// </returns>
    public bool Empty() =>
        Any() is false;

    /// <summary>
    /// Get all SelectTags associated with the instance, as a string.
    /// For SelectTags this will be a comma separated list.
    /// </summary>
    /// <returns>
    /// All SelectTags associated with the instance, as a string. 
    /// For SelectTags this will be a comma separated list.
    /// </returns>
    public string GetSelects() =>
        string.Join(", ", _selectTags);

    /// <summary>
    /// Get all TableTags associated with the instance.
    /// For SelectTags this will be multiple TableTags.
    /// </summary>
    /// <returns>
    /// All TableTags associated with the instance.
    /// For SelectTags this will be multiple TableTags.
    /// </returns>
    public IEnumerable<TableTag> GetTableTags() =>
        _selectTags
            .SelectMany(select => select.GetTableTags());

    /// <summary>
    /// Create a new <see cref="SelectTag"/> and append it to a copy of the underlying enumeration.
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="property">Provided property name</param>
    /// <param name="aliasName">provided alias name</param>
    /// <returns>
    /// Create a new <see cref="SelectTag"/> and append it to a copy of the underlying enumeration.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when the property name provided does not exist for T or is ineligible to model a column.
    /// </exception>
    internal SelectTags Append<T>(PropertyName property, AliasName? aliasName = null) =>
        new
        (
            _selectTags.Append
            (
                new SelectTag
                (
                    SqlToolsReflectorCache<T>
                        .GetColumnsFromProperties(property)
                        .FirstOrDefault()
                        ?.ColumnTag  ?? throw new InvalidPropertyException<T>(property),
                    aliasName is not null ? new AliasTag(aliasName) : null
                )
            )
        );

    /// <summary>
    /// Create a new <see cref="SelectTag"/> and append it to a copy of the underlying enumeration.
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="property">Provided property name</param>
    /// <param name="aliasName">provided alias name</param>
    /// <returns>
    /// Create a new <see cref="SelectTag"/> and append it to a copy of the underlying enumeration.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when the property name provided does not exist for T or is ineligible to model a column.
    /// </exception>
    [ExternalOnly]
    public SelectTags Append<T>(string property, string? aliasName = null)
    {
        AliasName? alias = AliasName.New(aliasName);
        if (alias.IsNotNullOrEmpty() && SqlIdentifierPattern.Fails(alias))
            throw new InvalidSqlIdentifierException(alias); //TODO: unit test

        return Append<T>(new PropertyName(property), alias);
    }


    /// <summary>
    /// Return a new <see cref="SelectTags"/> by concatenating <see cref="SelectTag"/> that correspond
    /// to the provided properties.
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="properties">Provided properties name</param>
    /// <returns>
    /// Return a new <see cref="SelectTags"/> by concatenating <see cref="SelectTag"/> that correspond
    /// to the provided properties.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when the property name provided does not exist for T or is ineligible to model a column.
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
    /// Return a new <see cref="SelectTags"/> by concatenating <see cref="SelectTag"/> that correspond
    /// to the provided properties.
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="properties">Provided properties name</param>
    /// <returns>
    /// Return a new <see cref="SelectTags"/> by concatenating <see cref="SelectTag"/> that correspond
    /// to the provided properties.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when the property name provided does not exist for T or is ineligible to model a column.
    /// </exception>
    [ExternalOnly]
    public SelectTags Concat<T>(params IEnumerable<string> properties) =>
        Concat<T>(properties.Select(name => new PropertyName(name)));


    internal static SelectTags Get<T>(PropertyName properties, AliasName? aliasName = null) =>
        new
        (
            new SelectTag
            (
                SqlToolsReflectorCache<T>
                    .GetColumnsFromProperties(properties)
                    .FirstOrDefault()
                    ?.ColumnTag ?? throw new InvalidPropertyException<T>(properties),
                aliasName is not null ? new AliasTag(aliasName) : null
            )
        );

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get a new Select Tags with a new Select Tag based of the property and alias provided.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property">Property provided</param>
    /// <param name="aliasName">Alias provided</param>
    /// <returns>
    /// A new Select Tags with a new Select Tag based of the property and alias provided.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Throws in the property is invalid for class T, or ineligible to model a column.
    /// </exception>
    [ExternalOnly]
    public static SelectTags Get<T>(string property, string? aliasName = null)
    {
        AliasName? alias = AliasName.New(aliasName);
        if (alias.IsNotNullOrEmpty() && SqlIdentifierPattern.Fails(alias))
            throw new InvalidSqlIdentifierException(alias); //TODO: unit test

        return Get<T>(new PropertyName(property), alias);
    }

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get new a <see cref="SelectTags"/> for multiple existing <see cref="SelectTag"/>s based of the properties provided.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="properties">Properties provided</param>
    /// <returns>
    /// A new  <see cref="SelectTags"/> for multiple existing <see cref="SelectTag"/>s based of the properties provided.
    /// </returns>
    public static SelectTags GetMany<T>(params IEnumerable<PropertyName> properties) =>
        new
        (
            SqlToolsReflectorCache<T>
                    .GetColumnsFromProperties(properties)
                    .Select(column => column.SelectTag)
        );

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get new a <see cref="SelectTags"/> for multiple existing <see cref="SelectTag"/>s based of the properties provided.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="properties">Properties provided</param>
    /// <returns>
    /// A new  <see cref="SelectTags"/> for multiple existing <see cref="SelectTag"/>s based of the properties provided.
    /// </returns>
    [ExternalOnly]
    public static SelectTags GetMany<T>(params IEnumerable<string> properties) =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));
}
