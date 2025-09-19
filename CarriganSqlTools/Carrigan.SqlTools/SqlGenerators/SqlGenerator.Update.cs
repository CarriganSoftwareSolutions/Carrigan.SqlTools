using Carrigan.Core.Extensions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Query;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// This method generates SQL to update a single record.
    /// </summary>
    /// <param name="entity">Record being updates</param>
    /// <param name="columns">Specify columns to update, leave null to update all except key fields.</param>
    /// <returns>SQL Query object</returns>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new()
    /// {
    ///     Id = 42,
    ///     Name = "Hank Hill",
    ///     Email = "Hank.Hill@example.com",
    ///     Phone = "+1(555)555-5555"
    /// };
    /// SqlQuery query = customerGenerator.UpdateById(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Customer]
    /// SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone
    /// WHERE [Id] = @Id;
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Note: SetColumns&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code language="csharp"><![CDATA[
    /// SetColumns&lt;Customer&gt; columns = new(nameof(Customer.Email));
    /// Customer entity = new() 
    /// { 
    ///     Id = 42, 
    ///     Name = "Hank", 
    ///     Email = "Hank@example.gov" 
    /// };
    /// SqlQuery query = customerGenerator.UpdateById(entity, columns);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Customer]
    /// SET [Email] = @Email
    /// WHERE [Id] = @Id;
    /// ]]></code>
    /// </example>
    public SqlQuery UpdateById(T entity, SetColumns<T>? columns = null)
    {
        IEnumerable<ColumnTag> updateTheseColumns = 
            (columns?.ColumnTags?.Any() ?? false) ? columns.ColumnTags : ColumnsLessKeys;

        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = updateTheseColumns.Concat(KeyColumns).Select(column => GetSqlParameterKeyValue(column, true, entity));

        List<Tuple<ColumnTag, ParameterTag>> setColumnAndParameterName = [];
        foreach (ColumnTag column in updateTheseColumns)
        {
            KeyValuePair<ParameterTag, object> parameter = GetSqlParameterKeyValue(column, true, entity);
            setColumnAndParameterName.Add(new(column, parameter.Key));
        }
        string sets = string.Join(", ", setColumnAndParameterName.Select(columnParameter => $"{columnParameter.Item1.ToString(false)} = @{columnParameter.Item2}"));

        List<Tuple<ColumnTag, string>> whereColumnAndParameterName = [];
        foreach (ColumnTag column in KeyColumns)
        {
            KeyValuePair<ParameterTag, object> parameter = GetSqlParameterKeyValue(column, true, entity);
            whereColumnAndParameterName.Add(new(column, parameter.Key));
        }
        string where = string.Join(", ", whereColumnAndParameterName.Select(columnParameter => $"{columnParameter.Item1.ToString(false)} = @{columnParameter.Item2}"));

        return new SqlQuery($"UPDATE {Table} SET {sets} WHERE {where};", [.. parameters]);
    }

    /// <summary>
    /// This method generates SQL to update one or more records.
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="entity">Record being updates</param>
    /// <param name="columns">Specify columns to update, leave null to update all except key fields.</param>
    /// <param name="idEntities">Id holders to indicate which records to update</param>
    /// <returns>SQL Query object</returns>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer updateValues = new()
    /// {
    ///     Name = "John Doe",
    ///     Email = string.Empty
    /// };
    /// 
    /// IEnumerable&lt;Customer&gt; customerIds =
    /// [
    ///     new () { Id = 42 },
    ///     new () { Id = 732 }
    /// ];
    /// 
    /// SetColumns&lt;Customer&gt; updateColumns = new(nameof(Customer.Name), nameof(Customer.Email));
    /// 
    /// SqlQuery query = customerGenerator.UpdateByIds(updateValues, updateColumns, customerIds);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Customer] 
    /// SET [Customer].[Name] = @ParameterSet_Name, [Customer].[Email] = @ParameterSet_Email 
    /// FROM [Customer] 
    /// WHERE (([Customer].[Id] = @Parameter_0_R_Id) OR ([Customer].[Id] = @Parameter_1_R_Id))
    /// ]]></code>
    /// </example>
    public SqlQuery UpdateByIds(T valuesEntity, SetColumns<T>? columns, params IEnumerable<T> idEntities)
    {
        Or or = new            (
                idEntities.Select(entity => new And
                (
                    KeyColumns.Select(column => new Equal
                        (
                            new Columns<T>(column._columnName), 
                            new Parameters(GetParameterTagFromColumn(column) ?? throw new NullReferenceException($"ParameterTag not found for column: {column}."), GetValue(column, entity)))
                        )
                ))
        );

        return Update(valuesEntity, columns, null, or);
    }

    /// <summary>
    /// This method generates SQL to update one or more records.
    /// Note: The data model should be public, and any properties you wish to access as columns should be public instance properties with a public getter.
    /// </summary>
    /// <param name="entity">Record being updates</param>
    /// <param name="columns">Specify columns to update, leave null to update all except key fields.</param>
    /// <param name="joins">Use joins to help determine which record to update</param>
    /// <param name="predicates">Use where clause predicates to help determine which records to update</param>
    /// <returns>SQL Query object</returns>
    /// <example>
    /// <para>
    /// Create Update SQL query with a Where clause.
    /// Note: SetColumns&lte;T&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: Columns&lte;T&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: ColumnValues&lte;T&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new() { Email = "spam@example.com" };
    /// SetColumns&lte;Customer&gt; setColumns = new(nameof(Customer.Email));
    /// ColumnValues&lte;Customer&gt; customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");
    /// 
    /// SqlQuery query = customerGenerator.Update(entity, setColumns, null, customerEmailEquals);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Customer] 
    /// SET [Customer].[Email] = @ParameterSet_Email 
    /// FROM [Customer] 
    /// WHERE ([Customer].[Email] = @Parameter_Email)
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>
    /// Create Update SQL query with Joins and a Where clause.
    /// Note: SetColumns&lte;T&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: Columns&lte;T&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: ColumnValues&lte;T&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Order entity = new() { Id = 10, Total = 123.45m };
    /// SetColumns&lte;Order&gt; setColumns = new(nameof(Order.Total));
    /// 
    /// Columns&lte;Customer&gt; customerId = new(nameof(Customer.Id));
    /// Columns&lte;Order&gt; orderCustomerId = new(nameof(Order.CustomerId));
    /// Equal customerIdsEquals = new(orderCustomerId, customerId);
    /// InnerJoin&lte;Order, Customer&gt; joinOnCustomerId = new(customerIdsEquals);
    /// 
    /// ColumnValues&lte;Customer&gt; customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");
    /// 
    /// SqlQuery query = orderGenerator.Update(entity, setColumns, joinOnCustomerId, customerEmailEquals);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Order] SET [Order].[Total] = @ParameterSet_Total FROM [Order] 
    /// INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) 
    /// WHERE ([Customer].[Email] = @Parameter_Email)
    /// ]]></code>
    /// </example>
    public SqlQuery Update(T entity, SetColumns<T>? columns, IJoins? joins, PredicatesBase? predicates)
    {
        IEnumerable<ColumnTag> updateTheseColumns =
            (columns?.ColumnTags?.Any() ?? false) ? columns.ColumnTags : ColumnsLessKeys;

        IEnumerable<TableTag> selectTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. predicates?.Column?.Select(col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);

        string setColumnValues = string.Join(", ", updateTheseColumns.Select(column => $"{column} = {GetParameterTagFromColumn(column)?.PrefixPrepend("@ParameterSet")}"));

        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = updateTheseColumns.Select(column => GetSqlParameterKeyValue(column, true, entity, null, "@ParameterSet"));
        Dictionary<ParameterTag, object> parametersDictionary = [.. parameters];


        StringBuilder queryBuilder = new($"UPDATE {Table} SET {setColumnValues} FROM {Table}");

        if (invalidTags.Any())
        {
            throw new ArgumentException($"{nameof(predicates)} contains the following invalid table identifiers: {invalidTags.Select(it => it.ToString()).JoinAnd()}", nameof(predicates));
        }

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {string.Join(' ', joins.ToSql())}");
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
            parametersDictionary.Add(predicates.GetParameters());
        }
        return new SqlQuery(queryBuilder.ToString(), parametersDictionary, CommandType.Text);
    }
}
