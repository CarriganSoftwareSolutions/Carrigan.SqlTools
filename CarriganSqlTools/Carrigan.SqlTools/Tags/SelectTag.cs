using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;

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
public class SelectTag : SelectTagsBase, IComparable<SelectTag>, IEquatable<SelectTag>, IEqualityComparer<SelectTag>
{
    /// <summary>
    /// The SQL text of the select item, e.g., <c>[Schema].[Table].[Column] AS [Alias]</c>.
    /// </summary>
    private readonly string _selectTag;

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
    {
        if (aliasTag is null)
            _selectTag = columnTag;
        else
            _selectTag = $"{columnTag} AS {aliasTag}";

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
    /// <param name="property">The name of the property to project.</param>
    /// <param name="aliasName">
    /// An optional alias name override. If provided, it must be a valid SQL identifier.
    /// </param>
    /// <returns>
    /// A new <see cref="SelectTag"/> representing the requested property projection.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="property"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/> is provided but fails SQL identifier validation.
    /// </exception>
    public static SelectTag Get<T>(PropertyName property, AliasName? aliasName = null)
    {
        if (aliasName.IsNotNullOrEmpty() && SqlIdentifierPattern.Fails(aliasName))
            throw new InvalidSqlIdentifierException(aliasName);
        ColumnInfo columnInfo =
            SqlToolsReflectorCache<T>
                .GetColumnsFromProperties(property)
                .FirstOrDefault() ?? throw new InvalidPropertyException<T>(property);
        return new(columnInfo.ColumnTag, AliasTag.New(aliasName ?? columnInfo.AliasName));
    }

    /// <summary>
    /// Creates a new <see cref="SelectTag"/> for the specified property on <typeparamref name="T"/>,
    /// using the provided alias if supplied; otherwise defaults to the property's alias attribute (if any).
    /// </summary>
    /// <typeparam name="T">The entity/model type containing the property.</typeparam>
    /// <param name="property">The property name to project.</param>
    /// <param name="aliasName">
    /// An optional alias name override. If provided, it must be a valid SQL identifier.
    /// </param>
    /// <returns>
    /// A new <see cref="SelectTag"/> representing the requested property projection.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="property"/> is not a valid, mappable column property for <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown when <paramref name="aliasName"/> is provided but fails SQL identifier validation.
    /// </exception>
    [ExternalOnly]
    public static SelectTag Get<T>(string property, string? aliasName = null)
    {
        AliasName? alias = AliasName.New(aliasName);

        return Get<T>(new PropertyName(property), alias);
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

    /// <summary>
    /// Implicitly converts a <see cref="SelectTag"/> to its SQL string representation.
    /// </summary>
    /// <param name="value">The <see cref="SelectTag"/> to convert.</param>
    /// <returns>
    /// The SQL text for this select item, e.g., <c>[Schema].[Table].[Column] AS [Alias]</c> or <c>[Schema].[Table].[Column]</c>.
    /// </returns>
    public static implicit operator string(SelectTag value)
        => value._selectTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="SelectTag"/> instance.
    /// </summary>
    /// <returns>
    /// The SQL text for this select item, e.g., <c>[Schema].[Table].[Column] AS [Alias]</c>.
    /// </returns>
    public override string ToString()
        => this;


    /// <summary>
    /// Compares this instance with another <see cref="SelectTag"/> to determine sort order.
    /// </summary>
    /// <param name="other">The other <see cref="SelectTag"/> to compare.</param>
    /// <returns>
    /// A signed integer: <c>0</c> if equal; less than <c>0</c> if this instance precedes
    /// <paramref name="other"/>; greater than <c>0</c> if it follows.
    /// </returns>
    /// <remarks>Comparison is case-insensitive via <see cref="StringComparison.OrdinalIgnoreCase"/>.</remarks>
    public int CompareTo(SelectTag? other)
    {
        if (other is null) return 1;
        return string.Compare(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether this instance is equal to another <see cref="SelectTag"/>.
    /// </summary>
    /// <param name="other">The other <see cref="SelectTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if both represent the same SQL text (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(SelectTag? other)
    {
        if (other is null) return false;
        return string.Equals(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to this <see cref="SelectTag"/>.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is a <see cref="SelectTag"/> with equal SQL text (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        obj is SelectTag ct && Equals(ct);

    /// <summary>
    /// Returns a hash code for this <see cref="SelectTag"/> instance.
    /// </summary>
    /// <returns>
    /// An integer hash code consistent with the case-insensitive equality semantics.
    /// </returns>
    public override int GetHashCode() =>
        _selectTag.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="SelectTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="SelectTag"/> to compare.</param>
    /// <param name="y">The second <see cref="SelectTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if both represent the same SQL text (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(SelectTag? x, SelectTag? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Equals(y);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="SelectTag"/> instance.
    /// </summary>
    /// <param name="obj">The <see cref="SelectTag"/> for which to compute a hash code.</param>
    /// <returns>An integer hash code for <paramref name="obj"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c>.</exception>
    public int GetHashCode(SelectTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="SelectTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="SelectTag"/> to compare.</param>
    /// <param name="right">The second <see cref="SelectTag"/> to compare.</param>
    /// <returns><c>true</c> if both are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(SelectTag? left, SelectTag? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }


    /// <summary>
    /// Determines whether two <see cref="SelectTag"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="SelectTag"/> to compare.</param>
    /// <param name="right">The second <see cref="SelectTag"/> to compare.</param>
    /// <returns><c>true</c> if they differ; otherwise, <c>false</c>.</returns>
    public static bool operator !=(SelectTag? left, SelectTag? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns the SQL text for all select tags represented by this instance.
    /// For a single <see cref="SelectTag"/>, this is simply its own SQL text.
    /// </summary>
    /// <returns>The SQL text represented by this instance.</returns>
    public override string ToSql() =>
        this;

    /// <summary>
    /// Gets all distinct <see cref="TableTag"/> values referenced by this select tag.
    /// For a single <see cref="SelectTag"/>, this returns its table tag.
    /// </summary>
    /// <returns>An enumeration containing the single <see cref="TableTag"/> used by this instance.</returns>
    internal override IEnumerable<TableTag> GetTableTags() => 
        [ColumnTag.TableTag];

    /// <summary>
    /// Indicates whether this instance represents any select tags.
    /// For a single <see cref="SelectTag"/>, this always returns <c>true</c>.
    /// </summary>
    /// <returns><c>true</c>.</returns>
    public override bool Any() =>
        true;

    /// <summary>
    /// Indicates whether this instance represents no select tags.
    /// For a single <see cref="SelectTag"/>, this always returns <c>false</c>.
    /// </summary>
    /// <returns><c>false</c>.</returns>
    public override bool Empty() =>
        false;

    /// <summary>
    /// Returns all select tags represented by this instance.
    /// For a single <see cref="SelectTag"/>, this returns itself.
    /// </summary>
    /// <returns>An enumeration containing this <see cref="SelectTag"/>.</returns>
    public override IEnumerable<SelectTag> All() =>
        [this];
}
