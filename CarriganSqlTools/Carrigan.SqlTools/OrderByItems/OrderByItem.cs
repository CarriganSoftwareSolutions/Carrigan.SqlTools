using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// This represents a part of an order by clause for an individual column in SQL
/// It also implements the <see cref="IOrderByClause"/> to reduce the amount of code needed to create a single column order by.
/// </summary>
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
    /// Constructor, provide the table with the Type <see cref="T"/> and the property name that represents the column name and sort direction as parameters.
    /// </summary>
    /// <param name="propertyName">The name of the parameter that represents the column to use.</param>
    /// <param name="sortDirection">The sort direction to use.</param>
    public OrderByItem(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
    {
        SqlToolsReflectorCache<T>.ValidateEntityPropertyNames(propertyName);
        ColumnTag = new(TableTag, propertyName);
        SortDirection = sortDirection;
    }

    public ColumnTag ColumnTag { get; private set; }

    /// <summary>
    /// Returns a TableTag representing the table being sorted on.
    /// </summary>
    public TableTag TableTag 
        => SqlToolsReflectorCache<T>.Table;

    /// <summary>
    /// Get the sort direction being used.
    /// </summary>
    public SortDirectionEnum SortDirection { get; private set; }

    /// <summary>
    /// This is part of the <see cref="IOrderByClause"/>. 
    /// In the context of this class, this returns the name of the table involved in the sort as a Enumerable.
    /// </summary>
    public IEnumerable<TableTag> TableTags => 
        [TableTag];

    /// <summary>
    /// Returns the SQL as a string for the given instance.
    /// Note: <see cref="OrderByItem{T}"/> unlike <see cref="OrderBy"/> does not include the "ORDER BY" text which has to be added in by the <see cref="SqlGenerator{T}"/>
    /// </summary>
    /// <returns>Returns the SQL as a string for the given instance.</returns>
    /// <example>[Order].[OrderDate] ASC</example>
    public string ToSql() =>
        $"{ColumnTag} {SortDirection.ToSql()}";

    /// <summary>
    /// Equals method required for <see cref="IEquatable{IOrderByItem}"/>. 
    /// Note: It is sort direction insensitive.
    /// </summary>
    /// <param name="other">The other instance being compared to.</param>
    /// <returns>true or false based on if the Table and Column tags are equal.</returns>
    public bool Equals(IOrderByItem? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return TableTag.Equals(other.TableTag)
            && ColumnTag.Equals(other.ColumnTag);
    }

    /// <summary>
    /// Equals method required for <see cref="IEquatable{IOrderByItem}"/>. 
    /// </summary>
    /// <param name="other">The other instance being compared to.</param>
    public override bool Equals(object? obj)
        => Equals(obj as IOrderByItem);

    /// <summary>
    /// GetHashCode method required for <see cref="IEquatable{IOrderByItem}"/>. 
    /// The Hash Value uses the TableTag and ColumnTag as part of the hash.
    /// </summary>
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
    /// The item is considered empty if the Column name is null, empty or whitespace.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
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
    /// Creates a new order by clause with a <see cref="IEnumerable<IOrderByItem>"> concatenated. This operation is immutable to the original object.
    /// </summary>
    /// <param name="orderByItem">Represents another individual order by item consisting of a table, column and sort direction.</param>
    /// <returns>Returns a new order by clause with a <see cref="IEnumerable<IOrderByItem>"> concatenated.</returns>
    public OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems) =>
        new (orderByItems.Prepend(this));

    /// <summary>
    /// Returns this object cast as the concrete implementation <see cref="OrderBy"/>
    /// </summary>
    /// <returns>Returns this object cast as the concrete implementation <see cref="OrderBy"/></returns>
    public OrderBy AsOrderBy() =>
        new (this);
}
