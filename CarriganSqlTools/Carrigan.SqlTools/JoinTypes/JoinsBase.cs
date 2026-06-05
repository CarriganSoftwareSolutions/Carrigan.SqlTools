using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Provides the non-generic base contract for SQL join collections.
/// </summary>
public abstract class JoinsBase
{
    /// <summary>
    /// Gets the table tags that participate in the join sequence, including the root table used by the join collection.
    /// </summary>
    internal abstract IEnumerable<TableTag> TableTags { get; }

    /// <summary>
    /// Gets the first concrete join in the collection.
    /// </summary>
    /// <returns>The first join in the collection.</returns>
    internal abstract JoinBase First();

    /// <summary>
    /// Determines whether the collection contains no concrete joins.
    /// </summary>
    /// <returns><see langword="true"/> when the collection is empty; otherwise, <see langword="false"/>.</returns>
    internal abstract bool IsEmpty();

    /// <summary>
    /// Renders the join collection as SQL fragments for the supplied dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render join targets and predicates.</param>
    /// <returns>The SQL fragments that make up the rendered join clauses.</returns>
    internal abstract IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect);
}