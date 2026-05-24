using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByClause;

/// <summary>
/// Concrete implementation of <see cref="OrderBys"/> for an <c>ORDER BY</c>
/// clause that supports multiple columns.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// OrderBy<Customer> orderBy1 = new(nameof(Customer.Name));
/// OrderBy<Customer> orderBy2 = new(nameof(Customer.Id), SortDirectionEnum.Descending);
/// OrderBys orderBys = new(orderBy1, orderBy2);
/// SqlQuery query = customerGenerator.Select(null, null, null, orderBys, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// ORDER BY 
///     [Customer].[Name] ASC, 
///     [Customer].[Id] DESC
/// ]]></code>
/// </example>
public class OrderBys
{
    /// <summary>
    /// Gets an empty <c>ORDER BY</c> clause.
    /// </summary>
    public static OrderBys Empty =>
        new();

    /// <summary>
    /// Holds all parts of the <c>ORDER BY</c> clause, with one <see cref="OrderByBase"/>
    /// for each individual column.
    /// </summary>
    private readonly IEnumerable<OrderByBase> _orderByItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBys"/> class,
    /// representing an <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderByItems">
    /// The <see cref="OrderByBase"/> objects defining the columns and sort directions
    /// for the <c>ORDER BY</c> clause.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItems"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="orderByItems"/> contains disallowed <c>null</c> values.
    /// </exception>
    public OrderBys(params IEnumerable<OrderByBase> orderByItems)
    {
        ArgumentNullException.ThrowIfNull(orderByItems, nameof(orderByItems));

        _orderByItems = orderByItems.Materialize(NullOptionsEnum.Exception);
    }

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
    /// A new <see cref="OrderBys"/> instance containing one <see cref="OrderBy{T}"/> item.
    /// </returns>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    public static OrderBys New<T>(PropertyName propertyName, SortDirectionEnum sortDirection) where T : class => 
        new(new OrderBy<T>(propertyName, sortDirection));

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
    /// A new <see cref="OrderBys"/> instance containing one <see cref="OrderBy{T}"/> item.
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
    /// Determines whether the specified <paramref name="orderByItem"/> is present
    /// in the <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderByItem">The individual order-by item to check.</param>
    /// <returns>
    /// <c>true</c> if the item is contained in this <c>ORDER BY</c>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItem"/> is <c>null</c>.
    /// </exception>
    public bool Contains(OrderByBase orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return _orderByItems.Contains(orderByItem);
    }

    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects referenced in the <c>ORDER BY</c> clause.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        _orderByItems.Select(static item => item.TableTag);

    /// <summary>
    /// Determines whether the <c>ORDER BY</c> clause is empty.
    /// </summary>
    public bool IsEmpty() =>
        _orderByItems.IsNullOrEmpty();

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified <see cref="OrderByBase"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItem">
    /// The <see cref="OrderByBase"/> to append, representing a table, column, and sort direction.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the appended <see cref="OrderByBase"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItem"/> is <c>null</c>.
    /// </exception>
    public OrderBys Append(OrderByBase orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new(_orderByItems.Append(orderByItem));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction. Then creates a new <c>ORDER BY</c> clause with 
    /// the specified <see cref="OrderByBase"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the appended <see cref="OrderByBase"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItem"/> is <c>null</c>.
    /// </exception>
    public OrderBys Append<T>(PropertyName propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) where T : class =>
        new(_orderByItems.Append(new OrderBy<T>(propertyName, sortDirection)));

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction. Then creates a new <c>ORDER BY</c> clause with 
    /// the specified <see cref="OrderByBase"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the appended <see cref="OrderByBase"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItem"/> is <c>null</c>.
    /// </exception>
    [ExternalOnly]
    public OrderBys Append<T>(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) where T : class =>
        Append<T>(new PropertyName(propertyName), sortDirection);

    [Obsolete("WithAppend has been deprecated. Please use just Append from now on.")]
    public OrderBys WithAppend(OrderByBase orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new(_orderByItems.Append(orderByItem));
    }

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified sequence of
    /// <see cref="OrderByBase"/> objects concatenated.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItems">
    /// The <see cref="OrderByBase"/> objects to append to the clause.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the concatenated items.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItems"/> is <c>null</c>.
    /// </exception>
    public OrderBys Concat(params IEnumerable<OrderByBase> orderByItems)
    {
        ArgumentNullException.ThrowIfNull(orderByItems, nameof(orderByItems));

        return new(_orderByItems.Concat(orderByItems));
    }

    [Obsolete("WithConcat has been deprecate. Please use just Concat from now on.")]
    public OrderBys WithConcat(params IEnumerable<OrderByBase> orderByItems)
    {
        ArgumentNullException.ThrowIfNull(orderByItems, nameof(orderByItems));

        return new(_orderByItems.Concat(orderByItems));
    }

    /// <summary>
    /// Returns this instance cast to the concrete implementation, <see cref="OrderBys"/>.
    /// </summary>
    /// <returns>
    /// This instance as an <see cref="OrderBys"/> object.
    /// </returns>
    public OrderBys AsOrderBy() => this;

    /// <summary>
    /// Returns all contained <see cref="OrderByBase"/> objects.
    /// </summary>
    /// <returns>
    /// An enumeration of all <see cref="OrderByBase"/> objects contained in this instance.
    /// </returns>
    public IEnumerable<OrderByBase> AsEnumerable() =>
        _orderByItems;

    /// <summary>
    /// Generates the SQL <c>ORDER BY</c> clause represented by this instance.
    /// </summary>
    /// <returns>
    /// A SQL string for the <c>ORDER BY</c> clause, or <see cref="string.Empty"/>
    /// if no ordering is defined.
    /// </returns>
    internal string ToSql(ISqlDialects dialect) =>
        IsEmpty()
            ? string.Empty
            : $"ORDER BY {string.Join(", ", _orderByItems.Select(item => item.ToSql(dialect)))}";
}
