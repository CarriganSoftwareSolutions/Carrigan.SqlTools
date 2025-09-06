using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Extensions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Reflection;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    public SqlQuery UpdateById(T entity, SetColumns<T>? columns = null)
    {
        IEnumerable<PropertyInfo> updateTheseProperties = (columns?.ColumnNames?.Any() ?? false)
            ? _PropertiesLessKeys.Where(property => columns.ColumnNames.Contains(property.Name))
            : _PropertiesLessKeys;
        IEnumerable<PropertyInfo> keyProperties = _Key;

        string setColumnValues = string.Join(", ", updateTheseProperties.Select(property => $"[{property.Name}] = @{property.Name}"));
        string whereclause = string.Join(" and ", keyProperties.Select(property => $"[{property.Name}] = @{property.Name}"));

        IEnumerable<KeyValuePair<string, object>> parameters = updateTheseProperties.Concat(keyProperties).Select(property => GetSqlParameterKeyValue(property, true, entity));


        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"UPDATE {TableTag} SET {setColumnValues} WHERE {whereclause};",
            CommandType = CommandType.Text
        };
    }


    public SqlQuery UpdateByIds(T valuesEntity, SetColumns<T> columns, params IEnumerable<T> idEntities)
    {
        Or or = new            (
                idEntities.Select(entity => new And
                (
                    _Key.Select(property => new Equal
                        (
                            new Columns<T>(property.Name), 
                            new Parameters(property.Name, property.GetValue(entity)))
                        )
                ))
        );

        return Update(valuesEntity, columns, null, or);
    }

    public SqlQuery Update(T entity, SetColumns<T> columns, IJoins? joins, PredicatesBase predicates)
    {
        IEnumerable<PropertyInfo> updateTheseProperties = columns.ColumnNames.Any()
            ? _PropertiesLessKeys.Where(property => columns.ColumnNames.Contains(property.Name))
            : _PropertiesLessKeys;
        IEnumerable<PropertyInfo> keyProperties = _Key;
        IEnumerable<TableTag> selectTableTags = (joins?.TableTags ?? []).Append(TableTag).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. (predicates?.Column?.Select(col => col.TableTag)?.Distinct() ?? [])];
        IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);

        string setColumnValues = string.Join(", ", updateTheseProperties.Select(property => $"{TableTag}.[{property.Name}] = @ParameterSet_{property.Name}"));

        IEnumerable<KeyValuePair<string, object>> parameters = updateTheseProperties.Select(property => GetSqlParameterKeyValue(property, true, entity, null, "@ParameterSet_"));
        Dictionary<string, object> parametersDictionary = [.. parameters];


        StringBuilder queryBuilder = new($"UPDATE {TableTag} SET {setColumnValues} FROM {TableTag}");

        if (invalidTags.Any())
        {
            throw new ArgumentException($"{nameof(predicates)} contains the following invalid table identifiers: {invalidTags.Select(it => it.ToString()).JoinAnd()}", nameof(predicates));
        }

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {string.Join(' ', joins.ToSql())}");
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
            parametersDictionary.Add(predicates.GetParameters());
        }
        return new SqlQuery()
        {
            QueryText = queryBuilder.ToString(),
            Parameters = parametersDictionary,
            CommandType = CommandType.Text
        };
    }
}
