using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Represents the <see cref="SqlGenerator{T}"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// Gets the Subquery value.
    /// </summary>
    public Subquery<T> Subquery
    (
        bool? distinct,
        SelectTagsBase? selects,
        Joins<T>? joins,
        Predicates? predicates,
        OrderBysBase? orderBy,
        PagingBase? paging
    ) =>
        BaseSubquery(distinct, selects, joins, predicates, orderBy, paging);

    /// <summary>
    /// Builds a subquery from the supplied subquery builder.
    /// </summary>
    /// <param name="subqueryBuilder">The subquery builder to materialize.</param>
    /// <returns>The result of the Subquery operation.</returns>
    public Subquery<T> Subquery(SubqueryBuilder<T> subqueryBuilder) =>
        Subquery(subqueryBuilder.Distinct, subqueryBuilder.Selects, subqueryBuilder.Joins, subqueryBuilder.Where, subqueryBuilder.OrderBys, subqueryBuilder.Paging);
}