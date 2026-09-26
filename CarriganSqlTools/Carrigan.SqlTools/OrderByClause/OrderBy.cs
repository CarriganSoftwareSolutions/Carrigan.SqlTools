using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByClause;

/// <summary>
/// Represents a expression within a SQL <c>ORDER BY</c> clause.
/// </summary>
public class OrderBy : ISqlFragment, IEquatable<OrderBy>
{
    /// <summary>
    /// Gets the <see cref="SqlExpression"/> associated with this item.
    /// </summary>
    public SqlExpression SqlExpression { get; }

    /// <summary>
    /// Gets the <see cref="IEnumerable{TableTag}"/> associated with this itemn.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        SqlExpression.LeafTables;

    /// <summary>
    /// Gets the sort direction for this item.
    /// </summary>
    public SortDirectionEnum SortDirection { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderBy"/> class with the specified sort direction.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to order by.</param>
    /// <param name="sortDirection">The sort direction to apply (defaults to <see cref="SortDirectionEnum.Ascending"/>).</param>
    public OrderBy(SqlExpression sqlExpression, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression, nameof(sqlExpression));

        SqlExpression = sqlExpression;
        SortDirection = sortDirection;
    }

    /// <summary>
    /// Determines whether the current instance is equal to another <see cref="OrderBy"/>.
    /// </summary>
    /// <remarks>
    /// Equality compares only the underlying <see cref="SqlExpression"/> and intentionally ignores <see cref="SortDirection"/>.
    /// </remarks>
    /// <param name="other">The <see cref="OrderBy"/> to compare with this instance.</param>
    /// <returns><c>true</c> if both items refer to the same table and column; otherwise, <c>false</c>.</returns>
    public bool Equals(OrderBy? other)
    {
        if (ReferenceEquals(this, other)) return true;
        else if (other is null) return false;
        else return SqlExpression == other.SqlExpression;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if <paramref name="obj"/> is an equal <see cref="OrderBy"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as OrderBy);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>An integer hash code for the current object.</returns>
    public override int GetHashCode() =>

        HashCode.Combine(SqlExpression);

    /// <summary>
    /// Yields the instance as a single-item sequence of <see cref="ISqlFragment"/>.
    /// </summary>
    /// <returns>A sequence of <see cref="ISqlFragment"/> containing the instance.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {
        foreach (ISqlFragment sqlFragment in SqlExpression.ToSqlFragments(dialect))
            yield return sqlFragment;
        yield return ISqlFragment.Space;
        yield return SortDirection.ToSqlFragment();
    }

    /// <summary>
    /// Gets the SQL fragment parameters referenced by the SQL fragment.
    /// </summary>
    /// <remarks>Enumeration is deferred; callers should materialize the sequence if it will be iterated
    /// multiple times or accessed concurrently.</remarks>
    /// <returns>A sequence of <see cref="SqlFragmentParameter"/> values referenced by the SQL fragment; empty if there are no parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        SqlExpression.GetSqlFragmentParameters(dialect);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString() =>
        Flatten(NeutralDialect.Instance).ToSql(NeutralDialect.Instance);

    /// <summary>
    /// Renders the SQL fragment using the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the SQL fragment.
    /// </param>
    /// <returns>
    /// A string that represents the SQL fragment in the specified dialect.
    /// </returns>
    public string ToSql(ISqlDialects dialect) =>
        Flatten(dialect).ToSql(dialect);
}
