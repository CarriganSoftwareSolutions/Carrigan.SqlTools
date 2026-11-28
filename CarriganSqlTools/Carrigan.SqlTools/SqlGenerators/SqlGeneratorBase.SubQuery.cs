using Carrigan.Core.Extensions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Contains shared SQL generation members for the specified model type.
/// </summary>
public abstract partial class SqlGeneratorBase<T>
{
    /// <summary>
    /// Gets the SQL fragment used to begin a subquery expression in shared generation code.
    /// </summary>
    protected virtual Subquery BaseSubquery
    (
        bool? distinct,
        SelectTags? selects,
        From? from,
        Joins? joins,
        Predicates? where,
        GroupBys? groupBys,
        Predicates? having,
        OrderBys? orderBy,
        PagingBase? paging
    ) =>
        new
        (
            selects.IsNotNullOrEmpty() ? selects : from?.SelectTags ?? new SelectTags(),
            BaseSelectFragments(distinct, selects, from, joins, where, groupBys, having, orderBy, paging)
        );
}
