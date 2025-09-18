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
    /// Generates SQL to delete the record passed in.  
    /// It only looks at the key fields for generating the SQL.
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="entity">use the data model as an id holder, uses only the key fields</param>
    /// <returns>Returns an SqlQuery object</returns>
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
        IEnumerable<KeyValuePair<string, object>> parameters = KeyColumns.Select(column => GetSqlParameterKeyValue(column, true, entity));
        string whereClause = string.Join(" and ", KeyColumns.Select(column => $"[{column._columnName}] = @{column._columnName}"));
        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"DELETE FROM {Table} WHERE {whereClause};",
            CommandType = CommandType.Text
        };
    }

    /// <summary>
    /// Generates SQL to delete all records for the given data model
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <returns>an SqlQuery object</returns>
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
    /// Generates SQL to delete the record passed in by Id.  
    /// It only looks at the key fields for generating the SQL.
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="entity">use the data model as an id holder, uses only the key fields</param>
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
    /// Generates SQL delete. 
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="joins">any joins</param>
    /// <param name="predicates">any where predicates</param>
    /// <returns>Returns an SqlQuery object</returns>
    /// <exception cref="SqlIdentifierException"></exception>
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
    /// <para>Note: Columns&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code language="csharp"><![CDATA[
    /// Columns&lt;Customer&gt; id = new(nameof(Customer.Id));
    /// Columns&lt;Order&gt; customerId = new(nameof(Order.CustomerId));
    /// Equal equals = new(id, customerId);
    /// InnerJoin&lt;Order, Customer&gt; join = new(equals);
    /// 
    /// SqlQuery query = orderGenerator.Delete(join, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// DELETE FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId])
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Note: Columns&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <para>Note: ColumnValues&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code language="csharp"><![CDATA[
    /// Columns&lt;Customer&gt; id = new(nameof(Customer.Id));
    /// Columns&lt;Order&gt; customerId = new(nameof(Order.CustomerId));
    /// Equal equals = new(id, customerId);
    /// InnerJoin&lt;Order, Customer&gt; join = new(equals);
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
    public SqlQuery Delete(IJoins? joins, PredicatesBase? predicates)
    {
        if (predicates == null && joins.IsNullOrEmpty())
        {
            return DeleteAll();
        }
        else
        {
            IEnumerable<TableTag> selectTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
            IEnumerable<TableTag> predicateTableTags = [.. predicates?.Column?.Select(col => col.TableTag)?.Distinct() ?? []];
            IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);
            StringBuilder queryBuilder = new($"DELETE FROM {Table}");

            if (invalidTags.Any())
            {
                throw new SqlIdentifierException(invalidTags);
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
                Parameters = predicates?.GetParameters() ?? [],
                CommandType = CommandType.Text
            };
        }
    }
}
