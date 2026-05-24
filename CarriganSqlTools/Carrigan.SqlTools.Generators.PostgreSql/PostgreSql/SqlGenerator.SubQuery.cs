using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    public SubQuery<T> SubQuery
    (
        bool? distinct,
        SelectTags? selects,
        Joins<T>? joins,
        Predicates? predicates,
        OrderBys? orderBy,
        PagingBase? paging
    ) =>
        BaseSubQuery(distinct, selects, joins, predicates, orderBy, paging);
}