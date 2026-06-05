using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
//IGNORE SPELLING: unindexed

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Contains SQL generation members for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
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
    ///     [
    ///         new()
    ///         {
    ///             Id = 42,
    ///             Name = "Hank",
    ///             Email = "Hank@example.com",
    ///             Phone = "+1(555)555-5555"
    ///         },
    ///         new()
    ///         {
    ///             Id = 732,
    ///             Name = "Homer",
    ///             Email = "Homer@example.com",
    ///             Phone = "+1(555)555-1234"
    ///         },
    ///     ];
    /// SqlQuery query = customerGenerator.Insert(null, null, customers);
    /// ]]></code>
    ///
    /// <para>Resulting SQL:</para>
    ///
    /// <code><![CDATA[
    /// INSERT INTO [Customer] ([Id], [Name], [Email], [Phone]) 
    /// VALUES (@Id_1, @Name_2, @Email_3, @Phone_4), (@Id_5, @Name_6, @Email_7, @Phone_8);
    /// ]]></code>
    /// </example>
    public SqlQuery Insert(ColumnCollectionBase<T>? insertColumnCollection, ColumnCollectionBase<T>? returnColumns, params IEnumerable<T> entities) =>
        base.BaseInsert(insertColumnCollection, returnColumns, entities);

    /// <summary>
    /// Builds an INSERT SQL query for the supplied model data.
    /// </summary>
    /// <param name="insertQuery">The insert builder to materialize.</param>
    /// <returns>A <see cref="SqlQuery"/> representing the INSERT statement.</returns>
    public SqlQuery Insert(InsertBuilder<T> insertQuery) =>
        Insert(insertQuery.InsertColumns, insertQuery.ReturnColumns, insertQuery.Records);
}
