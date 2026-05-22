using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Net;
using System.Text;
//IGNORE SPELLING: newid, unindexed

namespace Carrigan.SqlTools.Generators.SqlServer;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
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
    /// <see cref="GetSqlParameter(ColumnInfo, T)"/>.
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
    /// DECLARE @OutputTable TABLE(Id INT NOT NULL);
    /// INSERT INTO[Customer] ([Name], [Email], [Phone])
    /// OUTPUT INSERTED.Id INTO @OutputTable
    /// VALUES(@Name_1, @Email_2, @Phone_3);
    /// SELECT Id FROM @OutputTable;
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
        base.BaseInsertAutoId(entities);

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
    public SqlQuery Insert(ColumnCollection<T>? insertColumnCollection, ColumnCollection<T>? returnColumns, params IEnumerable<T> entities) =>
        base.BaseInsert(insertColumnCollection, returnColumns, entities);
}
