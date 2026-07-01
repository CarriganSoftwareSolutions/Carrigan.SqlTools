using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Base class for SQL expression nodes that reference a single reflected table column.
/// Carries the resolved <see cref="ColumnInfo"/> and exposes the owning <see cref="TableTag"/>.
/// </summary>
/// <remarks>
/// This class exists to centralize column/table metadata for expression nodes. Predicate types can consume
/// these column expressions when building SQL <c>WHERE</c>, <c>JOIN</c>, and other expression-bearing clauses.
/// </remarks>
public abstract class ColumnBase : SqlExpression
{
    /// <summary>
    /// Gets the resolved column metadata (name, tags, etc.) used by the expression.
    /// </summary>
    internal ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnBase"/> class.
    /// </summary>
    /// <param name="columnInfo">The resolved column metadata.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="columnInfo"/> is <c>null</c>.
    /// </exception>
    protected ColumnBase(ColumnInfo columnInfo) : base([], columnInfo)
    {
        ArgumentNullException.ThrowIfNull(columnInfo, nameof(columnInfo));
        ColumnInfo = columnInfo;
    }

    /// <summary>
    /// Gets the table tag represented by this column leaf expression.
    /// </summary>
    public override IEnumerable<TableTag> LeafTables =>
        [ColumnInfo.ColumnTag.TableTag];

    /// <summary>
    /// Indicates whether this column is valid in an aggregate SELECT list for the supplied <c>GROUP BY</c> clause.
    /// </summary>
    /// <param name="groupBys">The <c>GROUP BY</c> clause to check.</param>
    /// <returns><c>true</c> when this column is included in <paramref name="groupBys"/>; otherwise, <c>false</c>.</returns>
    public override bool IsAggregate(GroupBysBase? groupBys) =>
        groupBys?.Contains(this) ?? false;

    /// <summary>
    /// Returns the unquoted column tag representation.
    /// </summary>
    public override string ToString() =>
        ColumnInfo.ColumnTag.ToString();
}