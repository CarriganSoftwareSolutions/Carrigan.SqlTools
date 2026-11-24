using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Contains shared SQL generation members for the specified model type.
/// </summary>
public abstract partial class SqlGeneratorBase<T>
{
    /// <summary>
    /// Generates a SQL <c>SELECT *</c> statement that returns all rows
    /// from the table represented by <typeparamref name="T"/>.
    /// </summary>
    /// <param name="orderBy">
    /// Optional ordering to include in the <c>ORDER BY</c> clause.
    /// If omitted, the rows are returned without an explicit order.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>SELECT</c> statement.
    /// </returns>
    /// <remarks>
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="InvalidTableException">
    /// Thrown if any table referenced by <paramref name="orderBy"/> does not participate
    /// in the query (i.e., is not the base table and not included by joins).
    /// </exception>
    protected virtual SqlQuery BaseSelectAll(OrderBys? orderBy = null) =>
        BaseSelect(null, null, null, null, null, null, null, orderBy, null);

    /// <summary>
    /// Builds an <see cref="SqlQuery"/> containing a parameterized SQL
    /// <c>SELECT</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>JOIN</c>, <c>WHERE</c>, <c>GROUP BY</c>, <c>HAVING</c>,
    /// <c>ORDER BY</c>, and dialect-specific paging clauses.
    /// </summary>
    /// <param name="selects">
    /// Optional projected columns and result aliases. If omitted or empty, all columns from the table represented by <typeparamref name="T"/> are selected.
    /// </param>
    /// <param name="from">
    /// The source table for the query.
    /// </param>
    /// <param name="joins">
    /// Optional joins to include in the query. Omit to select only from the base table.
    /// </param>
    /// <param name="where">
    /// Optional filter predicates to compose the <c>WHERE</c> clause.
    /// </param>
    /// <param name="groupBys">
    /// The optional GROUP BY clause. When provided, this will be rendered after the WHERE clause and before the ORDER BY clause.
    /// </param>
    /// <param name="having">
    /// Optional filter predicates to compose the <c>HAVING</c> clause.
    /// </param>
    /// <param name="orderBy">
    /// Optional ordering to compose the <c>ORDER BY</c> clause.
    /// When <paramref name="paging"/> is provided, key columns are appended to
    /// the ordering (if not already present) to ensure stable paging semantics.
    /// </param>
    /// <param name="distinct">The SELECT DISTINCT behavior to apply.</param>
    /// <param name="paging">The paging fragment to include in the query.</param>
    /// <returns>
    /// An <see cref="SqlQuery"/> whose <c>QueryText</c> is the generated SQL and whose
    /// <c>Parameters</c> contain values participating anywhere in the rendered query, including projections, joins,
    /// <paramref name="where"/>, <paramref name="groupBys"/>, <paramref name="having"/>, and <paramref name="orderBy"/>.
    /// </returns>
    /// <remarks>
    /// When providing <paramref name="selects"/>, you will almost certainly need a different model
    /// matching the projected columns to materialize results correctly (since they may no longer map
    /// back to <typeparamref name="T"/>).
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="AmbiguousResultColumnException">
    /// Thrown when <paramref name="selects"/> defines duplicate or ambiguous result column names.
    /// </exception>
    /// <exception cref="InvalidTableException">
    /// Thrown when any table referenced by <paramref name="selects"/>, <paramref name="where"/>,
    /// <paramref name="groupBys"/>, <paramref name="having"/>, or <paramref name="orderBy"/>
    /// is not the base table nor included by <paramref name="joins"/>.
    /// </exception>
    /// <exception cref="AggregateExpressionInWhereClauseException">
    /// Thrown when <paramref name="where"/> contains an aggregate expression.
    /// </exception>
    protected virtual SqlQuery BaseSelect
    (
        bool? distinct,
        SelectTags? selects,
        From? from,
        Joins? joins,
        Predicates? where,
        GroupBys? groupBys,
        Predicates? having,
        OrderBys? orderBy,
        PagingBase? paging
) =>
        new(Dialect, CommandType.Text, BaseSelectFragments(distinct, selects, from, joins, where, groupBys, having, orderBy, paging));


    /// <summary>
    /// Builds the SQL fragments that make up a SELECT statement.
    /// </summary>
    /// <param name="distinct">The SELECT DISTINCT behavior to apply.</param>
    /// <param name="selects">The optional SELECT projection list.</param>
    /// <param name="from">The FROM clause.</param>
    /// <param name="joins">The optional joins to append after the FROM source.</param>
    /// <param name="where">The optional WHERE predicates.</param>
    /// <param name="groupBys">
    /// The optional GROUP BY clause. When provided, this will be rendered after the WHERE clause and before the ORDER BY clause.
    /// </param>
    /// <param name="having">The optional HAVING predicates.</param>
    /// <param name="orderBy">The optional ORDER BY clause.</param>
    /// <param name="paging">The optional paging clause.</param>
    /// <returns>The SQL fragments that render the SELECT statement.</returns>
    /// 
    protected virtual IEnumerable<ISqlFragment> BaseSelectFragments
    (
        bool? distinct,
        SelectTags? selects,
        From? from,
        Joins? joins,
        Predicates? where,
        GroupBys? groupBys,
        Predicates? having,
        OrderBys? orderBy,
        PagingBase? paging
    )
    {
        if (paging is not null)
        {
            // add the key to orderby when using an offset next, this is to overcome a limitation in SQL Server
            // that has unexpected behavior if the order by values are not unique
            orderBy ??= NewOrderBys();
            IEnumerable<OrderBy> orderByKeyItems =
            [
                .. KeyColumnInfo
                    .Select(key => NewOrderByKey(key.PropertyName, SortDirectionEnum.Ascending))
                    .Where(item => orderBy.Contains(item) == false)
            ];
            orderBy = orderBy.Concat(orderByKeyItems);
        }
        return SelectGenerator.Generate(Dialect, distinct, selects, from, joins, where, groupBys, having, orderBy, paging);
    }

    /// <summary>
    /// Generates a SQL <c>SELECT *</c> statement that returns rows matching the key
    /// properties of the specified entities.
    /// </summary>
    /// <param name="entities">
    /// One or more data model instances used only as ID holders; their key property values
    /// are combined into a predicate that selects matching rows.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>SELECT</c> statement.
    /// </returns>
    /// <remarks>
    /// The generated <c>WHERE</c> clause is composed as an <c>OR</c> of per-entity
    /// <c>AND</c> predicates over the key columns.
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entities"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="entities"/> is empty.
    /// </exception>
    /// <exception cref="NoPrimaryKeyPropertyException{T}">
    /// Thrown when <typeparamref name="T"/> has no key annotations (neither the SQL generator’s
    /// <c>PrimaryKey</c> nor <c>Key</c> attributes) and a “By Id” operation is invoked.
    /// </exception>
    protected virtual SqlQuery BaseSelectById(params IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        if (HasKeyProperty is false)
            throw new NoPrimaryKeyPropertyException<T>();
        else
            return BaseSelect(null, null, null, null, new Or(entities.Select(entity => new And(GetByKeyPredicates(entity)))), null, null, null, null);
    }
}
