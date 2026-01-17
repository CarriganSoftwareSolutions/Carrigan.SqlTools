using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Base class for predicate nodes that reference a single column.
/// Carries the resolved <see cref="ColumnInfo"/> and exposes the owning <see cref="TableTag"/>.
/// </summary>
/// <remarks>
/// This class exists to centralize column/table metadata for predicate nodes.
/// </remarks>
public abstract class ColumnBase : Predicates
{
    /// <summary>
    /// Gets the resolved column metadata (name, tags, etc.) used by the predicate.
    /// </summary>
    internal ColumnInfo ColumnInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnBase"/> class.
    /// </summary>
    /// <param name="columnInfo">The resolved column metadata.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="columnInfo"/> is <c>null</c>.
    /// </exception>
    protected ColumnBase(ColumnInfo columnInfo) : base([])
    {
        ArgumentNullException.ThrowIfNull(columnInfo, nameof(columnInfo));
        ColumnInfo = columnInfo;
    }

    /// <summary>
    /// Gets the table tag that owns this column.
    /// </summary>
    internal TableTag TableTag => ColumnInfo.ColumnTag.TableTag;
}