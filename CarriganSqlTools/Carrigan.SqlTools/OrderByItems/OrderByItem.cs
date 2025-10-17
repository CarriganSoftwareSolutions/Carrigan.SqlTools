using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a single-column specification within an SQL <c>ORDER BY</c> clause.
/// Also implements <see cref="OrderByBase"/> to simplify creation of single-column
/// order-by clauses.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that defines the table containing the column to order by.
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
    /// specifying the table type <typeparamref name="T"/>, the column name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that represents the column to order by.
    /// </param>
    /// <param name="sortDirection">
    /// The sort direction to apply.
    /// </param>
    public OrderByItem(PropertyName propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        : base(sortDirection) =>
        ColumnInfo = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).Single();

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
    [ExternalOnly]
    public OrderByItem(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending) :
        this (new PropertyName(propertyName), sortDirection) {}

    /// <summary>
    /// Gets the <see cref="ColumnInfo"/> that specifies the column being ordered.
    /// </summary>
    internal override ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Part of the <see cref="OrderByBase"/> implementation.
    /// Returns an <see cref="IEnumerable{TableTag}"/> containing the table
    /// involved in the sort for this order-by item.
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
    /// and represents the same table and column; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
        => Equals(obj as OrderByItemBase);

    /// <summary>
    /// Serves as the default hash function for <see cref="OrderByItemBase"/>.
    /// The hash code is computed from the <see cref="TableTag"/> and <see cref="ColumnInfo"/> values.
    /// </summary>
    /// <returns>
    /// An integer hash code based on the table and column tags.
    /// </returns>
    public override int GetHashCode()
        => HashCode.Combine(TableTag, ColumnInfo);

    /// <summary>
    /// In this context Contains is basically an alias to determine if this single item equals the item being passed in.
    /// This is because an order by item is being modeled as a single item enumeration, so an OrderBy or OrderByItem can be used interchangeably in query generations.
    /// Therefore, I wanted this method to use the same logic the Contains method uses to determine equality in an IEnumerable, just applied to a single item.
    /// </summary>
    /// <param name="orderByItem"></param>
    /// <returns>true if equal, else false.</returns>
    public override bool Contains(OrderByItemBase orderByItem) =>
        EqualityComparer<OrderByItemBase>.Default.Equals(this, orderByItem);

    /// <summary>
    /// Determines whether this order-by item is empty.
    /// An item is considered empty if its column name is <c>null</c>, empty, or consists only of white space.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the column name is <c>null</c>, empty, or white space; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown if the method is not implemented.
    /// </exception>
    public override bool IsEmpty() =>
        ColumnInfo.IsEmpty();

    /// <summary>
    /// Creates a new order by clause with a <see cref="OrderByItemBase"> appended. This operation is immutable to the original object.
    /// </summary>
    /// <param name="orderByItem">Represents another individual order by item consisting of a table, column and sort direction.</param>
    /// <returns>Returns a new order by clause with a <see cref="OrderByItemBase"> appended.</returns>
    public override OrderBy WithAppend(OrderByItemBase orderByItem) =>
        new (new List<OrderByItemBase>([this, orderByItem]));

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
    public override OrderBy WithConcat(params IEnumerable<OrderByItemBase> orderByItems) =>
        new (orderByItems.Prepend(this));

    /// <summary>
    /// Returns a new <see cref="OrderBy"/> instance that represents this object.
    /// </summary>
    /// <returns>
    /// A new <see cref="OrderBy"/> instance created from this object.
    /// </returns>
    public override OrderBy AsOrderBy() =>
        new (this);
}
