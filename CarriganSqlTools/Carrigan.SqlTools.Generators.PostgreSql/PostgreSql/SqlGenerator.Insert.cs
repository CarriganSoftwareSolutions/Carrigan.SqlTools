using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
//IGNORE SPELLING: unindexed

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Contains SQL generation members for the specified model type.
/// </summary>
public sealed partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for one or more entities,
    /// relying on database default values for key properties.
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
    /// <see cref="SqlGeneratorBase{T}.GetColumnValue"/>.
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
    /// INSERT INTO "Customer" ("Name", "Email", "Phone") 
    /// VALUES ($1, $2, $3)
    /// RETURNING "Id";
    /// ]]></code>
    /// </example>
    public SqlQuery InsertAutoId(params IEnumerable<T> entities) =>
        base.BaseInsertAutoId(entities);

    /// <summary>
    /// Generates a SQL <c>INSERT</c> statement for one or more entities.
    /// </summary>
    /// <param name="insertColumnCollection">The model properties representing the SQL columns to insert.</param>
    /// <param name="returnColumns">The model properties representing the SQL columns to return.</param>
    /// <param name="entities">The model instances representing SQL rows or parameter sets.</param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated multi-row <c>INSERT</c> statement.
    /// </returns>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// When one entity is provided, PostgreSQL positional parameters start at <c>$1</c>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// When multiple entities are provided, PostgreSQL positional parameters continue sequentially across rows
    /// (for example, <c>$5</c> after a four-column first row).
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
    /// 
    /// SqlQuery query = customerGenerator.Insert(null, null, customers);
    /// ]]></code>
    ///
    /// <para>Resulting SQL:</para>
    ///
    /// <code><![CDATA[
    /// INSERT INTO "Customer" ("Id", "Name", "Email", "Phone") 
    /// VALUES ($1, $2, $3, $4), ($5, $6, $7, $8);
    /// ]]></code>
    /// </example>
    public SqlQuery Insert(ColumnCollectionBase<T>? insertColumnCollection, ColumnCollectionBase<T>? returnColumns, params IEnumerable<T> entities) =>
        base.BaseInsert(insertColumnCollection, returnColumns, entities);

    /// <summary>
    /// Generates a PostgreSQL <c>INSERT</c> statement from an insert builder.
    /// </summary>
    /// <param name="insertQuery">The builder containing insert columns, optional returned columns, and records to insert.</param>
    /// <returns>A <see cref="SqlQuery"/> representing the generated PostgreSQL <c>INSERT</c> statement.</returns>
    public SqlQuery Insert(InsertBuilder<T> insertQuery) =>
        Insert(insertQuery.InsertColumns, insertQuery.ReturnColumns, insertQuery.Records);
}
