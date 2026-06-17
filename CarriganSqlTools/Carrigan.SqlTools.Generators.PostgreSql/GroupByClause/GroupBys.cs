using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents the concrete collection of <c>GROUP BY</c> items for the dialect package.
/// </summary>
public class GroupBys : GroupBysBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBys"/> class.
    /// </summary>
    public GroupBys() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBys"/> class.
    /// </summary>
    /// <param name="_groupByItems">The group-by items to include in the collection.</param>
    public GroupBys(params IEnumerable<GroupByBase> _groupByItems) : base(_groupByItems)
    {
    }

    /// <summary>
    /// Creates a new collection with the supplied item appended.
    /// </summary>
    /// <param name="groupByItem">The GROUP BY item to append.</param>
    /// <returns>A new collection containing the existing group-by items followed by <paramref name="groupByItem"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public override GroupBys Append(GroupByBase groupByItem)
    {
        ArgumentNullException.ThrowIfNull(groupByItem, nameof(groupByItem));

        return new GroupBys(_groupByItems.Append(groupByItem));
    }

    /// <summary>
    /// Appends an group-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>A new collection containing the additional group-by item.</returns>
    public override GroupBys Append<T>(PropertyName propertyName) =>
        new (_groupByItems.Append(new GroupBy<T>(propertyName)));

    /// <summary>
    /// Appends an group-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>A new collection containing the additional group-by item.</returns>
    public override GroupBys Append<T>(string propertyName) =>
        Append<T>(new PropertyName(propertyName));

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="groupByItems">The GROUP BY items to append.</param>
    /// <returns>A new collection containing the existing items followed by the supplied group-by items.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public override GroupBys Concat(params IEnumerable<GroupByBase> groupByItems)
    {
        ArgumentNullException.ThrowIfNull(groupByItems, nameof(groupByItems));

        return new GroupBys (_groupByItems.Concat(groupByItems));
    }

    /// <summary>
    /// Returns this instance as the concrete <see cref="GroupBys"/> type.
    /// </summary>
    public override GroupBys AsGroupBy() =>
        this;



    /// <summary>
    /// Defines an implicit conversion from <see cref="GroupByBase"/> to <see cref="GroupBysBase"/>,
    /// </summary>
    /// <param name="groupByItemBase">
    /// The <see cref="GroupByBase"/> instance to convert. The resulting <see cref="GroupBysBase"/> will contain this single item.
    /// </param>
    public static implicit operator GroupBys(GroupByBase groupByItemBase) =>
        (new GroupBys (groupByItemBase));

    /// <summary>
    /// Creates a new <c>GROUP BY</c> clause containing a single item for the specified
    /// property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The entity/model type that defines the table containing the property to group by.
    /// </typeparam>
    /// <param name="propertyName">The property representing the column to group by.</param>
    /// <returns>
    /// A new <see cref="GroupBysBase"/> instance containing one <see cref="GroupBy{T}"/> item.
    /// </returns>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public static GroupBys New<T>(PropertyName propertyName) where T : class =>
        new (new GroupBy<T>(propertyName));

    /// <summary>
    /// Creates a new <c>GROUP BY</c> clause containing a single item for the specified
    /// property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The entity/model type that defines the table containing the property to group by.
    /// </typeparam>
    /// <param name="propertyName">The name of the property representing the column to group by.</param>
    /// <returns>
    /// A new <see cref="GroupBysBase"/> instance containing one <see cref="GroupBy{T}"/> item.
    /// </returns>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public static GroupBys New<T>(string propertyName) where T : class =>
        GroupBys.New<T>(new PropertyName(propertyName));


    /// <summary>
    /// Gets an empty <c>GROUP BY</c> clause.
    /// </summary>
    public static GroupBys Empty =>
        new ();
}
