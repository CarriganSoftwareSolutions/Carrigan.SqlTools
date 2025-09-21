using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a single column specification within an SQL <c>ORDER BY</c> clause.
/// </summary>
public interface IOrderByItem : IEquatable<IOrderByItem>
{
    /// <summary>
    /// Represents the Column identified associated with this instance
    /// </summary>
    public ColumnTag ColumnTag { get; }
    /// <summary>
    /// Gets the <see cref="ColumnTag"/> associated with this instance.
    /// </summary>
    public TableTag TableTag { get; }
    /// <summary>
    /// Gets the sort direction, as defined by the <see cref="SortDirectionEnum"/> enumeration.
    /// </summary>
    public SortDirectionEnum SortDirection { get; }
    /// <summary>
    /// Generates the SQL fragment for this order-by item.
    /// </summary>
    /// <returns>
    /// A SQL string representing this item, for example <c>[Order].[OrderDate] ASC</c>.
    /// </returns>
    public string ToSql();
}
