using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a single column specification within an SQL <c>ORDER BY</c> clause.
/// </summary>
public abstract class OrderByItemBase : OrderByBase, IEquatable<OrderByItemBase> 
{
    /// <summary>
    /// Represents the Column identified associated with this instance
    /// </summary>
    internal abstract ColumnInfo ColumnInfo { get; }
    /// <summary>
    /// Gets the <see cref="ColumnInfo"/> associated with this instance.
    /// </summary>
    internal TableTag TableTag => 
        ColumnInfo.ColumnTag.TableTag;

    /// <summary>
    /// Gets the sort direction, as defined by the <see cref="SortDirectionEnum"/> enumeration.
    /// </summary>
    internal SortDirectionEnum SortDirection { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByItem{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the column name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that represents the column to order by.
    /// </param>
    /// <param name="sortDirection">
    /// The sort direction to apply.
    /// </param>
    public OrderByItemBase(SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) => 
        SortDirection = sortDirection;

    /// <summary>
    /// Determines whether the current instance is equal to another <see cref="OrderByItemBase"/>.
    /// </summary>
    /// <remarks>
    /// The comparison is based on the table and column tags only and ignores the sort direction.
    /// </remarks>
    /// <param name="other">The <see cref="OrderByItemBase"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the table and column tags are equal; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(OrderByItemBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return TableTag.Equals(other.TableTag)
            && ColumnInfo.Equals(other.ColumnInfo);
    }

    /// <summary>
    /// Generates the SQL fragment for this order-by item.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="OrderBy"/>, <see cref="OrderByItem{T}"/> does not include the
    /// <c>ORDER BY</c> keyword; it must be added separately by <see cref="SqlGenerator{T}"/>.
    /// </remarks>
    /// <returns>
    /// A SQL string representing this item, for example <c>[Order].[OrderDate] ASC</c>.
    /// </returns>
    internal override string ToSql() =>
        $"{ColumnInfo} {SortDirection.ToSql()}";

    public abstract override bool Equals(object? obj);

    public abstract override int GetHashCode();
}
