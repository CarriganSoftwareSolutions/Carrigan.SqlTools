using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a complete <c>ORDER BY</c> clause in SQL.
/// </summary>
public interface IOrderByClause
{
    /// <summary>
    /// Enumerates all tables referenced in the <c>ORDER BY</c> clause.
    /// </summary>
    IEnumerable<TableTag> TableTags { get; }

    /// <summary>
    /// Determines whether the specified <paramref name="orderByItem"/> column
    /// is already included in the <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderByItem">
    /// The individual order-by item to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the column is already part of the <c>ORDER BY</c> clause; otherwise, <c>false</c>.
    /// </returns>
    bool Contains(IOrderByItem orderByItem);

    /// <summary>
    /// Determines whether the <c>ORDER BY</c> clause contains any individual order-by columns.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the <c>ORDER BY</c> clause contains one or more columns; otherwise, <c>false</c>.
    /// </returns>
    bool IsEmpty();

    /// <summary>
    /// Generates the SQL <c>ORDER BY</c> clause represented by this instance.
    /// </summary>
    /// <returns>
    /// A SQL string for the <c>ORDER BY</c> clause.
    /// </returns>
    string ToSql();

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified <see cref="IOrderByItem"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItem">
    /// The individual <see cref="IOrderByItem"/>, consisting of a table, column, and sort direction, to append.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the appended <see cref="IOrderByItem"/>.
    /// </returns>
    OrderBy WithAppend(IOrderByItem orderByItem);

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified sequence of
    /// <see cref="IOrderByItem"/> objects concatenated.  
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItems">
    /// A sequence of <see cref="IOrderByItem"/> objects—each defining a table,
    /// column, and sort direction—to append to the clause.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the concatenated
    /// <see cref="IEnumerable{IOrderByItem}"/> items.
    /// </returns>
    OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems);

    /// <summary>
    /// Returns this instance cast to its concrete implementation, <see cref="OrderBy"/>.
    /// </summary>
    /// <returns>
    /// This instance cast as an <see cref="OrderBy"/> object.
    /// </returns>
    OrderBy AsOrderBy();
}