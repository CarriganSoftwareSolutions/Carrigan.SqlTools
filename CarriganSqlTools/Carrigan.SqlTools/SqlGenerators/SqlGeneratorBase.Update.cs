using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

public abstract partial class SqlGeneratorBase<T>
{
    /// <summary>
    /// Generates a SQL <c>UPDATE</c> statement that modifies a single row identified
    /// by the entity’s key properties.
    /// </summary>
    /// <param name="entity">
    /// The data model instance whose key properties identify the target row and whose
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
    /// <remarks>
    /// When generating SQL, only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="NoPrimaryKeyPropertyException{T}">
    /// Thrown when the entity type <typeparamref name="T"/> does not define a primary key property,
    /// but an ID-based update is requested.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="columns"/> selects one or more key properties, or when it selects
    /// no non-key columns.
    /// </exception>
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
    /// <see cref="ColumnCollectionBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnCollection<Customer> columns = new(nameof(Customer.Email));
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
    protected virtual SqlQuery BaseUpdateById(T entity, ColumnCollectionBase<T>? columns = null)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (HasKeyProperty is false)
            throw new NoPrimaryKeyPropertyException<T>();

        IEnumerable<ColumnInfo> updateTheseColumns =
            (columns?.ColumnInfo?.Any() ?? false) ? columns.ColumnInfo : GetGetColumnInfoLessKeys(SupportedTypes);

        IEnumerable<ISqlFragment>GetWhereFragments() =>
            KeyColumnInfo
                .Select
                (
                    column =>
                    new SqlFragmentGroup
                    (
                        new SqlFragmentText($"{column.ColumnTag.ToSql(Dialect, false)} = "),
                        GetSqlParameter(column, entity)
                    )
                )
                .JoinFragments(new SqlFragmentText(" AND "));

        IEnumerable<ISqlFragment> GetFragments()
        {
            yield return new SqlFragmentText("UPDATE ");
            yield return Table;
            yield return new SqlFragmentText(" SET ");
            foreach (ISqlFragment fragment in GetSetFragments(entity, updateTheseColumns, false))
                yield return fragment;
            yield return new SqlFragmentText($" WHERE ");
            foreach (ISqlFragment fragment in GetWhereFragments())
                yield return fragment;
            yield return ISqlFragment.Semicolon;
        }

