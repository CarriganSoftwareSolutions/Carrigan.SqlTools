using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents the canonical SQL expression for a single reflected table column.
/// </summary>
/// <remarks>
/// Reflection-created projections use this dialect-neutral representation. <see cref="ColumnTag"/> is the canonical
/// column identity used for rendering, equality, and table participation; the obsolete <see cref="ColumnInfo"/> property
/// remains available for compatibility. Dialect packages derive their public <c>Column&lt;T&gt;</c> types from this class.
/// </remarks>
public class Column : SqlExpression
{
    /// <summary>
    /// Gets the legacy resolved reflection metadata retained for compatibility.
    /// </summary>
    [Obsolete("No Longer Used")]
    public ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Gets the unique tag representing the column in SQL expressions and fragments.
    /// </summary>
    public ColumnTag ColumnTag { get; }

    /// <summary>
    /// Gets the unique tag representing the parameterized value of the column in SQL expressions and fragments.
    /// </summary>
    public ParameterTag ColumnParameterTag { get; }

    /// <summary>
    /// The name of the property representing the column.
    /// </summary>
    [Obsolete ("No Longer Used")]
    public PropertyName? PropertyName => 
        ColumnInfo?.PropertyName;

    /// <summary>
    /// Initializes a dialect-neutral column expression from already resolved reflection metadata.
    /// </summary>
    /// <param name="columnInfo">The resolved reflection metadata used to initialize the column and parameter tags.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="columnInfo"/> is <c>null</c>.
    /// </exception>
    protected internal Column(ColumnInfo columnInfo) : base([])
    {
        ArgumentNullException.ThrowIfNull(columnInfo, nameof(columnInfo));
#pragma warning disable CS0618 // Type or member is obsolete
        ColumnInfo = columnInfo;
#pragma warning restore CS0618 // Type or member is obsolete
        ColumnTag = columnInfo.ColumnTag;
        ColumnParameterTag = columnInfo.ParameterTag;
    }


    /// <summary>
    /// Gets the table tag represented by this column leaf expression.
    /// </summary>
    public override IEnumerable<TableTag> LeafTables =>
        [ColumnTag.TableTag];

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
    protected static ColumnInfo GetColumnInfo<T>(HashSet<Type> supportedTypes, PropertyName propertyName) where T : class
    {
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));

        return SqlToolsReflectorCache<T>.GetColumnsFromProperties(supportedTypes, propertyName).SingleOrDefault()
            ?? throw NoSuchProperty<T>(propertyName);
    }

    /// <summary>
    /// Creates a standardized <see cref="ArgumentException"/> for an invalid <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The property name that failed validation.</param>
    /// <returns>
    /// An <see cref="ArgumentException"/> describing the invalid property and the corresponding model/table context.
    /// </returns>
    internal static ArgumentException NoSuchProperty<T>(PropertyName propertyName) where T : class =>
        new($"{propertyName} is not a valid property name on {SqlToolsReflectorCache<T>.Type.Name}, representing: {SqlToolsReflectorCache<T>.Table}.", nameof(propertyName));

    /// <summary>
    /// Produces the SQL fragment represented by this column expression.
    /// </summary>
    /// <returns>
    /// The SQL-escaped column identifier (e.g., <c>[Schema].[Table].[Column]</c> or <c>[Table].[Column]</c>).
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return ColumnTag;
    }
}