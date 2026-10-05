using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;


namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents a SQL <c>GROUP BY</c> clause containing one or more grouping expressions.
/// </summary>
public class GroupBys
{

    /// <summary>
    /// Holds all parts of the <c>GROUP BY</c> clause, with one <see cref="GroupBy"/>
    /// for each individual grouping expression.
    /// </summary>
    internal readonly IEnumerable<GroupBy> GroupByItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBys"/> class.
    /// </summary>
    public GroupBys() : base() =>
        GroupByItems = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBys"/> class,
    /// representing an <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="groupByItems">
    /// The <see cref="GroupBy"/> objects defining the grouping expressions.
    /// for the <c>GROUP BY</c> clause.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="groupByItems"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="groupByItems"/> contains disallowed <c>null</c> values.
    /// </exception>
    public GroupBys(params IEnumerable<GroupBy> groupByItems)
    {
        ArgumentNullException.ThrowIfNull(groupByItems, nameof(groupByItems));

        GroupByItems = groupByItems.Materialize(NullOptionsEnum.ArgumentNullException);
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
    public bool Contains(GroupBy groupByItem)
    {
        ArgumentNullException.ThrowIfNull(groupByItem, nameof(groupByItem));

        return GroupByItems.Contains(groupByItem);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="sqlExpression"/> is represented by any of the grouping expressions.
    /// </summary>
    /// <param name="sqlExpression">
    /// The expression to check for representation in the <c>GROUP BY</c> clause.
    /// </param>
    /// <returns>
    /// <c>true</c> if the expression is represented by any grouping expression; otherwise, <c>false</c>.
    /// </returns>
    private bool ContainsEquivalent(SqlExpression sqlExpression) =>
        GroupByItems.Any(groupBy => groupBy.Equivalent(sqlExpression));

    /// <summary>
    /// Determines whether every non-aggregate portion of an expression is represented by the grouping keys.
    /// </summary>
    /// <remarks>
    /// Exact grouped sub-expressions satisfy the entire sub-tree. Aggregate sub-trees are intentionally ignored because
    /// their input columns do not need to be grouped. Other expressions are recursively validated until a grouped
    /// expression, aggregate expression, or column leaf is reached.
    /// </remarks>
    internal bool ContainsAllNonAggregateParts(SqlExpression sqlExpression)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression, nameof(sqlExpression));

        if (sqlExpression.IsAggregate() || ContainsEquivalent(sqlExpression))
            return true;
        
        else if (sqlExpression is Column)
            // if we reach this return, it is a non-aggregate column that is not contained in the group by
            return false;

        else
            return sqlExpression.ChildNodes.All(ContainsAllNonAggregateParts);
    }

    /// <summary>
    /// Indicates whether any grouping expression contains an aggregate expression.
    /// </summary>
    internal bool HasAggregates() =>
        GroupByItems.Any(static groupBy => groupBy.SqlExpression.HasAggregates());

    /// <summary>
    /// Determines whether a selected expression is validly represented by the current grouping keys.
    /// </summary>
    /// <remarks>
    /// A selected expression is represented when the complete expression is grouped, or when every column participating
    /// in the selected expression is independently present as a complete <c>GROUP BY</c> expression. Merely participating
    /// inside a larger grouping expression does not independently group a column.
    /// </remarks>
    /// <param name="selectTagBase">The selected expression to validate against the grouping keys.</param>
    /// <returns><c>true</c> when the selected expression is represented by the grouping keys; otherwise, <c>false</c>.</returns>
    internal bool ContainsEquivalent(SelectTag selectTagBase)
    {
        ArgumentNullException.ThrowIfNull(selectTagBase, nameof(selectTagBase));

        return ContainsAllNonAggregateParts(selectTagBase.SqlExpression);
    }
    public bool DoesNotContainsEquivalent(SelectTag selectTagBase) =>
        !ContainsEquivalent(selectTagBase);

