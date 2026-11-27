using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.SqlGenerators;

static internal class SelectGenerator
{
    /// <summary>
    /// Builds the SQL fragments that make up a SELECT statement.
    /// </summary>
    /// <param name="dialect">The SQL dialects to use for fragment generation.</param>
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
    /// <returns>The SQL fragments that render the SELECT statement.</returns>
    /// <param name="paging">The optional paging clause.</param>
    static private IEnumerable<ISqlFragment> GetSelectFragments
    (
        ISqlDialects dialect,
        bool? distinct,
        SelectTags? selects,
        From? from,
        Joins? joins,
        Predicates? where,
        GroupBys? groupBys,
        Predicates? having,
        OrderBys? orderBy, PagingBase? paging
    )
    {

        yield return ISqlFragment.Select;
        yield return ISqlFragment.Space;
        if (distinct ?? false)
        {
            yield return ISqlFragment.Distinct;
            yield return ISqlFragment.Space;
        }

        if (selects is not null && selects.Any())
            foreach (ISqlFragment fragment in selects.Flatten(dialect))
                yield return fragment;
        else
            yield return ISqlFragment.Star;

        if (from is not null)
        {
            yield return ISqlFragment.Space;
            yield return ISqlFragment.From;
            yield return ISqlFragment.Space;
            foreach (ISqlFragment fragment in from.ToSqlFragments(dialect))
                yield return fragment;
        }

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            yield return ISqlFragment.Space;
            foreach (ISqlFragment fragment in joins.Flatten(dialect))
                yield return fragment;
        }

        if (where is not null)
        {
            yield return ISqlFragment.Space;
            yield return ISqlFragment.Where;
            yield return ISqlFragment.Space;

            foreach (ISqlFragment fragment in where.ToSqlFragments(dialect))
                yield return fragment;
        }

        if (groupBys.IsNotNullOrEmpty())
        {
            yield return ISqlFragment.Space;
            foreach (ISqlFragment fragment in groupBys.ToSqlFragments())
                yield return fragment;
        }

        if (having is not null)
        {
            yield return ISqlFragment.Space;
            yield return ISqlFragment.Having;
            yield return ISqlFragment.Space;

            foreach (ISqlFragment fragment in having.ToSqlFragments(dialect))
                yield return fragment;
        }

        if (orderBy.IsNotNullOrEmpty())
        {
            yield return ISqlFragment.Space;
            foreach (ISqlFragment fragment in orderBy.ToSqlFragments())
                yield return fragment;
        }


        if (paging is not null)
        {
            yield return ISqlFragment.Space;
            yield return dialect.RenderPaging(paging);
        }
    }

    /// <summary>
    /// Builds the SQL fragments that make up a SELECT statement.
    /// </summary>
    /// <param name="dialect">The SQL dialects to use for fragment generation.</param>
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
    /// <returns>The SQL fragments that render the SELECT statement.</returns>
    /// <param name="paging">The optional paging clause.</param>
    static internal IEnumerable<ISqlFragment> GenerateAsFragments
    (
        ISqlDialects dialect,
        bool? distinct,
        SelectTags? selects,
        From? from,
        Joins? joins,
        Predicates? where,
        GroupBys? groupBys,
        Predicates? having,
        OrderBys? orderBy, PagingBase? paging
    )
    {

        //Checks that predicates in the where clause do not contain aggregate expressions.
        ValidateWherePredicates(where);

        if ((selects is null || selects.IsEmpty()) && groupBys.IsNotNullOrEmpty())
            throw new GroupByRequiresSelectException();

        if (groupBys?.HasAggregates() ?? false)
            throw new AggregateExpressionInGroupByClauseException();

        bool UsesGroupingSemantics() =>
            groupBys.IsNotNullOrEmpty() || having is not null || (selects?.HasAggregates() ?? false);

        bool IsValidForGrouping(SqlExpression expression) =>
            groupBys?.ContainsAllNonAggregateParts(expression)
            ?? expression.AllNonAggregateColumns.Any() is false;

        if (UsesGroupingSemantics() && selects is not null && selects.Any())
        {
            bool allSelectedExpressionsAreValid =
                selects._selectTags.All(select => IsValidForGrouping(select.SqlExpression));

            if (allSelectedExpressionsAreValid is false)
                throw new MixedAggregateSelectException();
        }

        if (having is not null && IsValidForGrouping(having) is false)
            throw new UngroupedColumnInHavingClauseException();

        TableTag? rootTableTag = from?.RootTable;

        IEnumerable<TableTag> selectableTableTags;

        if (rootTableTag is null)
        {
            selectableTableTags = [];
            if (joins.IsNotNullOrEmpty())
                throw new JoinsWithoutFromException();
        }
        else
            selectableTableTags = (joins?.Validate(rootTableTag)?.TableTags() ?? []).Prepend(rootTableTag).Distinct();

        IEnumerable<TableTag> selectedTableTags = [.. selects?.GetTableTags() ?? []];
        IEnumerable<TableTag> invalidSelectedTags = selectedTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> predicateTableTags = [.. where?.AllParticipatingTables?.Distinct() ?? []];
        IEnumerable<TableTag> invalidPredicateTableTags = predicateTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> groupByTableTags = [.. groupBys?.TableTags?.Distinct() ?? []];
        IEnumerable<TableTag> invalidGroupByTableTags = groupByTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> havingTableTags = [.. having?.AllParticipatingTables?.Distinct() ?? []];
        IEnumerable<TableTag> invalidHavingTableTags = havingTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> orderByTableTags = [.. orderBy?.TableTags?.Distinct() ?? []];
        IEnumerable<TableTag> invalidOrderByTags = orderByTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> invalidTags = invalidSelectedTags
            .Concat(invalidPredicateTableTags)
            .Concat(invalidGroupByTableTags)
            .Concat(invalidHavingTableTags)
            .Concat(invalidOrderByTags)
            .Distinct();

        AmbiguousResultColumnException? ambiguousResultColumns = AmbiguousResultColumnException.CheckNames(selects);
        if (ambiguousResultColumns is not null)
            throw ambiguousResultColumns;

        if (invalidTags.Any())
            throw new InvalidTableException(invalidTags);
        return GetSelectFragments(dialect, distinct, selects, from, joins, where, groupBys, having, orderBy, paging);
    }

    static internal SqlQuery Generate
    (
        ISqlDialects dialect,
        bool? distinct,
        SelectTags? selects,
        From? from,
        Joins? joins,
        Predicates? where,
        GroupBys? groupBys,
        Predicates? having,
        OrderBys? orderBy, PagingBase? paging
    ) =>
         new(dialect, CommandType.Text, GenerateAsFragments(dialect, distinct, selects, from, joins, where, groupBys, having, orderBy, paging));

    /// <summary>
    /// Validates that predicates intended for a <c>WHERE</c> clause do not contain aggregate expressions.
    /// </summary>
    /// <param name="predicates">The optional <c>WHERE</c> predicate tree to validate.</param>
    /// <exception cref="AggregateExpressionInWhereClauseException">
    /// Thrown when <paramref name="predicates"/> contains an aggregate expression.
    /// </exception>
    private static void ValidateWherePredicates(Predicates? predicates)
    {
        if (predicates?.HasAggregates() ?? false)
            throw new AggregateExpressionInWhereClauseException();
    }
}
