using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;
//TODO: proof read documentation
//TODO: Unit tests
public class SelectTag : IComparable<SelectTag>, IEquatable<SelectTag>, IEqualityComparer<SelectTag>, ISelectTags
{
    /// <summary>
    /// A string that represent the alias, <c>AS</c> clause.
    /// </summary>
    private readonly string _selectTag;

    internal readonly ColumnTag ColumnTag;
    internal readonly AliasTag? AliasTag;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class,
    /// which represents a fully qualified SQL column identifier
    /// in the form <c>[Schema].[Table].[Column]</c>.
    /// </summary>
    /// <param name="tableTag">
    /// The <see cref="TableTag"/> that identifies the table containing the column.
    /// </param>
    /// <param name="columnName">
    /// The name of the column. Must not be <c>null</c>, empty, or white space.
    /// </param>
    /// <param name="propertyInfo">
    /// The <see cref="PropertyInfo"/> associated with the column in the data model.
    /// </param>
    /// <param name="parameterTag">
    /// The <see cref="ParameterTag"/> used to represent the column as a SQL parameter.
    /// </param>
    internal SelectTag(ColumnTag column, AliasTag? aliasTag = null)
    {
        if (aliasTag is null)
            _selectTag = column;
        else
            _selectTag = $"{column} AS {aliasTag}";

        ColumnTag = column;
        AliasTag = aliasTag;
    }

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get a new Select Tag based of the property and alias provided.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property">Property provided</param>
    /// <param name="aliasName">Alias provided</param>
    /// <returns>
    /// A new Select Tag based of the property and alias provided.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Throws in the property is invalid for class T, or ineligible to model a column.
    /// </exception>
    internal static SelectTag Get<T>(PropertyName property, AliasName? aliasName = null) =>
        new
        (
            SqlToolsReflectorCache<T>
                .GetColumnsFromProperties(property)
                .FirstOrDefault()
                ?.ColumnTag ?? throw new InvalidPropertyException<T>(property),
            aliasName is not null ? new AliasTag(aliasName.Value) : null
        );

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get a new Select Tag based of the property and alias provided.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property">Property provided</param>
    /// <param name="aliasName">Alias provided</param>
    /// <returns>
    /// A new Select Tag based of the property and alias provided.
    /// </returns>
    /// <exception cref="InvalidPropertyException{T}">
    /// Throws in the property is invalid for class T, or ineligible to model a column.
    /// </exception>
    [ExternalOnly]
    public static SelectTag Get<T>(string property, string? aliasName = null)
    {
        if (aliasName.IsNotNullOrEmpty() && SqlIdentifierPattern.Fails(aliasName))
            throw new InvalidSqlIdentifierException(new AliasName(aliasName)); //TODO: unit test

        return Get<T>(new PropertyName(property), aliasName is not null ? new AliasName(aliasName) : null);
    }

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get a multiple existing Select Tags based of the properties provided.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="properties">Properties provided</param>
    /// <returns>
    /// A multiple existing Select Tags based of the properties provided.
    /// </returns>
    public static IEnumerable<SelectTag> GetMany<T>(params IEnumerable<PropertyName> properties) =>
        SqlToolsReflectorCache<T>
            .GetColumnsFromProperties(properties)  //TODO: It would be nice to get select directly from a property
            .Select(column => column.SelectTag); //TODO: It would be nice to skip this step.

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get a multiple existing Select Tags based of the properties provided.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="properties">Properties provided</param>
    /// <returns>
    /// A multiple existing Select Tags based of the properties provided.
    /// </returns>
    //TODO: Documentation, unit testing
    [ExternalOnly]
    public static IEnumerable<SelectTag> GetMany<T>(params IEnumerable<string> properties) =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// Get all Select Tags associated with T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    /// All Select Tags associated with T.
    /// </returns>
    public static IEnumerable<SelectTag> GetAll<T>() =>
        SqlToolsReflectorCache<T>
            .ColumnInfo
            .Select(column => column.SelectTag);

    /// <summary>
    /// Implicitly converts a <see cref="SelectTag"/> to its SQL string representation
    /// in the form <c>[Schema].[Table].[Column]</c>.
    /// </summary>
    /// <param name="value">The <see cref="SelectTag"/> to convert.</param>
    /// <returns>
    /// A SQL string that fully qualifies the column name, including schema and table if defined.
    /// </returns>
    public static implicit operator string(SelectTag value)
        => value._selectTag;

    /// <summary>
    /// Returns the SQL string representation of this <see cref="SelectTag"/> instance,
    /// equivalent to the result of the implicit conversion to <see cref="string"/>.
    /// </summary>
    /// <returns>
    public override string ToString()
        => this;

