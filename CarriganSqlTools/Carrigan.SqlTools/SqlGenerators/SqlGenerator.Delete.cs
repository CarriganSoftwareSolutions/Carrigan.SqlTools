using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement for the specified entity,
    /// using only its key fields to identify the record to remove.
    /// </summary>
    /// <param name="entity">
    /// An instance of the data model that supplies the key field values
    /// used to locate the record to delete.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// The data model type must be <c>public</c>, and any properties that should map to
    /// columns must be public instance properties with a public getter.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new() { Id = 42 };
    /// SqlQuery query = customerGenerator.Delete(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer] WHERE [Id] = @Id;
    /// ]]></code>
    /// </example>
    public SqlQuery Delete(T entity)
    {
        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = KeyColumnInfo.Select(column => GetSqlParameterKeyValue(column, entity));
        string whereClause = string.Join(" and ", KeyColumnInfo.Select(column => $"[{column.ColumnName}] = @{column.ParameterTag}"));
        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"DELETE FROM {Table} WHERE {whereClause};",
            CommandType = CommandType.Text
        };
    }

    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement that removes all rows
    /// from the table represented by the data model type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// The data model type must be <c>public</c>, and any properties that should map to
    /// columns must be public instance properties with a public getter.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = customerGenerator.DeleteAll();
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer];
    /// ]]></code>
    /// </example>
    public SqlQuery DeleteAll() => new ()
    {
        Parameters = [],
        QueryText = $"DELETE FROM {Table};",
        CommandType = CommandType.Text
    };

    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement that removes the rows matching the key
    /// fields of the specified entities.
    /// </summary>
    /// <param name="entities">
    /// A sequence of data model instances used only as ID holders;  
    /// their key field values determine which rows to delete.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// The data model type must be <c>public</c>, and any properties intended for use as
    /// columns must be public instance properties with a public getter.
    /// </remarks>
    /// <param name="entity">an IEnumerable use the data model as an id holder, uses only the key fields</param>
    /// <returns>Returns an SqlQuery object</returns>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new() { Id = 42 };
    /// SqlQuery query = customerGenerator.DeleteById(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer] WHERE ([Customer].[Id] = @Parameter_Id)
    /// ]]></code>
    /// </example>
    public SqlQuery DeleteById(params IEnumerable<T> entities) =>
        Delete(null, new Or(entities.Select(entity => SqlGenerator<T>.GetByKeyPredicates(entity))));

    /// <summary>
    /// Generates a SQL <c>DELETE</c> statement for the table represented by
    /// <typeparamref name="T"/>, with optional joins and filter predicates.
    /// </summary>
    /// <param name="joins">
    /// Optional <see cref="IJoins"/> that specify related tables to join when forming the delete statement.
    /// </param>
    /// <param name="predicates">
    /// Optional <see cref="PredicateBase"/> representing the <c>WHERE</c> conditions
    /// that determine which rows to delete.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>DELETE</c> statement.
    /// </returns>
    /// <remarks>
    /// The data model type must be <c>public</c>, and any properties intended to map to columns
    /// must be public instance properties with a public getter.
    /// </remarks>
    /// <exception cref="InvalidTableException">
    /// Thrown when a <see cref="TableTag"/> referenced during SQL generation
    /// belongs to a table that is not included in the <c>JOIN</c> clause or
    /// specified as the primary table.
    /// </exception>
    /// <example>
    /// <para>Example with null Joins and null predicates</para>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = customerGenerator.Delete(null, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer];
    /// ]]></code>
    /// </example>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// ColumnValues&lt;Customer&gt; coumnValue = new(nameof(Customer.Name), "Hank");
    /// SqlQuery query = customerGenerator.Delete(null, coumnValue);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Note: ColumnEqualsColumn&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn&lt;Customer, Order&gt; predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin&lt;Order, Customer&gt; join = new(predicate);
    /// 
    /// SqlQuery query = orderGenerator.Delete(join, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId])
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Note: ColumnEqualsColumn&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <para>Note: ColumnValues&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn&lt;Customer, Order&gt; predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin&lt;Order, Customer&gt; join = new(predicate);
    /// 
    /// ColumnValues&lt;Customer&gt; customerEmail = new(nameof(Customer.Email), "spam@example.com");
    /// 
    /// SqlQuery query = orderGenerator.Delete(join, customerEmail);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Parameter_Email)
    /// ]]></code>
    /// </example>
    public SqlQuery Delete(IJoins? joins, PredicateBase? predicates)
    {
        if (predicates == null && joins.IsNullOrEmpty())
        {
            return DeleteAll();
        }
        else
        {
            IEnumerable<TableTag> selectTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
            IEnumerable<TableTag> predicateTableTags = [.. predicates?.Columns?.Select(col => col.TableTag)?.Distinct() ?? []];
            IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);
            StringBuilder queryBuilder = new($"DELETE FROM {Table}");

            if (invalidTags.Any())
            {
                throw new InvalidTableException(invalidTags);
            }
            if (joins?.IsNotNullOrEmpty() ?? false)
            {
                queryBuilder.Append($" {string.Join(' ', joins.ToSql())}");
            }
            if (predicates is not null)
            {
                queryBuilder.Append($" WHERE {predicates.ToSql()}");
            }
            return new SqlQuery()
            {
                QueryText = queryBuilder.ToString(),
                Parameters = [.. (joins?.Parameters ?? []).Concat(predicates?.GetParameters() ?? [])],
                CommandType = CommandType.Text
            };
        }
    }
}
