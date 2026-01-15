using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;
/// <summary>
/// Base class for predicate nodes that reference a single column.
/// Carries the resolved <see cref="ReflectorCache.ColumnInfo"/> and exposes the owning <see cref="Tags.TableTag"/>.
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
    protected ColumnBase(ColumnInfo columnInfo) : base ([]) =>
        ColumnInfo = columnInfo;

    /// <summary>
    /// Gets the table tag that owns this column.
    /// </summary>
    internal TableTag TableTag => 
        ColumnInfo.ColumnTag.TableTag;
}