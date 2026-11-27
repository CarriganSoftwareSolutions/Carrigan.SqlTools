using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Net.NetworkInformation;
using System.Reflection.Emit;

namespace Carrigan.SqlTools.QueryBuilders;



/// <summary>
/// Represents the options used to build a SELECT query for the specified model type.
/// </summary>
public record SelectBuilder
{
    /// <summary>
    /// Gets or sets whether the SELECT query should use DISTINCT.
    /// </summary>
    public bool? Distinct { get; init; }

    /// <summary>
    /// Gets or sets the selected columns or expressions for the query.
    /// </summary>
    public SelectTags? Selects { get; init; }

    /// <summary>
    /// Gets or sets the FROM clause for the query.
    /// </summary>
    public From? From { get; init; }

    /// <summary>
    /// Gets or sets the joins to include in the query.
    /// </summary>
    public Joins? Joins { get; init; }

    /// <summary>
    /// Gets or sets the WHERE predicates for the query.
    /// </summary>
    public Predicates? Where { get; init; }

    /// <summary>
    /// Gets or sets the GROUP BY clause for the query.
    /// </summary>
    public GroupBys? GroupBys { get; init; }

    /// <summary>
    /// Gets or sets the <c>HAVING</c> predicates for the query.
    /// </summary>
    public Predicates? Having { get; init; }

    /// <summary>
    /// Gets or sets the ORDER BY clause for the query.
    /// </summary>
    public OrderBys? OrderBys { get; init; }

    /// <summary>
    /// Gets or sets the paging options for the query.
    /// </summary>
    public PagingBase? Paging { get; init; }

    /// <summary>
    /// Returns a copy of the current query with DISTINCT enabled.
    /// </summary>
    /// <returns>A new query instance with <see cref="Distinct"/> set to <see langword="true"/>.</returns>
    public SelectBuilder AsDistinct() =>
        this with { Distinct = true };

    /// <summary>
    /// Returns a copy of the current query with DISTINCT disabled.
    /// </summary>
    /// <returns>A new query instance with <see cref="Distinct"/> set to <see langword="false"/>.</returns>
    public SelectBuilder NotDistinct() =>
        this with { Distinct = false };

    /// <summary>
    /// Returns a copy of the current query with the specified DISTINCT setting.
    /// </summary>
    /// <param name="distinct">Whether the query should use DISTINCT, or <see langword="null"/> to use the default behavior.</param>
    /// <returns>A new query instance with the specified DISTINCT setting.</returns>
    public SelectBuilder WithDistinct(bool? distinct) =>
        this with { Distinct = distinct };

    /// <summary>
    /// Returns a copy of the current query with the specified select tags.
    /// </summary>
    /// <param name="selectTags">The select tags to include in the query.</param>
    /// <returns>A new query instance with the specified select tags.</returns>
    public SelectBuilder WithSelectTags(SelectTags? selectTags) =>
        this with { Selects = selectTags };

    /// <summary>
    /// Returns a copy of the current query with the specified FROM clause.
    /// </summary>
    /// <param name="from">The FROM clause to apply to the query.</param>
    /// <returns>A new query instance with the specified FROM clause.</returns>
    public SelectBuilder WithFrom(From? from) =>
        this with { From = from };

    /// <summary>
    /// Returns a copy of the current query with the specified joins.
    /// </summary>
    /// <param name="joins">The joins to include in the query.</param>
    /// <returns>A new query instance with the specified joins.</returns>
    public SelectBuilder WithJoins(Joins? joins) =>
        this with { Joins = joins };

    /// <summary>
    /// Returns a copy of the current query with the specified WHERE predicates.
    /// </summary>
    /// <param name="where">The WHERE predicates to apply to the query.</param>
    /// <returns>A new query instance with the specified WHERE predicates.</returns>
    public SelectBuilder WithWhere(Predicates? where) =>
        this with { Where = where };

    /// <summary>
    /// Returns a copy of the current query with the specified GROUP BY clause.
    /// </summary>
    /// <param name="groupsBy">
    /// The GROUP BY clause to apply to the query. This can be <see langword="null"/> to omit the GROUP BY clause, 
    /// or an instance of <see cref="GroupByClause.GroupBys"/> containing one or more group by items.
    /// </param>
    /// <returns>
    /// A new query instance with the specified GROUP BY clause. If <paramref name="groupsBy"/> is <see langword="null"/>,
    /// the returned query will not include a GROUP BY clause. Otherwise, the returned query will include the specified GROUP BY clause.
    /// </returns>
    public SelectBuilder WithGroupBy(GroupBys? groupsBy) =>
        this with { GroupBys = groupsBy };

    /// <summary>
    /// Returns a copy of the current query with the specified HAVING predicates.
    /// </summary>
    /// <param name="having">The HAVING predicates to apply to the query.</param>
    /// <returns>A new query instance with the specified HAVING predicates.</returns>
    public SelectBuilder WithHaving(Predicates? having) =>
        this with { Having = having };

    /// <summary>
    /// Returns a copy of the current query with the specified ORDER BY clause.
    /// </summary>
    /// <param name="orderBy">The ORDER BY clause to apply to the query.</param>
    /// <returns>A new query instance with the specified ORDER BY clause.</returns>
    public SelectBuilder WithOrderBy(OrderBys? orderBy) =>
        this with { OrderBys = orderBy };

    /// <summary>
    /// Returns a copy of the current query with the specified paging options.
    /// </summary>
    /// <param name="paging">The paging options to apply to the query.</param>
    /// <returns>A new query instance with the specified paging options.</returns>
    public SelectBuilder WithPaging(PagingBase? paging) =>
        this with { Paging = paging };


    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    //TODO: hould dialect be passed in here or should it be passed in to the constructor instead?
    public SqlQuery AsSqlQuery(ISqlDialects dialect) =>
        SelectGenerator.Generate(dialect, Distinct, Selects, From, Joins, Where, GroupBys, Having, OrderBys, Paging);
}
