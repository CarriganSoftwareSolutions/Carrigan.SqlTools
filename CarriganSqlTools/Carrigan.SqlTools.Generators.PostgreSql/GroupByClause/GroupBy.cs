using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents a single-column specification within a SQL <c>ORDER BY</c> clause.
/// Also implements <see cref="GroupBysBase"/> to make single-column order-by usage convenient.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table containing the column to order by.
/// </typeparam>
public class GroupBy<T> : GroupByBase where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public GroupBy(PropertyName propertyName) : base(GetColumnInfo(propertyName)) 
    {}

    private static ColumnInfo GetColumnInfo(PropertyName propertyName) =>
        SqlToolsReflectorCache<T>.GetColumnsFromProperties(DialectStatics.SupportedTypes, propertyName).Single();

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    [ExternalOnly]
    public GroupBy(string propertyName) : this(new PropertyName(propertyName))
    {
    }

    /// <summary>
    /// Converts a single <see cref="GroupBy{T}"/> item into an <see cref="GroupBys"/> collection.
    /// </summary>
    /// <param name="orderByItem">The single order-by item to wrap.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public static implicit operator GroupBys(GroupBy<T> orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new GroupBys(orderByItem);
    }

    /// <summary>
    /// Converts a single <see cref="GroupBy{T}"/> item into an <see cref="GroupBysBase"/> collection.
    /// </summary>
    /// <param name="orderByItem">The single order-by item to wrap.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public static implicit operator GroupBysBase(GroupBy<T> orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new GroupBys(orderByItem);
    }

    /// <summary>
    /// Part of the <see cref="GroupBysBase"/> implementation.
    /// Returns the single <see cref="TableTag"/> involved in this order-by item.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        [TableTag];
}
