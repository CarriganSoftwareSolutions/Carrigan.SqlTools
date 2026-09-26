using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByClause;

/// <summary>
/// Represents a single-column specification within a SQL <c>ORDER BY</c> clause.
/// Also implements <see cref="OrderBys"/> to make single-column order-by usage convenient.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table containing the column to order by.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// OrderBy<Customer> orderBy = new(nameof(Customer.Name));
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     OrderBys = orderBy
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// ORDER BY [Customer].[Name] ASC
/// ]]></code>
/// </example>
public class OrderBy<T> : OrderBy where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public OrderBy(PropertyName propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        : base(new Column<T>(propertyName), sortDirection)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    [ExternalOnly]
    public OrderBy(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        : this(new PropertyName(propertyName), sortDirection)
    {
    }

    /// <summary>
    /// Converts a single <see cref="OrderBy{T}"/> item into an <see cref="OrderBys"/> collection.
    /// </summary>
    /// <param name="orderByItem">The single order-by item to wrap.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public static implicit operator OrderBys(OrderBy<T> orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new OrderBys(orderByItem);
    }
}
