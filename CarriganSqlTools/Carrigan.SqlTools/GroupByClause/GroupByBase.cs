using Carrigan.Core.DataTypes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents a single-column specification within a SQL <c>GROUP BY</c> clause.
/// </summary>
public abstract class GroupByBase : StringWrapper, ISqlFragment
{
    /// <summary>
    /// Gets the <see cref="ColumnInfo"/> associated with this item.
    /// </summary>
    internal ColumnInfo ColumnInfo { get; init; }

    /// <summary>
    /// Gets the table tag associated with the grouped column.
    /// </summary>
    internal TableTag TableTag =>
        ColumnInfo.ColumnTag.TableTag;

    /// <summary>
    /// Initializes a grouped-column specification from reflected column metadata.
    /// </summary>
    /// <param name="columnInfo">The grouped column metadata.</param>
    public GroupByBase(ColumnInfo columnInfo) : base(columnInfo.ColumnTag) => 
        ColumnInfo = columnInfo;

    /// <summary>
    /// Yields the instance as a single-item sequence of <see cref="ISqlFragment"/>.
    /// </summary>
    /// <returns>A sequence of <see cref="ISqlFragment"/> containing the instance.</returns>
    public IEnumerable<ISqlFragment> Flatten() =>
        [this];

    /// <summary>
    /// Gets the SQL fragment parameters referenced by the SQL fragment.
    /// </summary>
    /// <remarks>Enumeration is deferred; callers should materialize the sequence if it will be iterated
    /// multiple times or accessed concurrently.</remarks>
    /// <returns>A sequence of <see cref="SqlFragmentParameter"/> values referenced by the SQL fragment; empty if there are no parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
        [];

    /// <summary>
    /// Generates the SQL fragment for this group-by item (without the <c>GROUP BY</c> keyword).
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render the fragment.</param>
    /// <remarks>
    /// The containing <see cref="SqlGenerators.SqlGeneratorBase{T}"/> is responsible for emitting the
    /// <c>GROUP BY</c> keyword and for joining multiple items.
    /// </remarks>
    /// <returns>A SQL string representing this item, e.g., <c>[Group].[GroupDate]</c>.</returns>
    public string ToSql(ISqlDialects dialect) =>
        ColumnInfo.ColumnTag.ToSql(dialect);
}
