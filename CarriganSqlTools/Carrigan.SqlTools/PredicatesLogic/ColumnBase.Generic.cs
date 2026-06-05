using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a model property (i.e., a table column) as a leaf node within predicate logic
/// for SQL <c>WHERE</c> and <c>JOIN</c> clauses.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that defines the table containing the referenced column.
/// </typeparam>
/// <remarks>
/// <see cref="ColumnBase{T}"/> validates property names and throws an exception if a property name is invalid.
/// </remarks>
public class ColumnBase<T> : ColumnBase where T: class
{
    /// <summary>
    /// The name of the property representing the column.
    /// </summary>
    internal PropertyName PropertyName { get; }

    /// <summary>
    /// Creates a standardized <see cref="ArgumentException"/> for an invalid <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The property name that failed validation.</param>
    /// <returns>
    /// An <see cref="ArgumentException"/> describing the invalid property and the corresponding model/table context.
    /// </returns>
    internal static ArgumentException NoSuchProperty(PropertyName propertyName) =>
        new($"{propertyName} is not a valid property name on {SqlToolsReflectorCache<T>.Type.Name}, representing: {SqlToolsReflectorCache<T>.Table}.", nameof(propertyName));

    /// <summary>
    /// Initializes a new <see cref="ColumnBase{T}"/> using a property name.
    /// </summary>
    /// <param name="supportedTypes">The CLR types supported by the current SQL dialect.</param>
    /// <param name="propertyName">The property name that identifies the column.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid, eligible property on <typeparamref name="T"/>.
    /// </exception>
    [ExternalOnly]
    internal ColumnBase(HashSet<Type> supportedTypes, string propertyName) : this(supportedTypes, new PropertyName(propertyName))
    { }

    /// <summary>
    /// Initializes a new <see cref="ColumnBase{T}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="supportedTypes">The CLR types supported by the current SQL dialect.</param>
    /// <param name="propertyName">The property name wrapper that identifies the column.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid, eligible property on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown only if the property passes validation but no matching column metadata is returned.
    /// This is not expected under normal conditions.
    /// </exception>
    internal ColumnBase(HashSet<Type> supportedTypes, PropertyName propertyName) : base(GetColumnInfo(supportedTypes, propertyName)) =>
        PropertyName = propertyName;

    /// <summary>
    /// Resolves reflected column metadata for a model property after applying dialect type filtering.
    /// </summary>
    /// <param name="supportedTypes">The CLR types supported by the current SQL dialect.</param>
    /// <param name="propertyName">The C# property name that represents the SQL column.</param>
    /// <returns>The reflected column metadata for <paramref name="propertyName"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="propertyName"/> does not map to exactly one supported model property.
    /// </exception>
    private static ColumnInfo GetColumnInfo(HashSet<Type> supportedTypes, PropertyName propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

        return SqlToolsReflectorCache<T>.GetColumnsFromProperties(supportedTypes, propertyName).SingleOrDefault()
            ?? throw NoSuchProperty(propertyName);
    }

    /// <summary>
    /// Produces the SQL fragment represented by this column.
    /// </summary>
    /// <returns>
    /// The SQL-escaped column identifier (e.g., <c>[Schema].[Table].[Column]</c> or <c>[Table].[Column]</c>).
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return ColumnInfo.ColumnTag;
    }
}