    /// <summary>
    /// Compares this <see cref="SelectTag"/> to another instance and returns a value
    /// that indicates their relative sort order.
    /// </summary>
    /// <param name="other">
    /// The <see cref="SelectTag"/> to compare with the current instance.
    /// </param>
    /// <returns>
    /// A signed integer that indicates the relative order of the two objects:
    /// <c>0</c> if they are equal, a negative value if this instance precedes
    /// <paramref name="other"/>, and a positive value if this instance follows
    /// <paramref name="other"/>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// If <paramref name="other"/> is <c>null</c>, this instance is considered greater.
    /// </remarks>
    public int CompareTo(SelectTag? other)
    {
        if (other is null) return 1;
        return string.Compare(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current <see cref="SelectTag"/> is equal to another
    /// <see cref="SelectTag"/> instance.
    /// </summary>
    /// <param name="other">
    /// The <see cref="SelectTag"/> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>true</c> if the two <see cref="SelectTag"/> instances represent the same column,
    /// ignoring case; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </remarks>
    public bool Equals(SelectTag? other)
    {
        if (other is null) return false;
        return string.Equals(this, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// <see cref="SelectTag"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is a <see cref="SelectTag"/> and
    /// represents the same column (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and delegates to
    /// <see cref="Equals(SelectTag?)"/>.
    /// </remarks>
    public override bool Equals(object? obj) =>
        obj is SelectTag ct && Equals(ct);

    /// <summary>
    /// Serves as the default hash function for the <see cref="SelectTag"/> class.
    /// </summary>
    /// <returns>
    /// An integer hash code for this <see cref="SelectTag"/>, computed in a manner
    /// consistent with the case-insensitive comparison used in <see cref="Equals(SelectTag?)"/>.
    /// </returns>
    public override int GetHashCode() =>
        _selectTag.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="SelectTag"/> instances are equal.
    /// </summary>
    /// <param name="x">The first <see cref="SelectTag"/> to compare.</param>
    /// <param name="y">The second <see cref="SelectTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="x"/> and <paramref name="y"/> represent the same column;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and uses
    /// <see cref="SelectTag.Equals(SelectTag?)"/> for the actual comparison logic.
    /// </remarks>
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
    /// <returns>
    /// An integer hash code for <paramref name="obj"/>, computed in a manner consistent
    /// with the case-insensitive comparison defined in <see cref="Equals(SelectTag?, SelectTag?)"/>.
    /// </returns>
    public int GetHashCode(SelectTag obj) =>
        obj is null ? throw new ArgumentNullException(nameof(obj)) : obj.GetHashCode();

    //TODO: Proof read Documentation, unit testing
    /// <summary>
    /// == operator
    /// </summary>
    /// <param name="left">The first <see cref="SelectTag"/> to compare.</param>
    /// <param name="right">The second <see cref="SelectTag"/> to compare.</param>
    /// <returns></returns>
    public static bool operator ==(SelectTag? left, SelectTag? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="SelectTag"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="SelectTag"/> to compare.</param>
    /// <param name="right">The second <see cref="SelectTag"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> represent the same column;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-insensitive and equivalent to calling
    /// <see cref="Equals(SelectTag?, SelectTag?)"/>.
    /// </remarks>
    public static bool operator !=(SelectTag? left, SelectTag? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether this <see cref="SelectTag"/> is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the SQL representation of this column is <c>null</c>, empty,
    /// or consists only of white space; otherwise, <c>false</c>.
    /// </returns>
    public bool IsEmpty() =>
        ToString().IsNullOrWhiteSpace();

    // TODO: proof Read Documentation, unit tests
    /// <summary>
    /// Get all SelectTags associated with the instance, as a string.
    /// For SelectTag this will just be itself.
    /// </summary>
    /// <returns>All SelectTags associated with the instance, as a string. For SelectTag this will just be itself.</returns>
    public string GetSelects() =>
        this;

    //Proof read documentation, unit tests
    /// <summary>
    /// Get all TableTags associated with the instance.
    /// For SelectTag this will just it's TableTag as an Enumerable.
    /// </summary>
    /// <returns>
    /// All TableTags associated with the instance.
    /// For SelectTag this will just it's TableTag as an Enumerable.
    /// </returns>
    public IEnumerable<TableTag> GetTableTags() => 
        [ColumnTag.TableTag];

    //Proof read documentation, unit tests
    /// <summary>
    /// Determines if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// </summary>
    /// <returns>
    /// True if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// </returns>
    public bool Any() =>
        true;
}
