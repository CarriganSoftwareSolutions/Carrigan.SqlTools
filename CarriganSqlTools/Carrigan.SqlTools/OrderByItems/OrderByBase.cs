using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a complete <c>ORDER BY</c> clause in SQL.
/// </summary>
public abstract class OrderByBase
{
    /// <summary>
    /// Enumerates all tables referenced in the <c>ORDER BY</c> clause.
    /// </summary>
    internal abstract IEnumerable<TableTag> TableTags { get; }

    /// <summary>
    /// Determines whether the specified <paramref name="orderByItem"/> is already included
    /// in the <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderByItem">The individual order-by item to check.</param>
    /// <returns>
    /// <c>true</c> if the item is already part of the <c>ORDER BY</c> clause; otherwise, <c>false</c>.
    /// </returns>
    public abstract bool Contains(OrderByItemBase orderByItem);

    /// <summary>
    /// Determines whether the <c>ORDER BY</c> clause is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the <c>ORDER BY</c> clause contains no items; otherwise, <c>false</c>.
    /// </returns>
    public abstract bool IsEmpty();

    /// <summary>
    /// Generates the SQL <c>ORDER BY</c> clause represented by this instance.
    /// </summary>
    /// <returns>
    /// A SQL string for the <c>ORDER BY</c> clause.
    /// </returns>
    internal abstract string ToSql();

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified <see cref="OrderByItemBase"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItem">
    /// The <see cref="OrderByItemBase"/>—consisting of a table, column, and sort direction—to append.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the appended <see cref="OrderByItemBase"/>.
    /// </returns>
    public abstract OrderBy WithAppend(OrderByItemBase orderByItem);


    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified sequence of
    /// <see cref="OrderByItemBase"/> objects concatenated.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItems">
    /// A sequence of <see cref="OrderByItemBase"/> objects—each defining a table,
    /// column, and sort direction—to append to the clause.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the concatenated items.
    /// </returns>
    public abstract OrderBy WithConcat(params IEnumerable<OrderByItemBase> orderByItems);

    /// <summary>
    /// Returns this instance cast to its concrete implementation, <see cref="OrderBy"/>.
    /// </summary>
    /// <returns>This instance as an <see cref="OrderBy"/> object.</returns>
    public abstract OrderBy AsOrderBy();
}