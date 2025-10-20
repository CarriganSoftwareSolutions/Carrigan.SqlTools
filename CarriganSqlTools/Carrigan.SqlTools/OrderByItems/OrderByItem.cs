using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a single-column specification within a SQL <c>ORDER BY</c> clause.
/// Also implements <see cref="OrderByBase"/> to make single-column order-by usage convenient.
/// </summary>
/// <typeparam name="T">
/// The entity/model type that defines the table containing the column to order by.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// OrderByItem<Customer> orderBy = new(nameof(Customer.Name));
/// SqlQuery query = customerGenerator.Select(null, null, null, orderBy, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// ORDER BY [Customer].[Name] ASC, [Customer].[Id] DESC
/// ]]></code>
/// </example>
public class OrderByItem<T> : OrderByItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByItem{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    public OrderByItem(PropertyName propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        : base(sortDirection) =>
        ColumnInfo = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).Single();

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByItem{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    [ExternalOnly]
    public OrderByItem(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) :
        this (new PropertyName(propertyName), sortDirection) {}

    /// <summary>
    /// The <see cref="Tags.ColumnInfo"/> that specifies the column being ordered.
    /// </summary>
    internal override ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Part of the <see cref="OrderByBase"/> implementation.
    /// Returns the single <see cref="TableTag"/> involved in this order-by item.
    /// </summary>
    internal override IEnumerable<TableTag> TableTags => 
        [TableTag];

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// <see cref="OrderByItemBase"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is an <see cref="OrderByItemBase"/>
    /// that represents the same table/column; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
        => Equals(obj as OrderByItemBase);

    /// <summary>
    /// Serves as the default hash function for <see cref="OrderByItemBase"/>.
    /// </summary>
    /// <returns>
    /// An integer hash code computed from the <see cref="TableTag"/> and <see cref="ColumnInfo"/>.
    /// </returns>
    public override int GetHashCode()
        => HashCode.Combine(TableTag, ColumnInfo);

    /// <summary>
    /// Returns whether this single order-by item “contains” the specified item.
    /// </summary>
    /// <remarks>
    /// Because <see cref="OrderByItem{T}"/> is modeled as a single-item sequence for interchangeability
    /// with <see cref="OrderBy"/>, this is effectively an equality check against <paramref name="orderByItem"/>.
    /// </remarks>
    /// <param name="orderByItem">The order-by item to compare.</param>
    /// <returns><c>true</c> if both items are equal; otherwise, <c>false</c>.</returns>
    public override bool Contains(OrderByItemBase orderByItem) =>
        EqualityComparer<OrderByItemBase>.Default.Equals(this, orderByItem);

    /// <summary>
    /// Determines whether this order-by item is empty.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the underlying column is considered empty; otherwise, <c>false</c>.
    /// </returns>
    public override bool IsEmpty() =>
        ColumnInfo.IsEmpty();

    /// <summary>
    /// Creates a new <see cref="OrderBy"/> with this item followed by <paramref name="orderByItem"/>.
    /// This operation is immutable; the current instance is not modified.
    /// </summary>
    /// <param name="orderByItem">The item to append (table/column/sort direction).</param>
    /// <returns>
    /// A new <see cref="OrderBy"/> that includes both this item and <paramref name="orderByItem"/>.
    /// </returns>
    public override OrderBy WithAppend(OrderByItemBase orderByItem) =>
        new (new List<OrderByItemBase>([this, orderByItem]));

    /// <summary>
    /// Creates a new <see cref="OrderBy"/> with this item followed by the specified sequence.
    /// This operation is immutable; the current instance is not modified.
    /// </summary>
    /// <param name="orderByItems">
    /// One or more sequences of <see cref="OrderByItemBase"/> objects to append.
    /// </param>
    /// <returns>
    /// A new <see cref="OrderBy"/> that includes this item and all provided items.
    /// </returns>
    public override OrderBy WithConcat(params IEnumerable<OrderByItemBase> orderByItems) =>
        new (orderByItems.Prepend(this));

    /// <summary>
    /// Returns a new <see cref="OrderBy"/> instance that represents this single item.
    /// </summary>
    /// <returns>A new <see cref="OrderBy"/> containing this item.</returns>
    public override OrderBy AsOrderBy() =>
        new (this);
}
