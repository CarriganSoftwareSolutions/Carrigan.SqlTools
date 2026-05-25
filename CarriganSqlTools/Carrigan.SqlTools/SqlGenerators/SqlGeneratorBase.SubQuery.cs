using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

public abstract partial class SqlGeneratorBase<T>
{
    protected virtual Subquery<T> BaseSubquery
    (
        bool? distinct,
        SelectTags? selects,
        Joins<T>? joins,
        Predicates? predicates,
        OrderBys? orderBy,
        PagingBase? paging
    ) =>
        new(BaseSelectFragments(distinct, null, selects, joins, predicates, orderBy, paging), Dialect);

}