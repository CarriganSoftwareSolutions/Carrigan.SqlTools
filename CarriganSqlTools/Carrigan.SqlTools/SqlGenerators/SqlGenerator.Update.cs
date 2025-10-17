using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Generates a SQL <c>UPDATE</c> statement that modifies a single row identified
    /// by the entity’s key fields.
    /// </summary>
    /// <param name="entity">
    /// The data model instance whose key fields identify the target row and whose
    /// property values supply the column values to set.
    /// </param>
    /// <param name="columns">
    /// Optional column selection. When provided, only these columns are updated; when
    /// <c>null</c>, all non-key columns are updated.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>UPDATE</c> statement,
    /// including parameters for both the <c>SET</c> values and the key-based <c>WHERE</c> filter.
    /// </returns>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new()
    /// {
    ///     Id = 42,
    ///     Name = "Hank",
    ///     Email = "Hank@example.com",
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
    /// <para>
    /// <see cref="SetColumns{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// SetColumns<Customer> columns = new(nameof(Customer.Email));
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
        if (HasKeyField is false)
            throw new NoPrimaryKeyField<T>();

        IEnumerable<ColumnInfo> updateTheseColumns = 
            (columns?.ColumnInfo?.Any() ?? false) ? columns.ColumnInfo : ColumnInfoLessKeys;

        IEnumerable<KeyValuePair<ParameterTag, object>> parameters = updateTheseColumns.Concat(KeyColumnInfo).Select(column => GetSqlParameterKeyValue(column, entity));

        List<Tuple<ColumnTag, ParameterTag>> setColumnAndParameterName = [];
        foreach (ColumnInfo column in updateTheseColumns)
        {
            KeyValuePair<ParameterTag, object> parameter = GetSqlParameterKeyValue(column, entity);
            setColumnAndParameterName.Add(new(column.ColumnTag, parameter.Key));
        }
        string sets = string.Join(", ", setColumnAndParameterName.Select(columnParameter => $"{columnParameter.Item1.ToString(false)} = @{columnParameter.Item2}"));

        List<Tuple<ColumnTag, string>> whereColumnAndParameterName = [];
        foreach (ColumnInfo column in KeyColumnInfo)
        {
            KeyValuePair<ParameterTag, object> parameter = GetSqlParameterKeyValue(column, entity);
            whereColumnAndParameterName.Add(new(column.ColumnTag, parameter.Key));
        }
        string where = string.Join(", ", whereColumnAndParameterName.Select(columnParameter => $"{columnParameter.Item1.ToString(false)} = @{columnParameter.Item2}"));

        return new SqlQuery()
        {
            Parameters = [.. parameters],
            QueryText = $"UPDATE {Table} SET {sets} WHERE {where};",
            CommandType = CommandType.Text
        };
    }

    /// <summary>
    /// Generates a SQL <c>UPDATE</c> statement that sets column values from
    /// <paramref name="valuesEntity"/> for all rows whose key fields match any of the
    /// specified <paramref name="idEntities"/>.
    /// </summary>
    /// <param name="valuesEntity">
    /// The data model instance providing the values for the <c>SET</c> clause.
    /// </param>
    /// <param name="columns">
    /// Optional column selection. When provided, only these columns are updated; when
    /// <c>null</c>, all non-key columns are updated.
    /// </param>
    /// <param name="idEntities">
    /// One or more data model instances used only as ID holders; their key field values
    /// are combined into an <c>OR</c> of per-entity <c>AND</c> predicates to select rows to update.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>UPDATE</c> statement,
    /// including parameters for both the <c>SET</c> values and the key-based <c>WHERE</c> filter.
    /// </returns>
    /// <remarks>
    /// The data model type must be <c>public</c>, and any properties intended to map to
    /// columns must be public instance properties with a public getter.
    /// </remarks>
    /// <example>
    /// <para>
    /// <see cref="SetColumns{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Customer updateValues = new()
    /// {
    ///     Name = "John Doe",
    ///     Email = string.Empty
    /// };
    /// 
    /// IEnumerable<Customer> customerIds =
    ///     [
    ///         new() { Id = 42 },
    ///             new() { Id = 732 }
    ///     ];
    /// 
    /// SetColumns<Customer> updateColumns = new(nameof(Customer.Name), nameof(Customer.Email));
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
        if (HasKeyField is false)
            throw new NoPrimaryKeyField<T>();
        else
        {
            Or or = new(
                    idEntities.Select(entity => new And
                    (
                        KeyColumnInfo.Select(column => new Equal
                            (
                                new Column<T>(column.PropertyName),
                                new Parameter(column.ParameterTag, column.PropertyInfo.GetValue(entity)))
                            )
                    ))
            );

            return Update(valuesEntity, columns, null, or);
        }
    }

    /// <summary>
    /// Generates a SQL <c>UPDATE</c> statement that modifies one or more rows,
    /// with optional <c>JOIN</c> and <c>WHERE</c> conditions.
    /// </summary>
    /// <param name="entity">
    /// The data model instance whose property values supply the column values to set.
    /// </param>
    /// <param name="columns">
    /// Optional column selection. When provided, only these columns are updated; when
    /// <c>null</c>, all non-key columns are updated.
    /// </param>
    /// <param name="joins">
    /// Optional <see cref="Joins"/> describing tables to join for the update.
    /// </param>
    /// <param name="predicates">
    /// Optional <see cref="PredicatesLogic.Predicates"/> describing the <c>WHERE</c> clause that
    /// determines which rows to update.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>UPDATE</c> statement,
    /// including parameters for the <c>SET</c> values and any predicate values.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="predicates"/> reference tables that are not present
    /// in the base table or the specified <paramref name="joins"/>.
    /// </exception>
    /// <example>
    /// <para>
    /// Create Update SQL query with a Where clause.
    /// <see cref="SetColumns{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnValue{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Order entity = new()
    /// {
    ///     Id = 10,
    ///     Total = 123.45m
    /// };
    /// 
    /// SetColumns<Order> setColumns = new(nameof(Order.Total));
    /// 
    /// Column<Customer> customerId = new(nameof(Customer.Id));
    /// Column<Order> orderCustomerId = new(nameof(Order.CustomerId));
    /// Equal customerIdsEquals = new(orderCustomerId, customerId);
    /// Joins<Order> joinOnCustomerId = Joins<Order>.InnerJoin<Customer>(customerIdsEquals);
    /// 
    /// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");
    /// 
    /// SqlQuery query = orderGenerator.Update(entity, setColumns, joinOnCustomerId, customerEmailEquals);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Order] 
    /// SET [Order].[Total] = @ParameterSet_Total 
    /// FROM [Order] 
    /// INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) 
    /// WHERE ([Customer].[Email] = @Parameter_Email)
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>
    /// Create Update SQL query with Joins and a Where clause.
    /// <see cref="SetColumns{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnValue{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new()
    /// {
    ///     Email = "spam@example.com"
    /// };
    /// SetColumns<Customer> setColumns = new(nameof(Customer.Email));
    /// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");
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
    public SqlQuery Update(T entity, SetColumns<T>? columns, JoinsBase? joins, Predicates? predicates)
    {
        IEnumerable<ColumnInfo> updateTheseColumns =
            (columns?.ColumnInfo?.Any() ?? false) ? columns.ColumnInfo : ColumnInfoLessKeys;

        IEnumerable<TableTag> selectTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. predicates?.Columns?.Select(col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);

        if (invalidTags.Any())
        {
            throw new ArgumentException($"{nameof(predicates)} contains the following invalid table identifiers: {invalidTags.Select(it => it.ToString()).JoinAnd()}", nameof(predicates));
        }

        Dictionary<ParameterTag, object> parametersDictionary = [.. updateTheseColumns.Select(column => GetSqlParameterKeyValue(column, entity, null, "@ParameterSet"))];

        string setColumnValues = string.Join(", ", updateTheseColumns.Select(column => $"{column} = {column.ParameterTag.PrefixPrepend("@ParameterSet")}"));
        StringBuilder queryBuilder = new($"UPDATE {Table} SET {setColumnValues} FROM {Table}");

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {joins.ToSql()}");
            parametersDictionary.Add(joins.Parameters);
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
            parametersDictionary.Add(predicates.GetParameters());
        }
        return new SqlQuery()
        {
            QueryText = queryBuilder.ToString(),
            Parameters = parametersDictionary,
            CommandType = CommandType.Text
        };
    }
}
