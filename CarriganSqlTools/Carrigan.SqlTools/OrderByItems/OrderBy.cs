using Carrigan.Core.Extensions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Linq;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Concrete implementation of the <see cref="IOrderByClause"/> for a multiple column Oder By Clause.
/// </summary>
/// <example>
/// OrderByItem<Customer> orderBy1 = new(nameof(Customer.Name));
/// OrderByItem<Customer> orderBy2 = new(nameof(Customer.Id), SortDirectionEnum.Descending);
/// OrderBy orderBy = new(orderBy1, orderBy2);
/// SqlQuery query = customerGenerator.Select(null, null, orderBy, null);
/// 
/// // SELECT [Customer].* FROM [Customer] 
/// // ORDER BY [Customer].[Name] ASC, [Customer].[Id] DESC
/// </example>
public class OrderBy: IOrderByClause
{
    /// <summary>
    /// An Empty Order By Clause
    /// </summary>
    public static OrderBy Empty => 
        new();

    /// <summary>
    /// Represents all the different parts of an order by clause, one for each individual column.
    /// </summary>
    private IEnumerable<IOrderByItem> _orderByItems;

    /// <summary>
    /// Constructor of an object representing the Order By clause
    /// </summary>
    /// <param name="orderByItems">partial order by representing an individual column.</param>
    public OrderBy(params IEnumerable<IOrderByItem> orderByItems)
    {
        _orderByItems = orderByItems;
    }

    /// <summary>
    /// Determines if the individual column ordering is represented in the Order By Clauses.
    /// Note: base on the table and column name only, the sort direction is insensitive.
    /// </summary>
    /// <param name="orderByItem"></param>
    /// <returns>True if the column is represented in the order by clause. Otherwise false.</returns>
    public bool Contains(IOrderByItem orderByItem) =>
        _orderByItems.Contains(orderByItem);

    /// <summary>
    /// An enumeration of all tables used in the order by clause.
    /// </summary>
    public virtual IEnumerable<TableTag>TableTags => _orderByItems.Select(item => item.TableTag);

    /// <summary>
    /// Produces the SQL represented by this class.
    /// Note: <see cref="OrderByItem{T}"/> unlike <see cref="OrderBy"/> does not include the "ORDER BY" text which has to be added in by the <see cref="SqlGenerator{T}"/>
    /// </summary>
    /// <returns>Returns a string for an Order By Clause.</returns>
    public virtual string ToSql() =>
        IsEmpty() 
            ? string.Empty
            : $"ORDER BY {string.Join(", ", _orderByItems.Select(item => item.ToSql()))}";

    /// <summary>
    /// Returns an enumeration of all contained <see cref="IOrderByItem"/>
    /// </summary>
    /// <returns>an enumeration of all contained <see cref="IOrderByItem"/></returns>
    public virtual IEnumerable<IOrderByItem> OrderByItemsAsEnumerable() =>
        _orderByItems;

    /// <summary>
    /// Determines if the Order By Clause is Empty.
    /// </summary>
    /// <returns>Returns true if empty, else returns false</returns>
    public virtual bool IsEmpty() =>
        OrderByItemsAsEnumerable().IsNullOrEmpty();

    /// <summary>
    /// Creates a new order by clause with a <see cref="IOrderByItem"> appended. This operation is immutable to the original object.
    /// </summary>
    /// <param name="orderByItem">Represents another individual order by item consisting of a table, column and sort direction.</param>
    /// <returns>Returns a new order by clause with a <see cref="IOrderByItem"> appended.</returns>
    public OrderBy WithAppend(IOrderByItem orderByItem) =>
        new (_orderByItems.Append(orderByItem));

    /// <summary>
    /// Creates a new order by clause with a <see cref="IEnumerable<IOrderByItem>"> concatenated. This operation is immutable to the original object.
    /// </summary>
    /// <param name="orderByItem">Represents another individual order by item consisting of a table, column and sort direction.</param>
    /// <returns>Returns a new order by clause with a <see cref="IEnumerable<IOrderByItem>"> concatenated.</returns>
    public OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems) =>
        new (_orderByItems.Concat(orderByItems));

    /// <summary>
    /// Returns this object cast as the concrete implementation <see cref="OrderBy"/>
    /// </summary>
    /// <returns>Returns this object cast as the concrete implementation <see cref="OrderBy"/></returns>
    public OrderBy AsOrderBy() => this;
}
