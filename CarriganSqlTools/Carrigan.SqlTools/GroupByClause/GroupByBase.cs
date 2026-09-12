using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Numerics;

namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents a single-column specification within a SQL <c>GROUP BY</c> clause.
/// </summary>
/// <remarks>
/// Equality and hashing are based on the grouped column's <see cref="ColumnTag"/> identity.
/// </remarks>
public abstract class GroupByBase :
    IEquatable<GroupByBase>,
    IEqualityOperators<GroupByBase, GroupByBase, bool>,
    ISqlFragment
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
    public GroupByBase(ColumnInfo columnInfo)
    {
        ArgumentNullException.ThrowIfNull(columnInfo);
        ColumnInfo = columnInfo;
    }

    /// <summary>
    /// Returns the qualified column name represented by this group-by item.
    /// </summary>
    public override string ToString() =>
        ColumnInfo.ColumnTag.ToString();

    /// <summary>
    /// Determines whether this instance groups the same column as another <see cref="GroupByBase"/>.
    /// </summary>
    /// <param name="other">The other group-by item to compare.</param>
    /// <returns><c>true</c> when both items identify the same column; otherwise, <c>false</c>.</returns>
    public bool Equals(GroupByBase? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return ColumnInfo.ColumnTag.Equals(other.ColumnInfo.ColumnTag);
    }

    /// <summary>
    /// Determines whether the specified object groups the same column as this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when <paramref name="obj"/> is an equivalent <see cref="GroupByBase"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as GroupByBase);

    /// <summary>
    /// Returns a hash code for this group-by item.
    /// </summary>
    /// <returns>A hash code consistent with <see cref="Equals(GroupByBase?)"/>.</returns>
    public override int GetHashCode() =>
        ColumnInfo.ColumnTag.GetHashCode();

    /// <summary>
    /// Determines whether two group-by items identify the same column.
    /// </summary>
    public static bool operator ==(GroupByBase? left, GroupByBase? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two group-by items identify different columns.
    /// </summary>
    public static bool operator !=(GroupByBase? left, GroupByBase? right) =>
        !(left == right);

    /// <summary>
    /// Yields the instance as a single-item sequence of <see cref="ISqlFragment"/>.
    /// </summary>
    /// <returns>A sequence of <see cref="ISqlFragment"/> containing the instance.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        [this];

    /// <summary>
    /// Gets the SQL fragment parameters referenced by the SQL fragment.
    /// </summary>
    /// <remarks>Enumeration is deferred; callers should materialize the sequence if it will be iterated
    /// multiple times or accessed concurrently.</remarks>
    /// <returns>A sequence of <see cref="SqlFragmentParameter"/> values referenced by the SQL fragment; empty if there are no parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
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