        return new SqlQuery(Dialect, CommandType.Text, GetFragments());
    }


    /// <summary>
    /// Generates a SQL <c>UPDATE</c> statement that sets column values from
    /// <paramref name="valuesEntity"/> for all rows whose key properties match any of the
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
    /// One or more data model instances used only as ID holders; their key property values
    /// are combined into an <c>OR</c> of per-entity <c>AND</c> predicates to select rows to update.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>UPDATE</c> statement,
    /// including parameters for both the <c>SET</c> values and the key-based <c>WHERE</c> filter.
    /// </returns>
    /// <remarks>
    /// When generating SQL, only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="NoPrimaryKeyPropertyException{T}">
    /// Thrown when the entity type <typeparamref name="T"/> does not define a primary key property,
    /// but an ID-based update is requested.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="valuesEntity"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="idEntities"/> is <c>null</c>.
    /// </exception>
    /// <example>
    /// <para>
    /// <see cref="ColumnCollectionBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
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
    /// ColumnCollection<Customer> updateColumns = new(nameof(Customer.Name), nameof(Customer.Email));
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
    protected virtual SqlQuery BaseUpdateByIds(T valuesEntity, ColumnCollectionBase<T>? columns, params IEnumerable<T> idEntities)
    {
        ArgumentNullException.ThrowIfNull(valuesEntity);

        if (HasKeyProperty is false)
            throw new NoPrimaryKeyPropertyException<T>();
        else
        {
            Or or = new(
                    idEntities.Select(entity => new And
                    (
                        KeyColumnInfo.Select(column => new Equal
                            (
                                GetColumn(column.PropertyName),
                                new Parameter(column.PropertyInfo.GetValue(entity), column.ParameterTag))
                            )
                    ))
            );

            return BaseUpdate(valuesEntity, columns, null, null, or);
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
    /// <param name="from"></param>
    /// <param name="joins">
    /// Optional <see cref="Joins"/> describing tables to join for the update.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>UPDATE</c> statement,
    /// including parameters for the <c>SET</c> values and any predicate values.
    /// </returns>
    /// <remarks>
    /// When generating SQL, only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="InvalidTableException">
    /// Thrown when <paramref name="predicates"/> reference tables that are not present
    /// on the base table or in the specified <paramref name="joins"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is <c>null</c>.
    /// </exception>
    /// <example>
    /// <para>
    /// Create Update SQL query with a Where clause.
    /// <see cref="ColumnCollectionBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnValueBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Order entity = new()
    /// {
    ///     Id = 10,
    ///     Total = 123.45m
    /// };
    /// 
    /// ColumnCollection<Order> columnCollection = new(nameof(Order.Total));
    /// 
    /// Column<Customer> customerId = new(nameof(Customer.Id));
    /// Column<Order> orderCustomerId = new(nameof(Order.CustomerId));
    /// Equal customerIdsEquals = new(orderCustomerId, customerId);
    /// InnerJoin<Customer> joinOnCustomerId = new(customerIdsEquals);
    /// 
    /// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");
    /// 
    /// SqlQuery query = orderGenerator.Update(entity, columnCollection, joinOnCustomerId, customerEmailEquals);
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
    /// <see cref="ColumnCollectionBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnValueBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new()
    /// {
    ///     Email = "spam@example.com"
    /// };
    /// ColumnCollection<Customer> columnCollection = new(nameof(Customer.Email));
    /// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");
    /// 
    /// SqlQuery query = customerGenerator.Update(entity, columnCollection, null, customerEmailEquals);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Customer] 
    /// SET [Email] = @ParameterSet_Email
    /// WHERE ([Customer].[Email] = @Parameter_Email)
    /// ]]></code>
    /// </example>
    /// <param name="predicates">
    /// Optional <see cref="PredicatesLogic.Predicates"/> describing the <c>WHERE</c> clause that
    /// determines which rows to update.
    /// </param>
    protected virtual SqlQuery BaseUpdate(T entity, ColumnCollectionBase<T>? columns, IEnumerable<TableTag>? from, JoinsBase? joins, Predicates? predicates)
    {
        ArgumentNullException.ThrowIfNull(entity);

        IEnumerable<ColumnInfo> updateTheseColumns =
            [.. ((columns?.ColumnInfo?.Any() ?? false) ? columns.ColumnInfo : GetGetColumnInfoLessKeys(SupportedTypes))];

        IEnumerable<TableTag> selectTableTags = (from ?? []).Prepend(Table).Concat(joins?.TableTags ?? []).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. predicates?.DescendantColumns?.Select(col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectTableTags);
        bool useFullyQualifiedSets = Dialect.DoesUpdateSupportsFullyQualifiedSets() && joins.IsNotNullOrEmpty();

        if (invalidTags.Any())
        {
            throw new InvalidTableException(invalidTags);
        }

        IEnumerable<ISqlFragment> GetFragments()
        {
            yield return new SqlFragmentText("UPDATE ");
            yield return Table;

            yield return new SqlFragmentText(" SET ");

            foreach (ISqlFragment fragment in GetSetFragments(entity, updateTheseColumns, useFullyQualifiedSets))
                yield return fragment;


            if (from.IsNotNullOrEmpty())
            {
                yield return new SqlFragmentText(" FROM ");
                foreach (ISqlFragment fragment in from.JoinFragments(", "))
                    yield return fragment;
            }
            else if (joins.IsNotNullOrEmpty())
            {
                yield return new SqlFragmentText(" FROM ");
                yield return Table;
            }

            if (joins.IsNotNullOrEmpty())
            {
                foreach (ISqlFragment fragment in joins.ToSqlFragments(Dialect))
                    yield return fragment;
            }

            if(predicates is not null)
            {
                yield return new SqlFragmentText($" WHERE ");
                foreach (ISqlFragment fragment in predicates.ToSqlFragments(Dialect))
                    yield return fragment;
            }
        }

        return new SqlQuery(Dialect, CommandType.Text, GetFragments());
    }


    /// <summary>
    /// Generates the SQL fragments for the <c>SET</c> clause of an <c>UPDATE</c> statement,
    /// </summary>
    /// <param name="entity">the entity containing the values to be updated</param>
    /// <param name="updateTheseColumns">the columns to be updated</param>
    /// <param name="useTableTag">whether to use the table tag in the SQL fragments</param>
    /// <returns></returns>
    private IEnumerable<ISqlFragment> GetSetFragments(T entity, IEnumerable<ColumnInfo> updateTheseColumns, bool useTableTag) =>
        updateTheseColumns
            .Select
            (
                column =>
                new SqlFragmentGroup
                (
                    new SqlFragmentText($"{column.ColumnTag.ToSql(Dialect, useTableTag)} = "),
                    GetSqlParameter(column, entity)
                )
            )
            .JoinFragments(new SqlFragmentText(", "));
}
