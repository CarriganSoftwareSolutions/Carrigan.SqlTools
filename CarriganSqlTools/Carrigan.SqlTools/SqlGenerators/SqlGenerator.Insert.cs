using Carrigan.Core.Extensions;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System.Text;
//IGNORE SPELLING: newid, unindexed

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Modifies a SQL <c>INSERT</c> statement so that it returns the generated key value
    /// (for example, a <c>UNIQUEIDENTIFIER</c> primary key) after insertion.
    /// </summary>
    /// <param name="queryText">
    /// The original SQL <c>INSERT</c> statement to modify.
    /// </param>
    /// <returns>
    /// A SQL string that inserts the row and returns the generated key value
    /// using an output table and a <c>SELECT</c> statement.
    /// </returns>
    /// <remarks>
    /// - Assumes the inserted key column is named <c>Id</c> and that <c>OUTPUT INSERTED.Id</c> is valid.
    /// - Leaves the input untouched except for replacing the first <c>VALUES</c> token with
    ///   <c>OUTPUT INSERTED.Id INTO @OutputTable VALUES</c>.
    /// </remarks>
    //TODO: The output id should be more generically determined.
    internal static string ModifyInsertQueryToReturnScalar(string queryText) =>
        // Build the final query using a temporary table to store the GUID
        new StringBuilder().AppendLine("DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);")
            .AppendLine(queryText.Replace("VALUES", "OUTPUT INSERTED.Id INTO @OutputTable VALUES"))
            .AppendLine("SELECT InsertedId FROM @OutputTable;")
            .ToString();

    /// <summary>
    /// Builds the <c>VALUES</c> clause for a single SQL <c>INSERT</c> row,
    /// generating parameter placeholders for the specified columns.
    /// </summary>
    /// <param name="columns">
    /// The collection of <see cref="ColumnInfo"/> objects that identify the columns to insert.
    /// </param>
    /// <param name="i">
    /// Optional zero-based index used to append a unique index to each parameter name.  
    /// If <c>null</c>, no index is appended.
    /// </param>
    /// <returns>
    /// A SQL string representing the <c>VALUES</c> clause for one row,
    /// for example <c>(@Column1, @Column2)</c>.
    private static string EnumeratedInsertValues(IEnumerable<ColumnInfo> columns, int? i = null) =>
        i == null
            ? $"({string.Join(", ", columns.Select(column => $"@{column.ParameterTag}"))})"
            : $"({string.Join(", ", columns.Select(column => $"@{column.ParameterTag.AddIndex(i.Value.ToString())}"))})";

    /// <summary>
    /// Builds the combined <c>VALUES</c> clause for a multi-row SQL <c>INSERT</c> statement,
    /// generating a parameterized value list for each entity.
    /// </summary>
    /// <param name="columns">
    /// The collection of <see cref="ColumnInfo"/> objects that identify the columns to insert.
    /// </param>
    /// <param name="entities">
    /// The collection of entity instances providing values for each row to insert.
    /// </param>
    /// <returns>
    /// A SQL string representing the <c>VALUES</c> clause for all entities,
    /// for example <c>(@Column1_0, @Column2_0), (@Column1_1, @Column2_1)</c>.
    /// </returns>
    private static string EnumeratedInsertValues(IEnumerable<ColumnInfo> columns, params IEnumerable<T> entities) =>
        $"{string.Join(", ", entities.Select((entity, index) => SqlGenerator<T>.EnumeratedInsertValues(columns, index)))}";

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for the specified entity,
    /// relying on database default values for key (identity, <c>NEWID()</c>) properties.
    /// </summary>
    /// <param name="entity">
    /// A data model instance representing the new record to insert.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// - If the model has no non-key columns, <c>DEFAULT VALUES</c> is used.
    /// - The statement is wrapped via <see cref="ModifyInsertQueryToReturnScalar(string)"/> to return the inserted key.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown if a column lacks a <see cref="ParameterTag"/> during parameter generation.
    /// This can surface indirectly from <see cref="GetSqlParameterKeyValue(ColumnInfo, T, int?, string?)"/>.
    /// </exception>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new()
    /// {
    ///     Name = "Hank",
    ///     Email = "Hank@example.com",
    ///     Phone = "+1(555)555-5555"
    /// };
    /// SqlQuery query = customerGenerator.InsertAutoId(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// INSERT INTO [Customer] ([Name], [Email], [Phone]) 
    /// VALUES (@Name, @Email, @Phone);
    /// ]]></code>
    /// </example>
    public SqlQuery InsertAutoId(T entity)
    {
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters;

        if (ColumnInfoLessKeys.None())
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
            parameters = ColumnInfoLessKeys.Select(key => GetSqlParameterKeyValue(key, entity));


            string columns = string.Join(", ", ColumnInfoLessKeys.Select(column => $"[{column.ColumnName}]"));
            string values = SqlGenerator<T>.EnumeratedInsertValues(ColumnInfoLessKeys);

            return new SqlQuery()
            {
                Parameters = [.. parameters],
                QueryText = SqlGenerator<T>.ModifyInsertQueryToReturnScalar($"INSERT INTO {Table} ({columns}) VALUES {values};"),
                CommandType = System.Data.CommandType.Text
            };
        }
    }

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for one or more entities.
    /// </summary>
    /// <param name="entities">
    /// A collection of data model instances representing the new records to insert.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated multi-row <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// - When only one entity is provided, unindexed parameter names are generated.  
    /// - When multiple entities are provided, parameter names are suffixed with the row index  
    ///   (e.g., <c>@Name_0</c>).  
    /// - When generating SQL, only properties that are publicly readable and belong to accessible  
    ///   types are considered. Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="entities"/> is empty.
    /// </exception>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// IEnumerable<Customer> customers =
    /// [
    ///     new()
    ///     {
    ///         Id = 42,
    ///         Name = "Hank",
    ///         Email = "Hank@example.com",
    ///         Phone = "+1(555)555-5555"
    ///     },
    ///     new()
    ///     {
    ///         Id = 732,
    ///         Name = "Homer",
    ///         Email = "Homer@example.com",
    ///         Phone = "+1(555)555-1234"
    ///     },
    /// ];
    ///
    /// SqlQuery query = customerGenerator.Insert(customers);
    /// ]]></code>
    ///
    /// <para>Resulting SQL:</para>
    ///
    /// <code><![CDATA[
    /// INSERT INTO [Customer] ([Id], [Name], [Email], [Phone])
    /// VALUES (@Id_0, @Name_0, @Email_0, @Phone_0),
    ///        (@Id_1, @Name_1, @Email_1, @Phone_1);
    /// ]]></code>
    /// </example>

    public SqlQuery Insert(params IEnumerable<T> entities)
    {
        if (entities.IsNullOrEmpty())
            throw new ArgumentException("No records provided.", nameof(entities));
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters;
        string values;

        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            parameters = [.. GetSqlParameterKeyValuePairs(entities.Single())];
        else
            parameters = [.. GetSqlParameterKeyValuePairs(entities)];

        string columns = string.Join(", ", ColumnInfo.Select(column => $"[{column.ColumnName}]"));
        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            values = SqlGenerator<T>.EnumeratedInsertValues(ColumnInfo);
        else
            values = SqlGenerator<T>.EnumeratedInsertValues(ColumnInfo, entities);


        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"INSERT INTO {Table} ({columns}) VALUES {values};",
            CommandType = System.Data.CommandType.Text
        };
    }
}
