using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Extensions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Builds an SqlQuery object, which contains a parameterized Sql SELECT with a Dictionary representing the parameter value pairs.
    /// </summary>
    /// <param name="joins">Defines the joins. Leave as null to leave out joins.</param>
    /// <param name="predicates">Defines the WHERE clause. Leave as null to leave out the WHERE clause.</param>
    /// <param name="OrderBy">Defines the ORDER BY clause. Leave as null to leave out the ORDER BY clause.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <param name="orderBy"></param>
    public SqlQuery SelectCount(IJoins? joins, PredicatesBase? predicates)
    {
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(TableTag).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. (predicates?.Column?.Select(col => col.TableTag)?.Distinct() ?? [])];
        IEnumerable<TableTag> invalidPredicateTags = predicateTableTags.Except(selectableTableTags);
        StringBuilder queryBuilder = new($"SELECT COUNT(*) FROM {TableTag}");

        if (invalidPredicateTags.Any())
        {
            throw new SqlIdentifierException(invalidPredicateTags);
        }

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {string.Join(' ', joins.ToSql())}");
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
        }
        return new SqlQuery()
        {
            QueryText = queryBuilder.ToString(),
            Parameters = predicates?.GetParameters() ?? [],
            CommandType = CommandType.Text
        };
    }
}