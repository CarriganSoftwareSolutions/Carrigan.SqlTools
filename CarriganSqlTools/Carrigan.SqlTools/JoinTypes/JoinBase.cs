using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Serves as the abstract base class for all SQL <c>JOIN</c> operation types,
/// including <c>INNER JOIN</c>, <c>LEFT JOIN</c>, <c>RIGHT JOIN</c>, and <c>FULL JOIN</c>.
/// </summary>
/// <remarks>
/// This class encapsulates shared logic for representing a join clause,
/// including access to predicates, involved table tags, and parameter resolution.
/// </remarks>
public abstract class JoinBase
{
    protected readonly Predicates _predicates;

    /// <summary>
    /// Initializes a new instance of the <see cref="JoinBase"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The predicate(s) used to define the <c>ON</c> condition for the <c>JOIN</c> clause.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicates"/> is <c>null</c>.
    /// </exception>
    protected JoinBase(Predicates predicates)
    {
        ArgumentNullException.ThrowIfNull(predicates, nameof(predicates));

        _predicates = predicates;
    }

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the right-hand (joined) table
    /// in the SQL <c>JOIN</c> clause.
    /// </summary>
    internal abstract TableTag TableTag { get; }

    /// <summary>
    /// Gets an enumeration of all <see cref="TableTag"/> objects involved in the
    /// current join's predicate logic.
    /// </summary>
    /// <remarks>
    /// Each <see cref="TableTag"/> represents a table referenced within the join's <c>ON</c> clause.
    /// When this join type does not use predicates, this returns an empty sequence.
    /// </remarks>
    internal IEnumerable<TableTag> JoinsOn =>
        _predicates.DescendantColumns
            .Select(static column => column.ColumnInfo.ColumnTag.TableTag)
            .Distinct();

    /// <summary>
    /// Generates the SQL string representing the specific <c>JOIN</c> clause.
    /// </summary>
    /// <param name="branchPrefix">
    /// The branch name used when generating predicate SQL and parameter tags via
    /// <see cref="Predicates.ToSqlFragments(string)"/>.
    /// </param>
    /// <returns>
    /// A SQL string representing the complete <c>JOIN</c> clause for this relationship.
    /// </returns>
    /// <remarks>
    /// For join types that require an <c>ON</c> clause, the derived implementation typically throws when no predicates exist.
    /// For join types that do not use predicates (for example, <c>CROSS JOIN</c>), the derived implementation should ignore
    /// <paramref name="branchPrefix"/>.
    /// </remarks>
    internal abstract string ToSql(string branchPrefix);

    /// <summary>
    /// Generates a sequence of SQL fragments that represent the current object.
    /// </summary>
    /// <param name="branchPrefix"> 
    /// The branch name used when generating predicate SQL and parameter tags via
    /// <see cref="Predicates.ToSqlFragments(string)"/>.
    /// </param>
    /// <returns>An enumerable collection of <see cref="SqlFragment"/> objects that compose the SQL representation of this
    /// instance.</returns>
    internal abstract IEnumerable<SqlFragment> ToSqlFragments(string branchPrefix);

    /// <summary>
    /// Retrieves all parameters associated with the predicate logic of this join.
    /// </summary>
    /// <param name="branchPrefix">
    /// The branch name used when generating predicate SQL and parameter tags via
    /// <see cref="Predicates.ToSqlFragments(string)"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Dictionary{TKey, TValue}"/> mapping each <see cref="ParameterTag"/>
    /// to its corresponding runtime value, for use in parameterized SQL queries.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="branchPrefix"/> is <c>null</c>.
    /// </exception>
    internal Dictionary<ParameterTag, object> GetParameters(string branchPrefix)
    {
        ArgumentNullException.ThrowIfNull(branchPrefix);

        return _predicates.ToSqlFragments(branchPrefix).GetParameters();
    }
}
