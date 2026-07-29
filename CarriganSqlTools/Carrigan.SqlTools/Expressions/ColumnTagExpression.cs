using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents an already resolved column tag as a leaf SQL expression.
/// </summary>
/// <remarks>
/// This is used when reflection has already resolved a projected column, including
/// SelectTagAttribute mappings where the result model
/// property points at a column from a different table.
/// </remarks>
internal sealed class ColumnTagExpression : SqlExpression
{
    /// <summary>
    /// The resolved column tag represented by this expression.
    /// </summary>
    internal ColumnTag ColumnTag { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnTagExpression"/> class.
    /// </summary>
    /// <param name="columnTag">The resolved column tag to represent.</param>
    internal ColumnTagExpression(ColumnTag columnTag) : base([], columnTag)
    {
        ArgumentNullException.ThrowIfNull(columnTag, nameof(columnTag));
        ColumnTag = columnTag;
    }

    /// <summary>
    /// Gets the table represented by this column leaf expression.
    /// </summary>
    public override IEnumerable<TableTag> LeafTables =>
        [ColumnTag.TableTag];

    /// <summary>
    /// Produces the SQL fragment represented by this column expression.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render identifiers.</param>
    /// <returns>The SQL fragment for the resolved column tag.</returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return ColumnTag;
    }

    /// <summary>
    /// Returns the unquoted column tag representation.
    /// </summary>
    public override string ToString() =>
        ColumnTag.ToString();
}
