using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents an empty predicate that emits no SQL and has no parameters.
/// Used for join types that do not have an <c>ON</c> clause (for example, <c>CROSS JOIN</c>).
/// </summary>
internal class EmptyPredicate : Predicates
{
    internal EmptyPredicate() : base([])
    {
    }

    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) => [];
}
