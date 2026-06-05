using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.QueryBuilders;

// Ignore Spelling: subquery

/// <summary>
/// Represents the shared options used to build a subquery for the specified model type.
/// </summary>
/// <typeparam name="T">The model type used as the primary query source.</typeparam>
public abstract record SubqueryBuilderBase<T> where T : class
{
    /// <summary>
    /// Gets or sets whether the subquery's SELECT query should use DISTINCT.
    /// </summary>
    public bool? Distinct { get; set; }

    /// <summary>
    /// Gets or sets the selected columns or expressions for the subquery.
    /// </summary>
    public SelectTagsBase? Selects { get; set; }

    /// <summary>
    /// Gets or sets the joins to include in the subquery.
    /// </summary>
    public Joins<T>? Joins { get; set; }

    /// <summary>
    /// Gets or sets the WHERE predicates for the subquery.
    /// </summary>
    public Predicates? Where { get; set; }

    /// <summary>
    /// Gets or sets the ORDER BY clause for the subquery.
    /// </summary>
    public OrderBysBase? OrderBys { get; set; }

    /// <summary>
    /// Gets or sets the paging options for the subquery.
    /// </summary>
    public PagingBase? Paging { get; set; }

    /// <summary>
    /// Returns a copy of the current subquery with DISTINCT enabled.
    /// </summary>
    /// <returns>A new subquery instance with <see cref="Distinct"/> set to <see langword="true"/>.</returns>
    public SubqueryBuilderBase<T> AsDistinct() =>
        this with { Distinct = true };

    /// <summary>
    /// Returns a copy of the current subquery with DISTINCT disabled.
    /// </summary>
    /// <returns>A new subquery instance with <see cref="Distinct"/> set to <see langword="false"/>.</returns>
    public SubqueryBuilderBase<T> NotDistinct() =>
        this with { Distinct = false };

    /// <summary>
    /// Returns a copy of the current subquery with the specified DISTINCT setting.
    /// </summary>
    /// <param name="distinct">Whether the subquery should use DISTINCT, or <see langword="null"/> to use the default behavior.</param>
    /// <returns>A new subquery instance with the specified DISTINCT setting.</returns>
    public SubqueryBuilderBase<T> WithDistinct(bool? distinct) =>
        this with { Distinct = distinct };

    /// <summary>
    /// Returns a copy of the current subquery with the specified select tags.
    /// </summary>
    /// <param name="selectTags">The select tags to include in the subquery.</param>
    /// <returns>A new subquery instance with the specified select tags.</returns>
    public SubqueryBuilderBase<T> WithSelectTags(SelectTagsBase? selectTags) =>
        this with { Selects = selectTags };

    /// <summary>
    /// Returns a copy of the current subquery with the specified joins.
    /// </summary>
    /// <param name="joins">The joins to include in the subquery.</param>
    /// <returns>A new subquery instance with the specified joins.</returns>
    public SubqueryBuilderBase<T> WithJoins(Joins<T>? joins) =>
        this with { Joins = joins };

    /// <summary>
    /// Returns a copy of the current subquery with the specified WHERE predicates.
    /// </summary>
    /// <param name="where">The WHERE predicates to apply to the subquery.</param>
    /// <returns>A new subquery instance with the specified WHERE predicates.</returns>
    public SubqueryBuilderBase<T> WithWhere(Predicates? where) =>
        this with { Where = where };

    /// <summary>
    /// Returns a copy of the current subquery with the specified ORDER BY clause.
    /// </summary>
    /// <param name="orderBy">The ORDER BY clause to apply to the subquery.</param>
    /// <returns>A new subquery instance with the specified ORDER BY clause.</returns>
    public SubqueryBuilderBase<T> WithOrderBy(OrderBysBase? orderBy) =>
        this with { OrderBys = orderBy };

    /// <summary>
    /// Returns a copy of the current subquery with the specified paging options.
    /// </summary>
    /// <param name="paging">The paging options to apply to the subquery.</param>
    /// <returns>A new subquery instance with the specified paging options.</returns>
    public SubqueryBuilderBase<T> WithPaging(PagingBase? paging) =>
        this with { Paging = paging };
}
