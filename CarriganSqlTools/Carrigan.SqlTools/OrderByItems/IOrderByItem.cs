using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// This interface represents a part of an order by clause for an individual column in SQL
/// </summary>
public interface IOrderByItem : IEquatable<IOrderByItem>
{
    /// <summary>
    /// Represents the Column identified associated with this instance
    /// </summary>
    public ColumnTag ColumnTag { get; }
    /// <summary>
    /// Represents the Table identified associated with this instance
    public TableTag TableTag { get; }
    /// <summary>
    /// An Enum to represent the sort direction.
    /// </summary>
    public SortDirectionEnum SortDirection { get; }
    /// <summary>
    /// Returns the SQL as a string for the given instance.
    /// </summary>
    /// <returns>Returns the SQL as a string for the given instance.</returns>
    /// <example>[Order].[OrderDate] ASC</example>
    public string ToSql();
}