    /// <summary>
    /// Determines whether a column expression is present in the <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="column">The column expression to check.</param>
    /// <returns><c>true</c> when the column appears in the <c>GROUP BY</c>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="column"/> is <c>null</c>.
    /// </exception>
    public bool Contains(Column column)
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));

        return GroupByItems.Any(groupBy =>
            groupBy.SqlExpression is Column groupedColumn
            && groupedColumn.ColumnInfo.ColumnTag.Equals(column.ColumnInfo.ColumnTag));
    }

    /// <summary>
    /// Enumerates all <see cref="TableTag"/> objects referenced in the <c>GROUP BY</c> clause.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        GroupByItems.SelectMany(static item => item.TableTags);

    /// <summary>
    /// Determines whether the <c>GROUP BY</c> clause is empty.
    /// </summary>
    public bool IsEmpty() =>
        GroupByItems.IsNullOrEmpty();

    /// <summary>
    /// Gets an empty <see cref="GroupBys"/> instance, representing no <c>GROUP BY</c> clause.
    /// </summary>
    public static GroupBys Empty =>
        new();


    /// <summary>
    /// Creates a new collection with the supplied item appended.
    /// </summary>
    /// <param name="groupByItem">The GROUP BY item to append.</param>
    /// <returns>A new collection containing the existing group-by items followed by <paramref name="groupByItem"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public GroupBys Append(GroupBy groupByItem)
    {
        ArgumentNullException.ThrowIfNull(groupByItem, nameof(groupByItem));

        return new GroupBys(GroupByItems.Append(groupByItem));
    }

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="groupByItems">The GROUP BY items to append.</param>
    /// <returns>A new collection containing the existing items followed by the supplied group-by items.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public GroupBys Concat(params IEnumerable<GroupBy> groupByItems)
    {
        ArgumentNullException.ThrowIfNull(groupByItems, nameof(groupByItems));

        return new GroupBys(GroupByItems.Concat(groupByItems));
    }

    /// <summary>
    /// Returns this instance cast to the concrete implementation, <see cref="GroupBys"/>.
    /// </summary>
    /// <returns>
    /// This instance as an <see cref="GroupBys"/> object.
    /// </returns>
    [Obsolete("This should now be a pointless method.")]
    public virtual GroupBys AsGroupBy() => this;

    /// <summary>
    /// Returns all contained <see cref="GroupBy"/> objects.
    /// </summary>
    /// <returns>
    /// An enumeration of all <see cref="GroupBy"/> objects contained in this instance.
    /// </returns>
    public IEnumerable<GroupBy> AsEnumerable() =>
        GroupByItems;

    /// <summary>
    /// Returns the SQL fragments that make up this <c>GROUP BY</c> clause.
    /// </summary>
    /// <remarks>
    /// The clause is kept as fragments until final query rendering so parameters embedded in grouping expressions
    /// remain discoverable by <see cref="SqlGenerators.SqlQuery.Parameters"/> and receive query-wide parameter ordering.
    /// </remarks>
    /// <returns>
    /// The fragments for the complete <c>GROUP BY</c> clause, or an empty sequence when no grouping is defined.
    /// </returns>
    internal IEnumerable<ISqlFragment> ToSqlFragments()
    {
        if (IsEmpty())
            yield break;

        yield return new SqlFragmentText("GROUP BY ");

        foreach (ISqlFragment fragment in GroupByItems.JoinFragments(ISqlFragment.CommaSpace))
            yield return fragment;
    }

    /// <summary>
    /// Generates the SQL <c>GROUP BY</c> clause represented by this instance.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render the clause.</param>
    /// <returns>
    /// A SQL string for the <c>GROUP BY</c> clause, or <see cref="string.Empty"/>
    /// if no grouping is defined.
    /// </returns>
    internal string ToSql(ISqlDialects dialect) =>
        ToSqlFragments().ToSql(dialect);


    /// <summary>
    /// Defines an implicit conversion from <see cref="GroupBy"/> to <see cref="GroupBys"/>,
    /// </summary>
    /// <param name="groupByItemBase">
    /// The <see cref="GroupBy"/> instance to convert. The resulting <see cref="GroupBys"/> will contain this single item.
    /// </param>
    public static implicit operator GroupBys(GroupBy groupByItemBase) =>
        (new GroupBys(groupByItemBase));
}
