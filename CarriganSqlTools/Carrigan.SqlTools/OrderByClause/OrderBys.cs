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
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     OrderBys = orderBys
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SQL Server
/// SELECT [Customer].* 
/// FROM [Customer] 
/// ORDER BY [Customer].[Name] ASC, 
///          [Customer].[Id] DESC
/// --PostgreSql      
/// SELECT "Customer".* 
/// FROM "Customer" 
/// ORDER BY "Customer"."Name" ASC, 
///          "Customer"."Id" DESC
/// ]]></code>
/// </example>
public class OrderBys
{

    /// <summary>
    /// Holds all parts of the <c>ORDER BY</c> clause, with one <see cref="OrderBy"/>
    /// for each individual column.
    /// </summary>
    protected readonly IEnumerable<OrderBy> _orderByItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBys"/> class,
    /// representing an <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderByItems">
    /// The <see cref="OrderBy"/> objects defining the columns and sort directions
    /// for the <c>ORDER BY</c> clause.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItems"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="orderByItems"/> contains disallowed <c>null</c> values.
    /// </exception>
    public OrderBys(params IEnumerable<OrderBy> orderByItems)
    {
        ArgumentNullException.ThrowIfNull(orderByItems, nameof(orderByItems));

        _orderByItems = orderByItems.Materialize(NullOptionsEnum.Exception);
    }

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
    public bool Contains(OrderBy orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return _orderByItems.Contains(orderByItem);
    }

    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects referenced in the <c>ORDER BY</c> clause.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        _orderByItems.SelectMany(static item => item.TableTags);

    /// <summary>
    /// Determines whether the <c>ORDER BY</c> clause is empty.
    /// </summary>
    public bool IsEmpty() =>
        _orderByItems.IsNullOrEmpty();

    /// <summary>
    /// Determines whether the <c>ORDER BY</c> clause is empty.
    /// </summary>
    public static OrderBys Empty =>
        new ();

    /// <summary>
    /// Creates a new collection with the supplied item appended.
    /// </summary>
    /// <param name="orderByItem">The ORDER BY item to append.</param>
    /// <returns>A new collection containing the existing order-by items followed by <paramref name="orderByItem"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public OrderBys Append(OrderBy orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new OrderBys(_orderByItems.Append(orderByItem));
    }

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="orderByItems">The ORDER BY items to append.</param>
    /// <returns>A new collection containing the existing items followed by the supplied order-by items.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public OrderBys Concat(params IEnumerable<OrderBy> orderByItems)
    {
        ArgumentNullException.ThrowIfNull(orderByItems, nameof(orderByItems));

        return new OrderBys(_orderByItems.Concat(orderByItems));
    }

    /// <summary>
    /// Returns this instance cast to the concrete implementation, <see cref="OrderBys"/>.
    /// </summary>
    /// <returns>
    /// This instance as an <see cref="OrderBys"/> object.
    /// </returns>
    [Obsolete("Not needed anymore.")]
    public OrderBys AsOrderBy() => this;

    /// <summary>
    /// Returns all contained <see cref="OrderBy"/> objects.
    /// </summary>
    /// <returns>
    /// An enumeration of all <see cref="OrderBy"/> objects contained in this instance.
    /// </returns>
    public IEnumerable<OrderBy> AsEnumerable() =>
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

    /// <summary>
    /// Defines an implicit conversion from <see cref="OrderBy"/> to <see cref="OrderBys"/>,
    /// </summary>
    /// <param name="orderBy">
    /// The <see cref="OrderBy"/> instance to convert. The resulting <see cref="OrderBys"/> will contain this single item.
    /// </param>
    public static implicit operator OrderBys(OrderBy orderBy) =>
        (new OrderBys(orderBy));
}
