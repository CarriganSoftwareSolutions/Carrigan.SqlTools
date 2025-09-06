using Carrigan.Core.Extensions;
using System.Reflection;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    private static string ModifyInsertQueryToReturnScalar(string queryText)
    {
        // Build the final query using a temporary table to store the GUID
        StringBuilder sqlQuery = new();
        sqlQuery.AppendLine("DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);");
        sqlQuery.AppendLine(queryText.Replace("VALUES", "OUTPUT INSERTED.Id INTO @OutputTable VALUES"));
        sqlQuery.AppendLine("SELECT InsertedId FROM @OutputTable;");
        return sqlQuery.ToString();
    }

    private static string EnumeratedInsertValues(IEnumerable<PropertyInfo> properties, int? i = null) =>
        i == null  
            ? $"({string.Join(", ", properties.Select(property => $"@{property.Name}"))})" 
            : $"({string.Join(", ", properties.Select(property => $"@{property.Name}_{i}"))})";

    private static string EnumeratedInsertValues(IEnumerable<PropertyInfo> properties, params IEnumerable<T> entities) =>
        $"{string.Join(", ", entities.Select((entity, index) => SqlGenerator<T>.EnumeratedInsertValues(properties, index)))}";

    public SqlQuery InsertAutoId(T entity)
    {
        IEnumerable<KeyValuePair<string, object>> parameters;

        if (_PropertiesLessKeys.None())
        {
            return new SqlQuery()
            {
                Parameters = [],
                QueryText = SqlGenerator<T>.ModifyInsertQueryToReturnScalar($"INSERT INTO {TableTag} DEFAULT VALUES;"),
                CommandType = System.Data.CommandType.Text
            };
        }
        else
        {
            parameters = _PropertiesLessKeys.Select(key => GetSqlParameterKeyValue(key, true, entity));


            string columns = string.Join(", ", _PropertiesLessKeys.Select(property => $"[{property.Name}]"));
            string values = SqlGenerator<T>.EnumeratedInsertValues(_PropertiesLessKeys);

            return new SqlQuery()
            {
                Parameters = [.. parameters],
                QueryText = SqlGenerator<T>.ModifyInsertQueryToReturnScalar($"INSERT INTO {TableTag} ({columns}) VALUES {values};"),
                CommandType = System.Data.CommandType.Text
            };
        }
    }

    public SqlQuery Insert(params IEnumerable<T> entities)
    {
        IEnumerable<PropertyInfo> properties = properties = _Properties;
        IEnumerable<KeyValuePair<string, object>> parameters;
        string values;

        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            parameters = [.. GetSqlParameterKeyValuePairs(true, entities.Single())];
        else
            parameters = [.. GetSqlParameterKeyValuePairs(true, entities)];

        string columns = string.Join(", ", properties.Select(property => $"[{property.Name}]"));
        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            values = SqlGenerator<T>.EnumeratedInsertValues(_Properties);
        else
            values = SqlGenerator<T>.EnumeratedInsertValues(_Properties, entities);


        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"INSERT INTO {TableTag} ({columns}) VALUES {values};",
            CommandType = System.Data.CommandType.Text
        };
    }
}
