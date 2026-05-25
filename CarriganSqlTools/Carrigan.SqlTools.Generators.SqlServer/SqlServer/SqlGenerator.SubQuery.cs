using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlServer;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    public Subquery<T> Subquery
    (
        bool? distinct,
        SelectTags? selects,
        Joins<T>? joins,
        Predicates? predicates,
        OrderBys? orderBy,
        PagingBase? paging
    ) =>
        BaseSubquery(distinct, selects, joins, predicates, orderBy, paging);

    public Subquery<T> Subquery(SubqueryBuilder<T> subqueryBuilder) =>
        Subquery(subqueryBuilder.Distinct, subqueryBuilder.Selects, subqueryBuilder.Joins, subqueryBuilder.Where, subqueryBuilder.OrderBys, subqueryBuilder.Paging);
}