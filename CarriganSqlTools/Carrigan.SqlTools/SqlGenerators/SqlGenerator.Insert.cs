using Carrigan.Core.Extensions;
using System.Reflection;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// This is a helper method that modifies insert queries to return a key field from the insert
    /// </summary>
    /// <param name="queryText">Insert Sql Query</param>
    /// <returns>modified Insert Sql Query</returns>
    private static string ModifyInsertQueryToReturnScalar(string queryText)
    {
        // Build the final query using a temporary table to store the GUID
        StringBuilder sqlQuery = new();
        sqlQuery.AppendLine("DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);");
        sqlQuery.AppendLine(queryText.Replace("VALUES", "OUTPUT INSERTED.Id INTO @OutputTable VALUES"));
        sqlQuery.AppendLine("SELECT InsertedId FROM @OutputTable;");
        return sqlQuery.ToString();
    }

    /// <summary>
    /// This is a helper method that generates the Values portion of the query
    /// </summary>
    private static string EnumeratedInsertValues(IEnumerable<PropertyInfo> properties, int? i = null) =>
        i == null  
            ? $"({string.Join(", ", properties.Select(property => $"@{property.Name}"))})" 
            : $"({string.Join(", ", properties.Select(property => $"@{property.Name}_{i}"))})";

    /// <summary>
    /// This is a helper method that generates the Values portion of the query
    /// </summary>
    private static string EnumeratedInsertValues(IEnumerable<PropertyInfo> properties, params IEnumerable<T> entities) =>
        $"{string.Join(", ", entities.Select((entity, index) => SqlGenerator<T>.EnumeratedInsertValues(properties, index)))}";

    /// <summary>
    /// This method generates an Insert SQL query, utilizing default values for key fields.
    /// Note: in order for this to work correctly, the key fields must have a default value.
    /// </summary>
    /// <param name="entity">a data model representing a new SQL record</param>
    /// <returns>an SQL query object</returns>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new()
    /// {
    ///     Name = "Hank",
    ///     Email = "Hank@example.com",
    ///     Phone = "+1(555)555-5555"
    /// };
    /// SqlQuery query = customerGenerator.InsertAutoId(entity);
    ///
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// INSERT INTO [Customer] ([Name], [Email], [Phone]) 
    /// VALUES (@Name, @Email, @Phone);
    /// ]]></code>
    /// </example>
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
    /// <summary>
    /// This method generates an Insert SQL query for one or more record, utilizing default values for key fields.
    /// Note: in order for this to work correctly, the key fields must have a default value.
    /// </summary>
    /// <param name="entity">a data model representing a new SQL record</param>
    /// <returns>an SQL query object</returns>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// IEnumerable<Customer> customers =
    ///     [
    ///         new()
    ///             {
    ///                 Id = 42,
    ///                 Name = "Hank",
    ///                 Email = "Hank@example.com",
    ///                 Phone = "+1(555)555-5555"
    ///             },
    ///             new()
    ///             {
    ///                 Id = 732,
    ///                 Name = "Homer",
    ///                 Email = "Homer@example.com",
    ///                 Phone = "+1(555)555-1234"
    ///             },
    ///     ];
    /// SqlQuery query = customerGenerator.Insert(customers);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// INSERT INTO 
    /// [Customer] ([Id], [Name], [Email], [Phone])
    /// VALUES 
    /// (@Id_0, @Name_0, @Email_0, @Phone_0), 
    /// (@Id_1, @Name_1, @Email_1, @Phone_1);
    /// ]]></code>
    /// </example>
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
