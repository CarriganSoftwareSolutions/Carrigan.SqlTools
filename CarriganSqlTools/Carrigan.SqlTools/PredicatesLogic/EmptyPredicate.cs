using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System.Collections.Generic;

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

    internal override IEnumerable<SqlFragment> ToSql(string prefix, string branchName, IEnumerable<ParameterTag> duplicates) => [];
}
