using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// This interface represents an entire order by clause in SQL
/// </summary>
public interface IOrderByClause
{
    /// <summary>
    /// An enumeration of all tables used in the order by clause.
    /// </summary>
    IEnumerable<TableTag> TableTags { get; }

    /// <summary>
    /// Determines in the individual order by item's column is already part of the order by clause.
    /// </summary>
    /// <param name="orderByItem"></param>
    /// <returns>Returns true if the individual order by item's column is already part of the order by clause. Else false.</returns>
    bool Contains(IOrderByItem orderByItem);
    /// <summary>
    /// Determines the order by clause contains any individual order by columns.
    /// </summary>
    /// <returns>Returns true if the Order By Clause contains any individual order by columns. Else false.</returns>
    bool IsEmpty();
    /// <summary>
    /// Produces the SQL represented by this class.
    /// </summary>
    /// <returns>Returns a string for an Order By Clause.</returns>
    string ToSql();
    /// <summary>
    /// Creates a new order by clause with a <see cref="IOrderByItem"> appended. This operation is immutable to the original object.
    /// </summary>
    /// <param name="orderByItem">Represents another individual order by item consisting of a table, column and sort direction.</param>
    /// <returns>Returns a new order by clause with a <see cref="IOrderByItem"> appended.</returns>
    OrderBy WithAppend(IOrderByItem orderByItem);
    /// <summary>
    /// Creates a new order by clause with a <see cref="IEnumerable<IOrderByItem>"> concatenated. This operation is immutable to the original object.
    /// </summary>
    /// <param name="orderByItem">Represents another individual order by item consisting of a table, column and sort direction.</param>
    /// <returns>Returns a new order by clause with a <see cref="IEnumerable<IOrderByItem>"> concatenated.</returns>
    OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems);
    /// <summary>
    /// Returns this object cast as the concrete implementation <see cref="OrderBy"/>
    /// </summary>
    /// <returns>Returns this object cast as the concrete implementation <see cref="OrderBy"/></returns>
    OrderBy AsOrderBy();
}