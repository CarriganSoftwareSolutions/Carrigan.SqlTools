using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
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
    /// Builds an <see cref="SqlQuery"/> containing a parameterized
    /// <c>SELECT COUNT(...)</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>SELECT</c> projection, <c>JOIN</c>, and <c>WHERE</c> clauses.
    /// </summary>
    /// <param name="distinct"></param>
    /// <param name="select">
    /// Optional result projection to be counted. If omitted or empty, the generator
    /// counts <c>{Table}.*</c>.
    /// </param>
    /// <param name="joins">
    /// Optional joins to include in the count query. Omit to count only rows from the base table.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> whose <c>QueryText</c> is the generated count SQL and whose
    /// <c>Parameters</c> are derived from <paramref name="predicates"/> and any joins.
    /// </returns>
    /// <remarks>
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="AmbiguousResultColumnException">
    /// Thrown when <paramref name="select"/> defines duplicate or ambiguous result column names.
    /// </exception>
    /// <exception cref="InvalidTableException">
    /// Thrown when any table referenced by <paramref name="select"/> or <paramref name="predicates"/> (or by their columns)
    /// is not the base table nor included by <paramref name="joins"/>.
    /// </exception>
    /// <param name="predicates">
    /// Optional filter predicates to compose the <c>WHERE</c> clause for the count.
    /// </param>
    protected virtual SqlQuery BaseSelectCount(bool? distinct, SelectTagBase? select, Joins<T>? joins, Predicates? predicates)
    {
        IEnumerable<ISqlFragment> GetFragments()
        {
            if (distinct ?? false)
                yield return new SqlFragmentText($"SELECT COUNT(DISTINCT ");
            else
                yield return new SqlFragmentText($"SELECT COUNT(");

            if (select is not null)
                yield return select.WithNoAlias();
            else
                yield return GetColumnInfo(SupportedTypes).First().ColumnTag;

            yield return new SqlFragmentText(") FROM ");
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
        }
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();

        TableTag selectedTableTag = select?.ColumnTag?.TableTag ?? Table;
        IEnumerable<TableTag> invalidSelectedTags = new[] { selectedTableTag }.Except(selectableTableTags);

        IEnumerable<TableTag> predicateTableTags = [.. predicates?.DescendantColumns?.Select(static col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidPredicateTags = predicateTableTags.Except(selectableTableTags);

        IEnumerable<TableTag> invalidTags = [.. invalidSelectedTags.Concat(invalidPredicateTags).Distinct()];

        if (invalidTags.Any())
            throw new InvalidTableException(invalidTags);

        return new SqlQuery(Dialect, CommandType.Text, GetFragments());
    }
}
