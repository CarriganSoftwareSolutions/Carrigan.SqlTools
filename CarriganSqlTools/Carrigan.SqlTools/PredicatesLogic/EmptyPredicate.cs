using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents an empty predicate that emits no SQL and has no parameters.
/// Used for join types that do not have an <c>ON</c> clause (for example, <c>CROSS JOIN</c>).
/// </summary>
internal class EmptyPredicate : Predicates
{
    /// <summary>
    /// Initializes a new empty predicate with no child predicates.
    /// </summary>
    internal EmptyPredicate() : base([])
    {
    }

    /// <summary>
    /// Returns no fragments because this predicate intentionally emits no SQL.
    /// </summary>
    /// <param name="dialect">The SQL dialect parameter is unused because no SQL is emitted.</param>
    /// <returns>An empty SQL fragment sequence.</returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) => [];
}
