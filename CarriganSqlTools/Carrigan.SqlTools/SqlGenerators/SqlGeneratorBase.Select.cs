using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Contains shared SQL generation members for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
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
    protected virtual SqlQuery BaseSelectAll(OrderBysBase? orderBy = null) =>
        BaseSelect(null, null, null, null, null, null, orderBy, null);

    /// <summary>
    /// Builds an <see cref="SqlQuery"/> containing a parameterized SQL
    /// <c>SELECT</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>JOIN</c>, <c>WHERE</c>, <c>ORDER BY</c>, and
    /// dialect-specific paging clauses.
    /// </summary>
    /// <param name="selects">
    /// Optional projected columns and result aliases. If omitted or empty, all columns from the table represented by <typeparamref name="T"/> are selected.
    /// </param>
    /// <param name="joins">
    /// Optional joins to include in the query. Omit to select only from the base table.
    /// </param>
    /// <param name="predicates">
    /// Optional filter predicates to compose the <c>WHERE</c> clause.
    /// </param>
    /// <param name="groupBys">
    /// The optional GROUP BY clause. When provided, this will be rendered after the WHERE clause and before the ORDER BY clause.
    /// </param>
    /// <param name="orderBy">
    /// Optional ordering to compose the <c>ORDER BY</c> clause.
    /// When <paramref name="paging"/> is provided, key columns are appended to
    /// the ordering (if not already present) to ensure stable paging semantics.
    /// </param>
    /// <param name="distinct">The SELECT DISTINCT behavior to apply.</param>
    /// <param name="subQuery">The subquery used as the query source.</param>
    /// <param name="paging">The paging fragment to include in the query.</param>
    /// <returns>
    /// An <see cref="SqlQuery"/> whose <c>QueryText</c> is the generated SQL and whose
    /// <c>Parameters</c> contain values from <paramref name="predicates"/> and any joins.
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
    /// Thrown when any table referenced by <paramref name="selects"/>, <paramref name="predicates"/>, or
    /// <paramref name="orderBy"/> is not the base table nor included by <paramref name="joins"/>.
    /// </exception>
    protected virtual SqlQuery BaseSelect
    (
        bool? distinct,
        Subquery<T>? subQuery,
        SelectTagsBase? selects,
        Joins<T>? joins,
        Predicates? predicates,
        GroupBysBase? groupBys,
        OrderBysBase? orderBy, PagingBase? paging
    ) =>
        new(Dialect, CommandType.Text, BaseSelectFragments(distinct, subQuery, selects, joins, predicates, groupBys, orderBy, paging));

    /// <summary>
    /// Builds the SQL fragments that make up a SELECT statement.
    /// </summary>
    /// <param name="distinct">The SELECT DISTINCT behavior to apply.</param>
    /// <param name="subQuery">The optional subquery used as the FROM source.</param>
    /// <param name="selects">The optional SELECT projection list.</param>
    /// <param name="joins">The optional joins to append after the FROM source.</param>
    /// <param name="predicates">The optional WHERE predicates.</param>
    /// <param name="groupBys">
    /// The optional GROUP BY clause. When provided, this will be rendered after the WHERE clause and before the ORDER BY clause.
    /// </param>
    /// <param name="orderBy">The optional ORDER BY clause.</param>
    /// <returns>The SQL fragments that render the SELECT statement.</returns>
    /// <param name="paging">The optional paging clause.</param>
    private IEnumerable<ISqlFragment> BaseSelectFragments
    (
        bool? distinct,
        Subquery<T>? subQuery,
        SelectTagsBase? selects,
        Joins<T>? joins,
        Predicates? predicates,
        GroupBysBase? groupBys,
        OrderBysBase? orderBy, PagingBase? paging
    )
    {
        IEnumerable<ISqlFragment> GetFragments()
        {
            if (distinct ?? false)
                yield return new SqlFragmentText($"SELECT DISTINCT ");
            else
                yield return new SqlFragmentText($"SELECT ");

            if (selects is not null && selects.Any())
                yield return selects;
            else if (HasAliasedColumns(SupportedTypes))
                yield return GetAllSelectTags();
            else
            {
                yield return Table;
                yield return new SqlFragmentText(".*");
            }
            yield return new SqlFragmentText(" FROM ");
            if (subQuery is not null)
            {
                yield return subQuery;
                yield return new SqlFragmentText(" AS ");
            }

            yield return Table;

            if (joins?.IsNotNullOrEmpty() ?? false)
            {
                foreach (ISqlFragment fragment in joins.ToSqlFragments(Dialect))
                    yield return fragment;
            }

            if (predicates is not null)
            {
                yield return new SqlFragmentText($" WHERE ");

                foreach (ISqlFragment fragment in predicates.ToSqlFragments(Dialect))
                    yield return fragment;
            }

            if(groupBys.IsNotNullOrEmpty())
                yield return new SqlFragmentText($" {groupBys.AsGroupBy().ToSql(Dialect)}");

            if (orderBy.IsNotNullOrEmpty())
                yield return new SqlFragmentText($" {orderBy.AsOrderBy().ToSql(Dialect)}");


            if (paging is not null)
            {
                yield return ISqlFragment.Space;
                yield return Dialect.RenderPaging(paging);
            }

        }
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();

        IEnumerable<TableTag> selectedTableTags = [.. selects?.GetTableTags() ?? []];
        IEnumerable<TableTag> invalidSelectedTags = selectedTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> predicateTableTags = [.. predicates?.DescendantColumns?.Select(static col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidPredicateTableTags = predicateTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> groupByTableTags = [.. groupBys?.TableTags?.Distinct() ?? []];
        IEnumerable<TableTag> invalidgroupByTableTags = predicateTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> orderByTableTags = [.. orderBy?.TableTags?.Distinct() ?? []];
        IEnumerable<TableTag> invalidOrderByTags = orderByTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> invalidTags = invalidSelectedTags
            .Concat(invalidPredicateTableTags)
            .Concat(invalidgroupByTableTags)
            .Concat(invalidOrderByTags)
            .Distinct();

        AmbiguousResultColumnException? ambiguousResultColumns = AmbiguousResultColumnException.CheckNames(selects);
        if (ambiguousResultColumns is not null)
            throw ambiguousResultColumns;

        if (invalidTags.Any())
            throw new InvalidTableException(invalidTags);

        if (paging is not null)
        {
            // add the key to orderby when using an offset next, this is to overcome a limitation in SQL Server
            // that has unexpected behavior if the order by values are not unique
            orderBy ??= NewOrderBys();
            IEnumerable<OrderByBase> orderByKeyItems =
            [
                .. KeyColumnInfo
                    .Select(key => NewOrderByKey(key.PropertyName, SortDirectionEnum.Ascending))
                    .Where(item => orderBy.Contains(item) == false)
            ];
            orderBy = orderBy.Concat(orderByKeyItems);
        }

        return GetFragments();
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
            return BaseSelect(null, null, null, null, new Or(entities.Select(entity => new And(GetByKeyPredicates(entity)))), null, null, null);
    }
}
