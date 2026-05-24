using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

public abstract partial class SqlGeneratorBase<T>
{
    protected virtual SubQuery<T> BaseSubQuery
    (
        bool? distinct,
        SelectTags? selects,
        Joins<T>? joins,
        Predicates? predicates,
        OrderBys? orderBy,
        PagingBase? paging
    ) =>
        new(BaseSelectFragments(distinct, selects, joins, predicates, orderBy, paging), Dialect);

}