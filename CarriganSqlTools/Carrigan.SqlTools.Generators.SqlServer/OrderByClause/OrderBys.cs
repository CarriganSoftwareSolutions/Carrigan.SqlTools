using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.OrderByClause;

/// <summary>
/// Represents the concrete collection of <c>ORDER BY</c> items for the dialect package.
/// </summary>
public class OrderBys : OrderBysBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBys"/> class.
    /// </summary>
    public OrderBys() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBys"/> class.
    /// </summary>
    /// <param name="_orderByItems">The order-by items to include in the collection.</param>
    public OrderBys(params IEnumerable<OrderByBase> _orderByItems) : base(_orderByItems)
    {
    }

    /// <summary>
    /// Creates a new collection with the supplied item appended.
    /// </summary>
    /// <param name="orderByItem">The ORDER BY item to append.</param>
    /// <returns>A new collection containing the existing order-by items followed by <paramref name="orderByItem"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public override OrderBys Append(OrderByBase orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new OrderBys(_orderByItems.Append(orderByItem));
    }

    /// <summary>
    /// Appends an order-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="sortDirection">The SQL sort direction.</param>
    /// <returns>A new collection containing the additional order-by item.</returns>
    public override OrderBys Append<T>(PropertyName propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) =>
        new (_orderByItems.Append(new OrderBy<T>(propertyName, sortDirection)));

    /// <summary>
    /// Appends an order-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="sortDirection">The SQL sort direction.</param>
    /// <returns>A new collection containing the additional order-by item.</returns>
    public override OrderBys Append<T>(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) =>
        Append<T>(new PropertyName(propertyName), sortDirection);

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="orderByItems">The ORDER BY items to append.</param>
    /// <returns>A new collection containing the existing items followed by the supplied order-by items.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public override OrderBys Concat(params IEnumerable<OrderByBase> orderByItems)
    {
        ArgumentNullException.ThrowIfNull(orderByItems, nameof(orderByItems));

        return new OrderBys (_orderByItems.Concat(orderByItems));
    }

    /// <summary>
    /// Returns this instance as the concrete <see cref="OrderBys"/> type.
    /// </summary>
    public override OrderBys AsOrderBy() =>
        this;



    /// <summary>
    /// Defines an implicit conversion from <see cref="OrderByBase"/> to <see cref="OrderBysBase"/>,
    /// </summary>
    /// <param name="orderByItemBase">
    /// The <see cref="OrderByBase"/> instance to convert. The resulting <see cref="OrderBysBase"/> will contain this single item.
    /// </param>
    public static implicit operator OrderBys(OrderByBase orderByItemBase) =>
        (new OrderBys (orderByItemBase));

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause containing a single item for the specified
    /// property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The entity/model type that defines the table containing the property to order by.
    /// </typeparam>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <returns>
    /// A new <see cref="OrderBysBase"/> instance containing one <see cref="OrderBy{T}"/> item.
    /// </returns>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public static OrderBys New<T>(PropertyName propertyName, SortDirectionEnum sortDirection) where T : class =>
        new (new OrderBy<T>(propertyName, sortDirection));

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause containing a single item for the specified
    /// property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The entity/model type that defines the table containing the property to order by.
    /// </typeparam>
    /// <param name="propertyName">The name of the property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <returns>
    /// A new <see cref="OrderBysBase"/> instance containing one <see cref="OrderBy{T}"/> item.
    /// </returns>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public static OrderBys New<T>(string propertyName, SortDirectionEnum sortDirection) where T : class =>
        OrderBys.New<T>(new PropertyName(propertyName), sortDirection);


    /// <summary>
    /// Gets an empty <c>ORDER BY</c> clause.
    /// </summary>
    public static OrderBys Empty =>
        new ();
}
