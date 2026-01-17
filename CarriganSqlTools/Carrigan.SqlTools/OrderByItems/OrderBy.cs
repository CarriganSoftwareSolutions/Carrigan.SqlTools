using Carrigan.Core.Enums;
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
public class OrderBy : OrderByBase
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
    /// The <see cref="OrderByItemBase"/> objects defining the columns and sort directions
    /// for the <c>ORDER BY</c> clause.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItems"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="orderByItems"/> contains disallowed <c>null</c> values.
    /// </exception>
    public OrderBy(params IEnumerable<OrderByItemBase> orderByItems)
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
    public override bool Contains(OrderByItemBase orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return _orderByItems.Contains(orderByItem);
    }

    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects referenced in the <c>ORDER BY</c> clause.
    /// </summary>
    internal override IEnumerable<TableTag> TableTags =>
        _orderByItems.Select(static item => item.TableTag);

    /// <summary>
    /// Generates the SQL <c>ORDER BY</c> clause represented by this instance.
    /// </summary>
    /// <returns>
    /// A SQL string for the <c>ORDER BY</c> clause, or <see cref="string.Empty"/>
    /// if no ordering is defined.
    /// </returns>
    internal override string ToSql() =>
        IsEmpty()
            ? string.Empty
            : $"ORDER BY {string.Join(", ", _orderByItems.Select(static item => item.ToSql()))}";

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
    public override bool IsEmpty() =>
        _orderByItems.IsNullOrEmpty();

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
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItem"/> is <c>null</c>.
    /// </exception>
    public override OrderBy WithAppend(OrderByItemBase orderByItem)
    {
        ArgumentNullException.ThrowIfNull(orderByItem, nameof(orderByItem));

        return new(_orderByItems.Append(orderByItem));
    }

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified sequence of
    /// <see cref="OrderByItemBase"/> objects concatenated.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItems">
    /// The <see cref="OrderByItemBase"/> objects to append to the clause.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the concatenated items.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="orderByItems"/> is <c>null</c>.
    /// </exception>
    public override OrderBy WithConcat(params IEnumerable<OrderByItemBase> orderByItems)
    {
        ArgumentNullException.ThrowIfNull(orderByItems, nameof(orderByItems));

        return new(_orderByItems.Concat(orderByItems));
    }

    /// <summary>
    /// Returns this instance cast to the concrete implementation, <see cref="OrderBy"/>.
    /// </summary>
    /// <returns>
    /// This instance as an <see cref="OrderBy"/> object.
    /// </returns>
    public override OrderBy AsOrderBy() => this;
}
