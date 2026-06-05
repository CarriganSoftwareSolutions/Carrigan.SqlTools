using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents the <see cref="SqlGeneratorBase{T}"/> component.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public abstract partial class SqlGeneratorBase<T>
{
    /// <summary>
    /// Gets the BaseSubquery value.
    /// </summary>
    protected virtual Subquery<T> BaseSubquery
    (
        bool? distinct,
        SelectTagsBase? selects,
        Joins<T>? joins,
        Predicates? predicates,
        OrderBysBase? orderBy,
        PagingBase? paging
    ) =>
        new(BaseSelectFragments(distinct, null, selects, joins, predicates, orderBy, paging), Dialect);

}
