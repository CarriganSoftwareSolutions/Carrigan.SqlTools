using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Represents a single-column specification within a SQL <c>ORDER BY</c> clause.
/// Also implements <see cref="OrderBys"/> to make single-column order-by usage convenient.
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
/// ORDER BY [Customer].[Name] ASC
/// ]]></code>
/// </example>
public class OrderBy<T> : OrderByBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public OrderBy(PropertyName propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        : base(sortDirection) =>
        ColumnInfo = SqlToolsReflectorCache<T>.GetColumnsFromProperties(propertyName).Single();

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name,
    /// and the desired sort direction.
    /// </summary>
    /// <param name="propertyName">The property representing the column to order by.</param>
    /// <param name="sortDirection">The sort direction to apply.</param>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    [ExternalOnly]
    public OrderBy(string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        : this(new PropertyName(propertyName), sortDirection)
    {
    }

    /// <summary>
    /// The <see cref="Tags.ColumnInfo"/> that specifies the column being ordered.
    /// </summary>
    internal override ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Part of the <see cref="OrderBys"/> implementation.
    /// Returns the single <see cref="TableTag"/> involved in this order-by item.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        [TableTag];

    /// <summary>
    /// Determines whether the specified object is equal to the current
    /// <see cref="OrderByBase"/> instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is an <see cref="OrderByBase"/>
    /// that represents the same table/column; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) =>
        Equals(obj as OrderByBase);

    /// <summary>
    /// Serves as the default hash function for <see cref="OrderByBase"/>.
    /// </summary>
    /// <returns>
    /// An integer hash code computed from the <see cref="TableTag"/> and <see cref="ColumnInfo"/>.
    /// </returns>
    public override int GetHashCode() =>
        HashCode.Combine(TableTag, ColumnInfo);
}
