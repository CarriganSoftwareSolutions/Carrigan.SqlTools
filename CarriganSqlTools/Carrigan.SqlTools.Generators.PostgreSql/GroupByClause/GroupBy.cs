using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents a single-column convenience specialization of a SQL <c>GROUP BY</c> expression.
/// It preserves the property-name convenience API while the base <see cref="GroupBy"/> supports arbitrary SQL expressions.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table containing the column to group by.
/// </typeparam>
public class GroupBy<T> : GroupBy where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name
    /// </summary>
    /// <param name="propertyName">The property representing the column to group by.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public GroupBy(PropertyName propertyName) : base(new Column<T>(propertyName))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name.
    /// </summary>
    /// <param name="propertyName">The property representing the column to group by.</param>
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
    /// <param name="groupByItem">The single group-by item to wrap.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public static implicit operator GroupBys(GroupBy<T> groupByItem)
    {
        ArgumentNullException.ThrowIfNull(groupByItem, nameof(groupByItem));

        return new GroupBys(groupByItem);
    }
}
