using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Provides the non-generic base contract for SQL join collections.
/// </summary>
public abstract class JoinsBase
{
    internal abstract IEnumerable<TableTag> TableTags { get; }
    internal abstract JoinBase First();
    internal abstract bool IsEmpty();
    internal abstract IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect);
}