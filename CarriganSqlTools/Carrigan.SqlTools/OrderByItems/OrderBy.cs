using Carrigan.Core.Extensions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Concrete implementation of <see cref="OrderByBase"/> for an <c>ORDER BY</c>
/// clause that supports multiple columns.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// OrderByItem<Customer> orderBy1 = new(nameof(Customer.Name));
/// OrderByItem<Customer> orderBy2 = new(nameof(Customer.Id), SortDirectionEnum.Descending);
/// OrderBy orderBy = new(orderBy1, orderBy2);
/// SqlQuery query = customerGenerator.Select(null, null, null, orderBy, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// ORDER BY [Customer].[Name] ASC, [Customer].[Id] DESC
/// ]]></code>
/// </example>
public class OrderBy: OrderByBase
{
    /// <summary>
    /// Gets an empty <c>ORDER BY</c> clause.
    /// </summary>
    public static OrderBy Empty => 
        new();

    /// <summary>
    /// Holds all parts of the <c>ORDER BY</c> clause, with one <see cref="OrderByItemBase"/>
    /// for each individual column.
    /// </summary>
    private readonly IEnumerable<OrderByItemBase> _orderByItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy"/> class,
    /// representing an <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderByItems">
    /// One or more sequences of <see cref="OrderByItemBase"/> objects,
    /// each defining an individual column and sort direction.
    /// </param>
    public OrderBy(params IEnumerable<OrderByItemBase> orderByItems) => 
        _orderByItems = orderByItems;

    /// <summary>
    /// Determines whether the specified <paramref name="orderByItem"/> is represented
    /// in the <c>ORDER BY</c> clause.
    /// </summary>
    /// <remarks>
    /// The check compares only the table and column names and ignores the sort direction.
    /// </remarks>
    /// <param name="orderByItem">
    /// The individual order-by item to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the column is represented in the <c>ORDER BY</c> clause; otherwise, <c>false</c>.
    /// </returns>
    public override bool Contains(OrderByItemBase orderByItem) =>
        _orderByItems.Contains(orderByItem);

    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects referenced
    /// in the <c>ORDER BY</c> clause.
    /// </summary>
    internal override IEnumerable<TableTag>TableTags =>
        _orderByItems.Select(item => item.TableTag);

    /// <summary>
    /// Generates the SQL <c>ORDER BY</c> clause represented by this instance.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="OrderBy"/>, <see cref="OrderByItem{T}"/> does not include the
    /// <c>ORDER BY</c> keyword itself; the <see cref="SqlGenerator{T}"/> must add it.
    /// </remarks>
    /// <returns>
    /// A SQL string for the <c>ORDER BY</c> clause, or <see cref="string.Empty"/>
    /// if no ordering is defined.
    /// </returns>
    internal override string ToSql() =>
        IsEmpty() 
            ? string.Empty
            : $"ORDER BY {string.Join(", ", _orderByItems.Select(item => item.ToSql()))}";

    /// <summary>
    /// Returns all contained <see cref="OrderByItemBase"/> objects.
    /// </summary>
    /// <returns>
    /// An enumeration of all <see cref="OrderByItemBase"/> objects contained in this instance.
    /// </returns>
    public virtual IEnumerable<OrderByItemBase> OrderByItemsAsEnumerable() =>
        _orderByItems;

    /// <summary>
    /// Determines whether the <c>ORDER BY</c> clause is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the <c>ORDER BY</c> clause contains no items; otherwise, <c>false</c>.
    /// </returns>
    public override bool IsEmpty() =>
        OrderByItemsAsEnumerable().IsNullOrEmpty();

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified <see cref="OrderByItemBase"/> appended.  
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItem">
    /// The <see cref="OrderByItemBase"/> to append, representing a table, column, and sort direction.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the appended <see cref="OrderByItemBase"/>.
    /// </returns>
    public override OrderBy WithAppend(OrderByItemBase orderByItem) =>
        new(_orderByItems.Append(orderByItem));

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified sequence of
    /// <see cref="OrderByItemBase"/> objects concatenated.  
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItems">
    /// One or more sequences of <see cref="OrderByItemBase"/> objects—each defining a table,
    /// column, and sort direction—to append to the clause.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the concatenated
    /// <see cref="IEnumerable{IOrderByItem}"/> items.
    /// </returns>
    public override OrderBy WithConcat(params IEnumerable<OrderByItemBase> orderByItems) =>
        new (_orderByItems.Concat(orderByItems));

    /// <summary>
    /// Returns this instance cast to its concrete implementation, <see cref="OrderBy"/>.
    /// </summary>
    /// <returns>
    /// This instance as an <see cref="OrderBy"/> object.
    /// </returns>
    public override OrderBy AsOrderBy() => this;
}
