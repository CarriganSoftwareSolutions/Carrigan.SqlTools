using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Contains SQL generation members for the specified model type.
/// </summary>
public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// Builds a subquery with the specified SELECT options.
    /// </summary>
    /// <param name="distinct">The SELECT DISTINCT behavior to apply.</param>
    /// <param name="selects">The optional SELECT projection list.</param>
    /// <param name="from">The source table or derived table, or null for a source-free SELECT.</param>
    /// <param name="joins">The optional joins to append after the FROM source.</param>
    /// <param name="predicates">The optional WHERE predicates.</param>
    /// <param name="groupBys">The optional GROUP BY clause.</param>
    /// <param name="having">The optional HAVING predicates.</param>
    /// <param name="orderBy">The optional ORDER BY clause.</param>
    /// <param name="paging">The optional paging clause.</param>
    /// <returns>A subquery that can be used as a SQL fragment.</returns>
    public Subquery Subquery
    (
        bool? distinct,
        SelectTags? selects,
        From? from,
        Joins? joins,
        Predicates? predicates,
        GroupBys? groupBys,
        Predicates? having,
        OrderBys? orderBy,
        PagingBase? paging
    ) =>
        BaseSubquery(distinct, selects, from, joins, predicates, groupBys, having, orderBy, paging);

    /// <summary>
    /// Builds a subquery from the supplied subquery builder.
    /// </summary>
    /// <param name="subqueryBuilder">The subquery builder to materialize.</param>
    /// <returns>A subquery that can be used as a SQL fragment.</returns>
    public Subquery Subquery(SubqueryBuilder<T> subqueryBuilder) =>
        Subquery
        (
            subqueryBuilder.Distinct,
            subqueryBuilder.Selects,
            subqueryBuilder.From,
            subqueryBuilder.Joins,
            subqueryBuilder.Where,
            subqueryBuilder.GroupBys,
            subqueryBuilder.Having,
            subqueryBuilder.OrderBys,
            subqueryBuilder.Paging
        );
}