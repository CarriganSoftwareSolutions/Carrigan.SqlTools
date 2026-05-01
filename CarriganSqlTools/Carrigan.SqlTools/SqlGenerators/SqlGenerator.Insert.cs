using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tags;
using System.Net;
using System.Text;
//IGNORE SPELLING: newid, unindexed

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Builds the <c>VALUES</c> clause for a single SQL <c>INSERT</c> row,
    /// generating parameter placeholders for the specified columns.
    /// </summary>
    /// <param name="columns">
    /// The collection of <see cref="ColumnInfo"/> objects that identify the columns to insert.
    /// </param>
    /// <param name="i">
    /// Optional zero-based index used to append a unique index to each parameter name.
    /// If <c>null</c>, no index is appended and unindexed parameter names are used.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{SqlFragment}"/> representing the <c>VALUES</c> list for one row,
    /// for example <c>(@Column1, @Column2)</c> or <c>(@Column1_0, @Column2_0)</c>.
    /// </returns>
    private IEnumerable<SqlFragment> GetInsertValueFragments(IEnumerable<ColumnInfo> columns, T entity, int? i = null) =>
    [
        new SqlFragmentText("("),
        ..columns
            .Select(column => new SqlFragmentParameter(GetSqlParameter(column, entity, i)))
            .JoinFragments(new SqlFragmentText(", ")),
        new SqlFragmentText(")")
    ];

    /// <summary>
    /// Builds the <c>VALUES</c> clause for a single SQL <c>INSERT</c> row,
    /// generating parameter placeholders for the specified columns.
    /// </summary>
    /// <param name="columns">
    /// The collection of <see cref="ColumnInfo"/> objects that identify the columns to insert.
    /// </param>
    /// <param name="i">
    /// Optional zero-based index used to append a unique index to each parameter name.
    /// If <c>null</c>, no index is appended and unindexed parameter names are used.
    /// </param>
    /// <returns>
    /// A <see cref="SqlFragmentGroup"/> representing the <c>VALUES</c> list for one row,
    /// for example <c>(@Column1, @Column2)</c> or <c>(@Column1_0, @Column2_0)</c>.
    /// </returns>
    private SqlFragmentGroup GetEnumeratedInsertValueFragmentsGroup(IEnumerable<ColumnInfo> columns, T entity, int i) =>
        new(GetInsertValueFragments(columns, entity, i));

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
    /// An enumerable sequence of <see cref="SqlFragment"/> representing the combined <c>VALUES</c> clause for all entities.
    /// </returns>
    private IEnumerable<SqlFragment> GetEnumeratedInsertValueFragments(IEnumerable<ColumnInfo> columns, IEnumerable<T> entities) =>
        entities.Select((entity, index) => GetEnumeratedInsertValueFragmentsGroup(columns, entity, index))
            .JoinFragments(new SqlFragmentText(", "));

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for one or more entities,
    /// relying on database default values for key (identity, <c>NEWID()</c>) properties.
    /// </summary>
    /// <param name="entities">
    /// One or more data model instances representing the new records to insert.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// If the model has no non-key columns, <c>DEFAULT VALUES</c> is used.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown if a column lacks a <see cref="ParameterTag"/> during parameter generation.
    /// This can surface indirectly from
    /// <see cref="GetSqlParameter(ColumnInfo, T, int?, string?)"/>.
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

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for one or more entities.
    /// </summary>
    /// <param name="insertColumnCollection">
    /// An optional collection specifying which columns to insert. If <c>null</c>,
    /// all mapped columns are included.
    /// </param>
    /// <param name="returnColumns">
    /// An optional collection specifying which columns’ inserted values should be returned.
    /// If <c>null</c>, no values are returned.
    /// </param>
    /// <param name="entities">
    /// One or more data model instances representing the new records to insert.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated multi-row <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// When only one entity is provided, unindexed parameter names are generated.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// When multiple entities are provided, parameter names are suffixed with the row index
    /// (for example, <c>@Name_0</c>).
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Only properties that are publicly readable and belong to accessible types
    /// are considered. Members not visible outside their defining assembly are ignored.
    /// </description>
    /// </item>
    /// </list>
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
        IEnumerable<SqlFragment> insertIntoFragments;

        if (entities.IsNullOrEmpty())
            throw new ArgumentException("No records provided.", nameof(entities));

        string columns = string.Join(", ", insertTheseColumns.Select(column => Dialect.QuoteIdentifier(column.ColumnName)));

        insertIntoFragments = [new SqlFragmentText($"INSERT INTO {Table} ({columns})")];

        IEnumerable<SqlFragment> valuesFragments = entities.Count() switch //when there is only one record use the overload that doesn't add index counts to the parameters
        {
            1 => new SqlFragmentText($"VALUES ")
                    .Concat(GetInsertValueFragments(insertTheseColumns, entities.Single()))
                    .Append(new SqlFragmentText(";")),
            _ => new SqlFragmentText($"VALUES ")
                    .Concat(GetEnumeratedInsertValueFragments(insertTheseColumns, entities))
                    .Append(new SqlFragmentText(";")),
        };

        IEnumerable<SqlFragment> queryFragments = returnColumns switch
        {
            null => insertIntoFragments.Append(new SqlFragmentText(" ")).Concat(valuesFragments),
            _ => Dialect.GetInsertReturningFragments<T>(insertIntoFragments, valuesFragments, returnColumns.ColumnInfo),
        };

        return queryFragments.ToSqlQuery();
    }
}
