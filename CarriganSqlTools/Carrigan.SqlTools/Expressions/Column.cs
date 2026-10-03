using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Base class for SQL expression nodes that reference a single reflected table column.
/// Carries the resolved <see cref="ColumnInfo"/> and exposes the owning <see cref="TableTag"/>.
/// </summary>
/// <remarks>
/// This class exists to centralize column/table metadata for expression nodes. Predicate types can consume
/// these column expressions when building SQL <c>WHERE</c>, <c>JOIN</c>, and other expression-bearing clauses.
/// </remarks>
public class Column : SqlExpression, IColumnBase, IColumnExpressionIdentity
{
    /// <summary>
    /// Gets the resolved column metadata (name, tags, etc.) used by the expression.
    /// </summary>
    public ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// The name of the property representing the column.
    /// </summary>
    public PropertyName PropertyName => 
        ColumnInfo.PropertyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="Column"/> class.
    /// </summary>
    /// <param name="columnInfo">The resolved column metadata.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="columnInfo"/> is <c>null</c>.
    /// </exception>
    protected Column(ColumnInfo columnInfo) : base([])
    {
        ArgumentNullException.ThrowIfNull(columnInfo, nameof(columnInfo));
        ColumnInfo = columnInfo;
    }

    ColumnTag IColumnExpressionIdentity.EqualityColumnTag =>
        ColumnInfo.ColumnTag;

    /// <summary>
    /// Gets the table tag represented by this column leaf expression.
    /// </summary>
    public override IEnumerable<TableTag> LeafTables =>
        [ColumnInfo.ColumnTag.TableTag];

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
        yield return ColumnInfo.ColumnTag;
    }
}