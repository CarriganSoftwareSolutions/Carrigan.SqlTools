using Carrigan.Core.Extensions;
using SqlTools.Exceptions;
using SqlTools.Extensions;
using SqlTools.Joins;
using SqlTools.Predicates;
using SqlTools.Tags;
using System.Data;
using System.Text;

namespace SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    public SqlQuery Delete(T entity)
    {
        IEnumerable<KeyValuePair<string, object>> parameters = _Key.Select(property => GetSqlParameterKeyValue(property, true, entity));
        string whereclause = string.Join(" and ", _Key.Select(property => $"[{property.Name}] = @{property.Name}"));
        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"DELETE FROM {TableTag} WHERE {whereclause};",
            CommandType = CommandType.Text
        };
    }
    public SqlQuery DeleteAll()
    {
        return new SqlQuery()
        {
            Parameters = [],
            QueryText = $"DELETE FROM {TableTag};",
            CommandType = CommandType.Text
        };
    }

    public SqlQuery DeleteById(params IEnumerable<T> entities) =>
        Delete(null, new Or(entities.Select(entity => SqlGenerator<T>.GetByKeyPredicates(entity))));

    public SqlQuery Delete(IJoins? joins, PredicatesBase? predicates)
    {
        if (predicates == null && joins.IsNullOrEmpty())
        {
            return DeleteAll();
        }
        else
        {
            IEnumerable<TableTag> selectTableTags = (joins?.TableTags ?? []).Append(TableTag).Distinct();
            IEnumerable<TableTag> predicateTableTags = [.. (predicates?.Column?.Select(col => col.TableTag)?.Distinct() ?? [])];
            IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);
            StringBuilder queryBuilder = new($"DELETE FROM {TableTag}");

            if (invalidTags.Any())
            {
                throw new SqlIdentifierException(invalidTags);
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
}
