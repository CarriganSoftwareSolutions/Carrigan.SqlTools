using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a single-column specification within an SQL <c>ORDER BY</c> clause.
/// Also implements <see cref="IOrderByClause"/> to simplify creation of single-column
/// order-by clauses.
/// </summary>
/// <typeparam name="T">
/// The entity or data model type that defines the table containing the column to order by.
/// </typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// OrderByItem&lt;Customer&gt; orderBy = new(nameof(Customer.Name));
/// SqlQuery query = customerGenerator.Select(null, null, orderBy, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// ORDER BY [Customer].[Name] ASC
/// ]]></code>
/// </example>
public class OrderByItem<T> : IOrderByItem, IOrderByClause
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
    public OrderByItem(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
    {
        ColumnTag = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).Single();
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Gets the <see cref="ColumnTag"/> that specifies the column being ordered.
    /// </summary>
    public ColumnTag ColumnTag { get; private set; }

    /// <summary>
    /// Gets the <see cref="TableTag"/> representing the table whose column is being sorted.
    /// </summary>

    public TableTag TableTag 
        => SqlToolsReflectorCache<T>.Table;

    /// <summary>
    /// Gets the <see cref="SortDirectionEnum"/> value that specifies the sort direction applied.
    /// </summary>
    public SortDirectionEnum SortDirection { get; private set; }

    /// <summary>
    /// Part of the <see cref="IOrderByClause"/> implementation.
    /// Returns an <see cref="IEnumerable{TableTag}"/> containing the table
    /// involved in the sort for this order-by item.
    /// </summary>
    public IEnumerable<TableTag> TableTags => 
        [TableTag];

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
    public string ToSql() =>
        $"{ColumnTag} {SortDirection.ToSql()}";

    /// <summary>
    /// Determines whether the current instance is equal to another <see cref="IOrderByItem"/>.
    /// </summary>
    /// <remarks>
    /// The comparison is based on the table and column tags only and ignores the sort direction.
    /// </remarks>
    /// <param name="other">The <see cref="IOrderByItem"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the table and column tags are equal; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(IOrderByItem? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return TableTag.Equals(other.TableTag)
            && ColumnTag.Equals(other.ColumnTag);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// <see cref="IOrderByItem"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is an <see cref="IOrderByItem"/>
    /// and represents the same table and column; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
        => Equals(obj as IOrderByItem);

    /// <summary>
    /// Serves as the default hash function for <see cref="IOrderByItem"/>.
    /// The hash code is computed from the <see cref="TableTag"/> and <see cref="ColumnTag"/> values.
    /// </summary>
    /// <returns>
    /// An integer hash code based on the table and column tags.
    /// </returns>
    public override int GetHashCode()
        => HashCode.Combine(TableTag, ColumnTag);

    /// <summary>
    /// In this context Contains is basically an alias to determine if this single item equals the item being passed in.
    /// This is because an order by item is being modeled as a single item enumeration, so an OrderBy or OrderByItem can be used interchangeably in query generations.
    /// Therefore, I wanted this method to use the same logic the Contains method uses to determine equality in an IEnumerable, just applied to a single item.
    /// </summary>
    /// <param name="orderByItem"></param>
    /// <returns>true if equal, else false.</returns>
    public bool Contains(IOrderByItem orderByItem) =>
        EqualityComparer<IOrderByItem>.Default.Equals(this, orderByItem);

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
    public bool IsEmpty() =>
        ColumnTag.IsEmpty();

    /// <summary>
    /// Creates a new order by clause with a <see cref="IOrderByItem"> appended. This operation is immutable to the original object.
    /// </summary>
    /// <param name="orderByItem">Represents another individual order by item consisting of a table, column and sort direction.</param>
    /// <returns>Returns a new order by clause with a <see cref="IOrderByItem"> appended.</returns>
    public OrderBy WithAppend(IOrderByItem orderByItem) =>
        new (new List<IOrderByItem>([this, orderByItem]));

    /// <summary>
    /// Creates a new <c>ORDER BY</c> clause with the specified <see cref="IOrderByItem"/> appended.  
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="orderByItem">
    /// The <see cref="IOrderByItem"/> to append, representing a table, column, and sort direction.
    /// </param>
    /// <returns>
    /// A new <c>ORDER BY</c> clause that includes the appended <see cref="IOrderByItem"/>.
    /// </returns>
    public OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems) =>
        new (orderByItems.Prepend(this));

    /// <summary>
    /// Returns a new <see cref="OrderBy"/> instance that represents this object.
    /// </summary>
    /// <returns>
    /// A new <see cref="OrderBy"/> instance created from this object.
    /// </returns>
    public OrderBy AsOrderBy() =>
        new (this);
}
