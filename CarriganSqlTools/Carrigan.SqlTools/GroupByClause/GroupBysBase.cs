using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Tags;


namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Concrete implementation of <see cref="GroupBysBase"/> for an <c>GROUP BY</c>
/// clause that supports multiple columns.
/// </summary>
public abstract class GroupBysBase
{

    /// <summary>
    /// Holds all parts of the <c>GROUP BY</c> clause, with one <see cref="GroupByBase"/>
    /// for each individual column.
    /// </summary>
    protected readonly IEnumerable<GroupByBase> _groupByItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBysBase"/> class,
    /// representing an <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="groupByItems">
    /// The <see cref="GroupByBase"/> objects defining the columns.
    /// for the <c>GROUP BY</c> clause.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="groupByItems"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="groupByItems"/> contains disallowed <c>null</c> values.
    /// </exception>
    public GroupBysBase(params IEnumerable<GroupByBase> groupByItems)
    {
        ArgumentNullException.ThrowIfNull(groupByItems, nameof(groupByItems));

        _groupByItems = groupByItems.Materialize(NullOptionsEnum.Exception);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="groupByItem"/> is present
    /// in the <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="groupByItem">The individual group-by item to check.</param>
    /// <returns>
    /// <c>true</c> if the item is contained in this <c>GROUP BY</c>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="groupByItem"/> is <c>null</c>.
    /// </exception>
    public bool Contains(GroupByBase groupByItem)
    {
        ArgumentNullException.ThrowIfNull(groupByItem, nameof(groupByItem));

        return _groupByItems.Contains(groupByItem);
    }

    /// <summary>
    /// Determines whether a column expression is present in the <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="column">The column expression to check.</param>
    /// <returns><c>true</c> when the column appears in the <c>GROUP BY</c>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="column"/> is <c>null</c>.
    /// </exception>
    public bool Contains(ColumnBase column)
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));

        return _groupByItems.Any(groupByItem => groupByItem.ColumnInfo.ColumnTag == column.ColumnInfo.ColumnTag);
    }

    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects referenced in the <c>GROUP BY</c> clause.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        _groupByItems.Select(static item => item.TableTag);

    /// <summary>
    /// Determines whether the <c>GROUP BY</c> clause is empty.
    /// </summary>
    public bool IsEmpty() =>
        _groupByItems.IsNullOrEmpty();

    /// <summary>
    /// Creates a new <c>GROUP BY</c> clause with the specified <see cref="GroupByBase"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="groupByItem">
    /// The <see cref="GroupByBase"/> to append, representing a table, column.
    /// </param>
    /// <returns>
    /// A new <c>GROUP BY</c> clause that includes the appended <see cref="GroupByBase"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="groupByItem"/> is <c>null</c>.
    /// </exception>
    public abstract GroupBysBase Append(GroupByBase groupByItem);

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name.
    /// Then creates a new <c>GROUP BY</c> clause with
    /// the specified <see cref="GroupByBase"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The property representing the column to group by.</param>
    /// <returns>
    /// A new <c>GROUP BY</c> clause that includes the appended <see cref="GroupByBase"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    public abstract GroupBysBase Append<T>(PropertyName propertyName) where T : class;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBy{T}"/> class,
    /// specifying the table type <typeparamref name="T"/>, the property name.
    /// Then creates a new <c>GROUP BY</c> clause with
    /// the specified <see cref="GroupByBase"/> appended.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The property representing the column to group by.</param>
    /// <returns>
    /// A new <c>GROUP BY</c> clause that includes the appended <see cref="GroupByBase"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="propertyName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="Exceptions.InvalidPropertyException{T}">
    /// Thrown when <paramref name="propertyName"/> does not map to a valid column on <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the resolved column metadata does not contain exactly one match.
    /// </exception>
    [ExternalOnly]
    public virtual GroupBysBase Append<T>(string propertyName) where T : class =>
        Append<T>(new PropertyName(propertyName));

    /// <summary>
    /// Creates a new <c>GROUP BY</c> clause with the specified sequence of
    /// <see cref="GroupByBase"/> objects concatenated.
    /// This operation is immutable and does not modify the original instance.
    /// </summary>
    /// <param name="groupByItems">
    /// The <see cref="GroupByBase"/> objects to append to the clause.
    /// </param>
    /// <returns>
    /// A new <c>GROUP BY</c> clause that includes the concatenated items.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="groupByItems"/> is <c>null</c>.
    /// </exception>
    public abstract GroupBysBase Concat(params IEnumerable<GroupByBase> groupByItems);

    /// <summary>
    /// Returns this instance cast to the concrete implementation, <see cref="GroupBysBase"/>.
    /// </summary>
    /// <returns>
    /// This instance as an <see cref="GroupBysBase"/> object.
    /// </returns>
    public virtual GroupBysBase AsGroupBy() => this;

    /// <summary>
    /// Returns all contained <see cref="GroupByBase"/> objects.
    /// </summary>
    /// <returns>
    /// An enumeration of all <see cref="GroupByBase"/> objects contained in this instance.
    /// </returns>
    public IEnumerable<GroupByBase> AsEnumerable() =>
        _groupByItems;

    /// <summary>
    /// Generates the SQL <c>GROUP BY</c> clause represented by this instance.
    /// </summary>
    /// <returns>
    /// A SQL string for the <c>GROUP BY</c> clause, or <see cref="string.Empty"/>
    /// if no grouping is defined.
    /// </returns>
    internal string ToSql(ISqlDialects dialect) =>
        IsEmpty()
            ? string.Empty
            : $"GROUP BY {string.Join(", ", _groupByItems.Select(item => item.ToSql(dialect)))}";
}
