using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a SELECT projection tag for a single column, consisting of a fully qualified
/// column identifier (e.g., <c>[Schema].[Table].[Column]</c>) and an optional alias
/// (e.g., <c>AS [Alias]</c>).
/// </summary>
/// <remarks>
/// <para>
/// Instances are typically created from reflection-derived <see cref="Tags.ColumnTag"/> values,
/// optionally combined with an <see cref="Tags.AliasTag"/>. This tag is used by SQL generators to
/// render the SELECT list for a query.
/// </para>
/// <para>
/// Implements <see cref="IComparable{SelectTag}"/>, <see cref="IEquatable{SelectTag}"/>,
/// and <see cref="IEqualityComparer{SelectTag}"/> for use in ordered and hashed collections.
/// </para>
/// </remarks>
public class SelectTag : StringWrapper, ISqlFragment
{

    /// <summary>
    /// The fully qualified column identifier for this select item.
    /// </summary>
    internal readonly ColumnTag ColumnTag;
    /// <summary>
    /// The optional alias applied to this select item.
    /// </summary>
    internal readonly AliasTag? AliasTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class.
    /// </summary>
    /// <param name="columnTag">
    /// The column identifier to select. Must represent a valid column.
    /// </param>
    /// <param name="aliasTag">
    /// The optional alias (i.e., <c>AS [Alias]</c>) to apply to the selected column.
    /// </param>
    internal SelectTag(ColumnTag columnTag, AliasTag? aliasTag = null) 
        : base(aliasTag.IsNullOrWhiteSpace() ? columnTag : $"{columnTag} AS {aliasTag}")
    {
        ColumnTag = columnTag;
        AliasTag = aliasTag;
    }

    /// <summary>
    /// Gets the expected result set column name for this projection, choosing the alias
    /// (when present) or the underlying column name.
    /// </summary>
    internal ResultColumnName ResultColumnName =>
        new (AliasTag?.ToString() ?? ColumnTag.ColumnName);


    /// <summary>
    /// Creates a new <see cref="SelectTag"/> for the specified property on <typeparamref name="T"/>,
    /// using the provided alias if supplied; otherwise defaults to the property's alias attribute (if any).
    /// </summary>
    /// <typeparam name="T">The entity/model type containing the property.</typeparam>
    /// <param name="propertyName">The name of the property to project.</param>
    /// <param name="aliasName">
    /// An optional alias name override. If provided, it must be a valid SQL identifier.
    /// </param>
    /// <returns>
    /// A new <see cref="SelectTag"/> representing the requested property projection.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/> is provided but fails SQL identifier validation.
    /// </exception>
    public static SelectTag Get<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        SqlToolsReflectorCache<T>.GetSelectTag(propertyName, aliasName);

    /// <summary>
    /// Creates a new <see cref="SelectTag"/> for the specified property on <typeparamref name="T"/>,
    /// using the provided alias if supplied; otherwise defaults to the property's alias attribute (if any).
    /// </summary>
    /// <typeparam name="T">The entity/model type containing the property.</typeparam>
    /// <param name="propertyName">The property name to project.</param>
    /// <param name="aliasName">
    /// An optional alias name override. If provided, it must be a valid SQL identifier.
    /// </param>
    /// <returns>
    /// A new <see cref="SelectTag"/> representing the requested property projection.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/> is provided but fails SQL identifier validation.
    /// </exception>
    [ExternalOnly]
    public static SelectTag Get<T>(string propertyName, string? aliasName = null)
    {
        AliasName? alias = AliasName.New(aliasName);

        return Get<T>(new PropertyName(propertyName), alias);
    }

    /// <summary>
    /// Returns existing <see cref="SelectTag"/> instances for the provided property names on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines the properties.</typeparam>
    /// <param name="properties">One or more property names to project.</param>
    /// <returns>
    /// An enumeration of <see cref="SelectTag"/> objects corresponding to the specified properties.
    /// </returns>
    public static IEnumerable<SelectTag> GetMany<T>(params IEnumerable<PropertyName> properties) =>
        SqlToolsReflectorCache<T>
            .GetColumnsFromProperties(properties)
            .Select(column => column.SelectTag);

    /// <summary>
    /// Returns existing <see cref="SelectTag"/> instances for the provided property names on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type that defines the properties.</typeparam>
    /// <param name="properties">One or more property names to project.</param>
    /// <returns>
    /// An enumeration of <see cref="SelectTag"/> objects corresponding to the specified properties.
    /// </returns>
    [ExternalOnly]
    public static IEnumerable<SelectTag> GetMany<T>(params IEnumerable<string> properties) =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Returns all <see cref="SelectTag"/> projections for every mappable column on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The entity/model type to project.</typeparam>
    /// <returns>All <see cref="SelectTag"/> instances for <typeparamref name="T"/>.</returns>
    public static IEnumerable<SelectTag> GetAll<T>() =>
        SqlToolsReflectorCache<T>
            .ColumnInfo
            .Select(column => column.SelectTag);

    public IEnumerable<ISqlFragment> Flatten()
    {
        yield return this;
    }
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
        [];

    public string ToSql(ISqlDialects dialect) =>
        AliasTag is null ? ColumnTag.ToSql(dialect) : $"{ColumnTag.ToSql(dialect)} AS {AliasTag.ToSql(dialect)}";

    public SelectTag WithNoAlias() =>
        new (ColumnTag);
}
