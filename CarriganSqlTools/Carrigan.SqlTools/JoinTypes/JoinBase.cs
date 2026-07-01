using Carrigan.SqlTools.Dialects;
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
    /// <summary>
    /// Gets the predicate used to relate the joined table to the previous table source.
    /// </summary>
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
        _predicates
            .DescendantLeafTables
            .Distinct();

    /// <summary>
    /// Generates a sequence of SQL fragments that represent the current object.
    /// </summary>
    /// <param name="dialect"></param>
    /// <returns>An enumerable collection of <see cref="ISqlFragment"/> objects that compose the SQL representation of this
    /// instance.</returns>
    ///
    internal abstract IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect);
}
