using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Contains shared SQL generation members for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public abstract partial class SqlGeneratorBase<T>
{
    /// <summary>
    /// Gets the SQL fragment used to begin a subquery expression in shared generation code.
    /// </summary>
    protected virtual Subquery<T> BaseSubquery
    (
        bool? distinct,
        SelectTagsBase? selects,
        Joins<T>? joins,
        Predicates? predicates,
        GroupBysBase? groupBys,
        OrderBysBase? orderBy, 
        PagingBase? paging
) =>
        new(BaseSelectFragments(distinct, null, selects, joins, predicates, groupBys, orderBy, paging), Dialect);

}
