using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tags;
using System.Net;
using System.Text;
//IGNORE SPELLING: newid, unindexed

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    //TODO: Unit test returned column names both as inserted into the output table and the return name

    /// <summary>
    /// Generates SQL to declare an OutputTable. This is used as part of the query to the inserted values for columns determined by <paramref name="columnInfo"/>
    /// </summary>
    /// <param name="columnInfo">Designates columns for which the inserted values should be returned.</param>
    /// <returns>SQL to declare an OutputTable. This is used as part of the query to the inserted values for columns determined by <paramref name="columnInfo"/></returns>
    internal static string ReturnTableDefinition(IEnumerable<ColumnInfo> columnInfo) =>
        $"DECLARE @OutputTable TABLE ({string.Join(", ", columnInfo.Select(column => $"{column.ColumnName} {column.SqlType.TypeDeclaration}"))});";

    /// <summary>
    /// Generates SQL output the columns into the output table. This is used as part of the query to the inserted values for columns determined by <paramref name="columnInfo"/>
    /// </summary>
    /// <param name="columnInfo">Designates columns for which the inserted values should be returned.</param>
    /// <returns>
    /// Generates SQL output the columns into the output table. This is used as part of the query to the inserted values for columns determined by <paramref name="columnInfo"/>
    /// </returns>
    internal static string ReturnOutputColumns(IEnumerable<ColumnInfo> columnInfo) =>
       $"OUTPUT {string.Join(", ", columnInfo.Select(column => $"INSERTED.{column.ColumnName}"))} INTO @OutputTable";


    /// <summary>
    /// Gets the name of the column to use for returning the values inserted for the designated columns in <paramref name="columnInfo"/>
    /// </summary>
    /// <remarks>
    /// Because the invoker will be expecting the names as would be done with a select, we need to do the select from the output table to match the name the invoker is expecting.
    /// </remarks>
    /// <param name="columnInfo">Designates columns for which the inserted values should be returned.</param>
    /// <returns>
    /// Returns the name of the column to use for returning the values inserted for the designated columns in <paramref name="columnInfo"/>
    /// </returns>
    internal static string ReturnSelectName(ColumnInfo columnInfo)
    {
        string resultColumnName = InvocationReflectorCache<T>.GetResultColumnName(columnInfo.PropertyInfo);
        if (resultColumnName != columnInfo.ColumnName)
            return $"{columnInfo.ColumnName} AS {resultColumnName}";
        else
            return columnInfo.ColumnName;
    }

    /// <summary>
    /// Generates SQL to select from the output column. This is used as part of the query to the inserted values for columns determined by <paramref name="columnInfo"/>
    /// </summary>
    /// <param name="columnInfo">Designates columns for which the inserted values should be returned.</param>
    /// <returns>
    /// Generates SQL to select from the output column. This is used as part of the query to the inserted values for columns determined by <paramref name="columnInfo"/>
    /// </returns>
    internal static string ReturnSelectOutput(IEnumerable<ColumnInfo> columnInfo) =>
        $"SELECT {string.Join(", ", columnInfo.Select(column => ReturnSelectName(column)))} FROM @OutputTable;";

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
    private static string EnumeratedInsertValues(IEnumerable<ColumnInfo> columns, IEnumerable<T> entities) =>
        $"{string.Join(", ", entities.Select((entity, index) => SqlGenerator<T>.EnumeratedInsertValues(columns, index)))}";

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for the specified entity,
    /// relying on database default values for key (identity, <c>NEWID()</c>) properties.
    /// </summary>
    /// <param name="entities">
    /// A data model instance(s) representing the new record to insert.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// - If the model has no non-key columns, <c>DEFAULT VALUES</c> is used.
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
    /// <example>
    /// <code language="csharp"><![CDATA[
    ///CompositePrimaryKeyTable testEntity = new()
    ///{
    ///    NotKey1 = 1,
    ///    NotKey2 = 2,
    ///    NotKey3 = 3
    ///};
    ///CompositePrimaryKeyTable testEntity2 = new()
    ///{
    ///    NotKey1 = 1,
    ///    NotKey2 = 2,
    ///    NotKey3 = 3
    ///};
    ///
    /// SqlQuery query = _sqlGeneratorForCompositePrimaryKeyTable.InsertAutoId(testEntity, testEntity2);     
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DECLARE @OutputTable TABLE (Id1 INT, Id2 INT);
    /// INSERT INTO [Ck] ([NotKey1], [NotKey2], [NotKey3])
    /// OUTPUT INSERTED.Id1, INSERTED.Id2 INTO @OutputTable
    /// VALUES (@NotKey1_0, @NotKey2_0, @NotKey3_0), (@NotKey1_1, @NotKey2_1, @NotKey3_1);
    /// SELECT Id1, Id2 FROM @OutputTable;
    /// ]]></code>
    /// </example>
    public SqlQuery InsertAutoId(params IEnumerable<T> entities) =>
        Insert
        (
            new ColumnCollection<T>(ColumnInfoLessKeys.Select(column => column.PropertyName)),
            new ColumnCollection<T>(KeyColumnInfo.Select(column => column.PropertyName)),
            entities
        );

    //TODO: Update documentation for new parameter ColumnCollection<T>? insertColumnCollection, code review
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
    /// SqlQuery query = customerGenerator.Insert(null, null, customers);
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
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// EntityWithTableAttribute testEntity = new()
    /// {
    ///     Name = "Test Name",
    ///     DateOf = new DateTime(2023, 10, 1),
    ///     When = "Now",
    ///     Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" }
    /// };
    /// EntityWithTableAttribute testEntity2 = new()
    /// {
    ///     Name = "Test Name2",
    ///     DateOf = new DateTime(2025, 12, 6),
    ///     When = "Now",
    ///     Address = new Address { Street = "123 Fake St", City = "Test City", PostalCode = "37067" }
    /// };
    /// 
    /// ColumnCollection<EntityWithTableAttribute> insertColumns = new("Id", "Name", "When");
    /// 
    /// 
    /// SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(insertColumns, null, testEntity, testEntity2);
    /// ]]></code>
    ///
    /// <para>Resulting SQL:</para>
    ///
    /// <code><![CDATA[
    /// INSERT INTO [Test] ([Id], [Name], [When]) 
    /// VALUES (@Id_0, @Name_0, @When_0), 
    ///        (@Id_1, @Name_1, @When_1)
    /// ]]></code>
    /// </example>

    /// <example>
    /// <code language="csharp"><![CDATA[
    /// EntityWithTableAttribute testEntity = new()
    /// {
    ///     Name = "Test Name",
    ///     DateOf = new DateTime(2023, 10, 1),
    ///     When = "Now",
    ///     Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "37067" } 
    /// };
    /// EntityWithTableAttribute testEntity2 = new()
    /// {
    ///     Name = "Test Name2",
    ///     DateOf = new DateTime(2025, 12, 6), 
    ///     When = "Now",
    ///     Address = new Address { Street = "123 Fake St", City = "Test City", PostalCode = "37067" }
    /// };
    /// 
    /// ColumnCollection<EntityWithTableAttribute> insertColumns = new("Name", "When");
    /// ColumnCollection<EntityWithTableAttribute> returnColumns = new("Id", "DateOf");
    /// 
    /// SqlQuery query = _sqlGeneratorForEntityWithTableAttribute.Insert(insertColumns, returnColumns, testEntity, testEntity2);
    /// ]]></code>
    ///
    /// <para>Resulting SQL:</para>
    ///
    /// <code><![CDATA[
    /// DECLARE @OutputTable TABLE (Id UNIQUEIDENTIFIER, DateOf DATETIME2);
    /// INSERT INTO [Test] ([Name], [When])
    /// OUTPUT INSERTED.Id, INSERTED.DateOf INTO @OutputTable
    /// VALUES (@Name_0, @When_0), (@Name_1, @When_1);
    /// SELECT Id, DateOf FROM @OutputTable;
    /// ]]></code>
    /// </example>
    public SqlQuery Insert(ColumnCollection<T>? insertColumnCollection, ColumnCollection<T>? returnColumns, params IEnumerable<T> entities)
    {
        IEnumerable<ColumnInfo> insertTheseColumns = insertColumnCollection?.ColumnInfo ?? ColumnInfo;
        string queryPart1;
        string queryPart2;
        StringBuilder queryBuilder = new();

        if (entities.IsNullOrEmpty())
            throw new ArgumentException("No records provided.", nameof(entities));
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters;
        string values;

        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            parameters = [.. GetSqlParameterKeyValuePairs(insertTheseColumns, entities.Single())];
        else
            parameters = [.. GetSqlParameterKeyValuePairs(insertTheseColumns, entities)];

        string columns = string.Join(", ", insertTheseColumns.Select(column => $"[{column.ColumnName}]"));
        if(entities.Count() == 1) //when there is only one record use the overload that doesn't add index counts to the parameters
            values = SqlGenerator<T>.EnumeratedInsertValues(insertTheseColumns);
        else
            values = SqlGenerator<T>.EnumeratedInsertValues(insertTheseColumns, entities);

        queryPart1 = $"INSERT INTO {Table} ({columns})";
        queryPart2 = $"VALUES {values};";

        if (returnColumns is null)
        {
            return new SqlQuery()
            {
                Parameters = [.. parameters],
                QueryText = $"{queryPart1} {queryPart2}",
                CommandType = System.Data.CommandType.Text
            };
        }
        else 
        {
            //TODO: Add unit test to make sure we get the expected results when doing multiples.
            queryBuilder.AppendLine(ReturnTableDefinition(returnColumns.ColumnInfo));
            queryBuilder.AppendLine(queryPart1);
            queryBuilder.AppendLine(ReturnOutputColumns(returnColumns.ColumnInfo));
            queryBuilder.AppendLine(queryPart2);
            queryBuilder.AppendLine(ReturnSelectOutput(returnColumns.ColumnInfo));
            return new SqlQuery()
            {
                Parameters = [.. parameters],
                QueryText = queryBuilder.ToString(),
                CommandType = System.Data.CommandType.Text
            };
        }

    }
}
