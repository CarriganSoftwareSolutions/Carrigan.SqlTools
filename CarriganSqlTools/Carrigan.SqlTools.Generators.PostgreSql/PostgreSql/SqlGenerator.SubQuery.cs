using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Contains SQL generation members for the specified model type.
/// </summary>
public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// Gets the SQL fragment that starts a subquery expression.
    /// </summary>
    public Subquery<T> Subquery
    (
        bool? distinct,
        SelectTagsBase? selects,
        Joins<T>? joins,
        Predicates? predicates,
        GroupBysBase? groupBys,
        OrderBysBase? orderBy, 
        PagingBase? paging
) =>
        BaseSubquery(distinct, selects, joins, predicates, groupBys, orderBy, paging);


    /// <summary>
    /// Builds a subquery from the supplied subquery builder.
    /// </summary>
    /// <param name="subqueryBuilder">The subquery builder to materialize.</param>
    /// <returns>A subquery that can be used as a SQL fragment.</returns>
    public Subquery<T> Subquery(SubqueryBuilder<T> subqueryBuilder) =>
        Subquery(subqueryBuilder.Distinct, subqueryBuilder.Selects, subqueryBuilder.Joins, subqueryBuilder.Where, null, subqueryBuilder.OrderBys, subqueryBuilder.Paging);
}