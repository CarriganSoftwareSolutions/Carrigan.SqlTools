using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Reflection;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// This is a helper method that modifies insert queries to return a key field from the insert
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="queryText">Insert Sql Query</param>
    /// <returns>modified Insert Sql Query</returns>
    internal static string ModifyInsertQueryToReturnScalar(string queryText) =>
        // Build the final query using a temporary table to store the GUID
        new StringBuilder().AppendLine("DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);")
            .AppendLine(queryText.Replace("VALUES", "OUTPUT INSERTED.Id INTO @OutputTable VALUES"))
            .AppendLine("SELECT InsertedId FROM @OutputTable;")
            .ToString();

    private static string EnumeratedInsertValues(IEnumerable<ColumnTag> columns, int? i = null) =>
        i == null
            ? $"({string.Join(", ", columns.Select(column => $"@{column._columnName}"))})"
            : $"({string.Join(", ", columns.Select(column => $"@{column._columnName}_{i}"))})";

    /// <summary>
    /// This is a helper method that generates the Values portion of the query
    /// </summary>
    private static string EnumeratedInsertValues(IEnumerable<ColumnTag> columns, params IEnumerable<T> entities) =>
        $"{string.Join(", ", entities.Select((entity, index) => SqlGenerator<T>.EnumeratedInsertValues(columns, index)))}";

    /// <summary>
    /// This method generates an Insert SQL query, utilizing default values for key fields.
    /// Note: in order for this to work correctly, the key fields must have a default value.
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
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

        if (ColumnsLessKeys.None())
        {
            return new SqlQuery()
            {
                Parameters = [],
                QueryText = SqlGenerator<T>.ModifyInsertQueryToReturnScalar($"INSERT INTO {Table} DEFAULT VALUES;"),
                CommandType = System.Data.CommandType.Text
            };
        }
        else
        {
            parameters = PropertiesLessKeys.Select(key => GetSqlParameterKeyValue(key, true, entity));


            string columns = string.Join(", ", ColumnsLessKeys.Select(column => $"[{column._columnName}]"));
            string values = SqlGenerator<T>.EnumeratedInsertValues(ColumnsLessKeys);

            return new SqlQuery()
            {
                Parameters = [.. parameters],
                QueryText = SqlGenerator<T>.ModifyInsertQueryToReturnScalar($"INSERT INTO {Table} ({columns}) VALUES {values};"),
                CommandType = System.Data.CommandType.Text
            };
        }
    }
    /// <summary>
    /// This method generates an Insert SQL query for one or more record, utilizing default values for key fields.
    /// Note: in order for this to work correctly, the key fields must have a default value.
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
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
        IEnumerable<KeyValuePair<string, object>> parameters;
        string values;

        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            parameters = [.. GetSqlParameterKeyValuePairs(true, entities.Single())];
        else
            parameters = [.. GetSqlParameterKeyValuePairs(true, entities)];

        string columns = string.Join(", ", Columns.Select(column => $"[{column._columnName}]"));
        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            values = SqlGenerator<T>.EnumeratedInsertValues(Columns);
        else
            values = SqlGenerator<T>.EnumeratedInsertValues(Columns, entities);


        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"INSERT INTO {Table} ({columns}) VALUES {values};",
            CommandType = System.Data.CommandType.Text
        };
    }
}
