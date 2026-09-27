using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System.Numerics;

namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents a single item in a SQL GROUP BY clause, encapsulating the SQL expression used for grouping.
/// </summary>
public class GroupBy : IEquatable<GroupBy>, IEqualityOperators<GroupBy, GroupBy, bool>, ISqlFragment
{
    /// <summary>
    /// Gets the SQL expression used as this grouping key.
    /// </summary>
    public SqlExpression SqlExpression { get; }

    /// <summary>
    /// Gets every table participating in this grouping expression.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        SqlExpression.AllParticipatingTables;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBy"/> class with the specified SQL expression.
    /// </summary>
    /// <param name="sqlExpression">
    /// The SQL expression that defines the column or expression to group by.
    /// </param>
    public GroupBy(SqlExpression sqlExpression)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression);
        SqlExpression = sqlExpression;
    }

    /// <summary>
    /// Returns a string representation of the SQL fragment for this group-by item.
    /// </summary>
    /// <returns>
    /// A SQL string representing this item, e.g., <c>[Order].[OrderDate]</c>.
    /// </returns>
    public override string ToString() =>
        ToSql(NeutralDialect.Instance);


    /// <summary>
    /// Determines whether the specified <see cref="GroupBy"/> instance is equal to the current instance.
    /// </summary>
    /// <param name="other">
    /// The <see cref="GroupBy"/> instance to compare with the current instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="GroupBy"/> instance is equal to the current instance; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(GroupBy? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return SqlExpression.Equals(other.SqlExpression);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with the current instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified object is equal to the current instance; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object? obj) =>
        Equals(obj as GroupBy);

    /// <summary>
    /// Determines whether the specified SQL expression is equivalent to the current instance.
    /// </summary>
    /// <param name="sqlExpression">
    /// The SQL expression to compare with the current instance.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified SQL expression is equivalent to the current instance; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equivalent(SqlExpression sqlExpression) =>
        SqlExpression.Equals(sqlExpression);

    /// <summary>
    /// Returns a hash code for this group-by item.
    /// </summary>
    /// <returns>A hash code consistent with <see cref="Equals(GroupBy?)"/>.</returns>
    public override int GetHashCode() =>
        SqlExpression.GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="GroupBy"/> instances are equal.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="GroupBy"/> instance to compare.
    /// </param>
    /// <param name="right">
    /// The second <see cref="GroupBy"/> instance to compare.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="GroupBy"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(GroupBy? left, GroupBy? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="GroupBy"/> instances are not equal.
    /// </summary>
    /// <param name="left">
    /// The first <see cref="GroupBy"/> instance to compare.
    /// </param>
    /// <param name="right">
    /// The second <see cref="GroupBy"/> instance to compare.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="GroupBy"/> instances are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(GroupBy? left, GroupBy? right) =>
        !(left == right);

    /// <summary>
    /// Flattens the grouping expression into the leaf SQL fragments that render the expression.
    /// </summary>
    /// <remarks>
    /// Unlike the former column-only group-by implementation, this method does not return the
    /// <see cref="GroupBy"/> wrapper itself. Returning the expression leaves allows nested parameters
    /// to participate in query-wide parameter ordering and final dialect-specific parameter naming.
    /// </remarks>
    /// <param name="dialect">The SQL dialect used to render dialect-dependent fragments.</param>
    /// <returns>The flattened leaf SQL fragments that render the grouping expression.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect) =>
        SqlExpression.ToSqlFragments(dialect).Flatten(dialect);

    /// <summary>
    /// Gets the SQL fragment parameters referenced by the SQL fragment.
    /// </summary>
    /// <remarks>Enumeration is deferred; callers should materialize the sequence if it will be iterated
    /// multiple times or accessed concurrently.</remarks>
    /// <returns>A sequence of <see cref="SqlFragmentParameter"/> values referenced by the SQL fragment; empty if there are no parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        SqlExpression.GetSqlFragmentParameters(dialect);

    /// <summary>
    /// Generates the SQL fragment for this group-by item (without the <c>GROUP BY</c> keyword).
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render the fragment.</param>
    /// <remarks>
    /// The containing <see cref="SqlGenerators.SqlGeneratorBase{T}"/> is responsible for emitting the
    /// <c>GROUP BY</c> keyword and for joining multiple items.
    /// </remarks>
    /// <returns>A SQL string representing this item, e.g., <c>[Order].[OrderDate]</c>.</returns>
    public string ToSql(ISqlDialects dialect) =>
        Flatten(dialect).ToSql(dialect) ?? string.Empty;
}
