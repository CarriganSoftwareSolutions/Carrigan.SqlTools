using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a single-column specification within a SQL <c>ORDER BY</c> clause.
/// </summary>
public abstract class OrderByItemBase : OrderByBase, IEquatable<OrderByItemBase> 
{
    /// <summary>
    /// Gets the <see cref="Tags.ColumnInfo"/> associated with this item.
    /// </summary>
    internal abstract ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Gets the table tag associated with the ordered column.
    /// </summary>
    internal TableTag TableTag => 
        ColumnInfo.ColumnTag.TableTag;

    /// <summary>
    /// Gets the sort direction for this item.
    /// </summary>
    internal SortDirectionEnum SortDirection { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByItemBase"/> class with the specified sort direction.
    /// </summary>
    /// <param name="sortDirection">The sort direction to apply (defaults to <see cref="SortDirectionEnum.Ascending"/>).</param>
    protected OrderByItemBase(SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) => 
        SortDirection = sortDirection;

    /// <summary>
    /// Determines whether the current instance is equal to another <see cref="OrderByItemBase"/>.
    /// </summary>
    /// <remarks>
    /// Equality compares only the table and column (i.e., <see cref="TableTag"/> and <see cref="ColumnInfo"/>)
    /// and intentionally ignores <see cref="SortDirection"/>.
    /// </remarks>
    /// <param name="other">The <see cref="OrderByItemBase"/> to compare with this instance.</param>
    /// <returns><c>true</c> if both items refer to the same table and column; otherwise, <c>false</c>.</returns>
    public bool Equals(OrderByItemBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return TableTag.Equals(other.TableTag)
            && ColumnInfo.Equals(other.ColumnInfo);
    }

    /// <summary>
    /// Generates the SQL fragment for this order-by item (without the <c>ORDER BY</c> keyword).
    /// </summary>
    /// <remarks>
    /// The containing <see cref="SqlGenerators.SqlGenerator{T}"/> is responsible for emitting the
    /// <c>ORDER BY</c> keyword and for joining multiple items.
    /// </remarks>
    /// <returns>A SQL string representing this item, e.g., <c>[Order].[OrderDate] ASC</c>.</returns>
    internal override string ToSql() =>
        $"{ColumnInfo} {SortDirection.ToSql()}";

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is an equal <see cref="OrderByItemBase"/>; otherwise, <c>false</c>.</returns>
    public abstract override bool Equals(object? obj);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>An integer hash code for the current object.</returns>
    public abstract override int GetHashCode();
}
