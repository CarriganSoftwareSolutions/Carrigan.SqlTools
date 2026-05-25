using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.QueryBuilders;


//Ignore Spelling: subquery

/// <summary>
/// Represents the options used to build a SELECT query for the specified model type.
/// </summary>
/// <typeparam name="T">The model type used as the primary query source.</typeparam>
public abstract record SelectBuilderBase<T> where T : class
{
    /// <summary>
    /// Gets or sets whether the SELECT query should use DISTINCT.
    /// </summary>
    public bool? Distinct { get; set; }

    /// <summary>
    /// Gets or sets the selected columns or expressions for the query.
    /// </summary>
    public SelectTags? Selects { get; set; }

    /// <summary>
    /// Gets or sets the subquery to use as the query source instead of the default table.
    /// </summary>
    public Subquery<T>? Subquery { get; set; }

    /// <summary>
    /// Gets or sets the joins to include in the query.
    /// </summary>
    public Joins<T>? Joins { get; set; }

    /// <summary>
    /// Gets or sets the WHERE predicates for the query.
    /// </summary>
    public Predicates? Where { get; set; }

    /// <summary>
    /// Gets or sets the ORDER BY clause for the query.
    /// </summary>
    public OrderBys? OrderBys { get; set; }

    /// <summary>
    /// Gets or sets the paging options for the query.
    /// </summary>
    public PagingBase? Paging { get; set; }

    /// <summary>
    /// Returns a copy of the current query with DISTINCT enabled.
    /// </summary>
    /// <returns>A new query instance with <see cref="Distinct"/> set to <see langword="true"/>.</returns>
    public SelectBuilderBase<T> AsDistinct() =>
        this with { Distinct = true };

    /// <summary>
    /// Returns a copy of the current query with DISTINCT disabled.
    /// </summary>
    /// <returns>A new query instance with <see cref="Distinct"/> set to <see langword="false"/>.</returns>
    public SelectBuilderBase<T> NotDistinct() =>
        this with { Distinct = false };

    /// <summary>
    /// Returns a copy of the current query with the specified DISTINCT setting.
    /// </summary>
    /// <param name="distinct">Whether the query should use DISTINCT, or <see langword="null"/> to use the default behavior.</param>
    /// <returns>A new query instance with the specified DISTINCT setting.</returns>
    public SelectBuilderBase<T> WithDistinct(bool? distinct) =>
        this with { Distinct = distinct };

    /// <summary>
    /// Returns a copy of the current query with the specified select tags.
    /// </summary>
    /// <param name="selectTags">The select tags to include in the query.</param>
    /// <returns>A new query instance with the specified select tags.</returns>
    public SelectBuilderBase<T> WithSelectTags(SelectTags? selectTags) =>
        this with { Selects = selectTags };

    /// <summary>
    /// Returns a copy of the current query with the specified subquery source.
    /// </summary>
    /// <param name="subQuery">The subquery to use as the query source.</param>
    /// <returns>A new query instance with the specified subquery source.</returns>
    public SelectBuilderBase<T> WithSubquery(Subquery<T>? subQuery) =>
        this with { Subquery = subQuery };

    /// <summary>
    /// Returns a copy of the current query with the specified joins.
    /// </summary>
    /// <param name="joins">The joins to include in the query.</param>
    /// <returns>A new query instance with the specified joins.</returns>
    public SelectBuilderBase<T> WithJoins(Joins<T>? joins) =>
        this with { Joins = joins };

    /// <summary>
    /// Returns a copy of the current query with the specified WHERE predicates.
    /// </summary>
    /// <param name="where">The WHERE predicates to apply to the query.</param>
    /// <returns>A new query instance with the specified WHERE predicates.</returns>
    public SelectBuilderBase<T> WithWhere(Predicates? where) =>
        this with { Where = where };

    /// <summary>
    /// Returns a copy of the current query with the specified ORDER BY clause.
    /// </summary>
    /// <param name="orderBy">The ORDER BY clause to apply to the query.</param>
    /// <returns>A new query instance with the specified ORDER BY clause.</returns>
    public SelectBuilderBase<T> WithOrderBy(OrderBys? orderBy) =>
        this with { OrderBys = orderBy };

    /// <summary>
    /// Returns a copy of the current query with the specified paging options.
    /// </summary>
    /// <param name="paging">The paging options to apply to the query.</param>
    /// <returns>A new query instance with the specified paging options.</returns>
    public SelectBuilderBase<T> WithPaging(PagingBase? paging) =>
        this with { Paging = paging };
}