using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Contains SQL generation members for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
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
    /// SET [Name] = @Name_1, [Email] = @Email_2, [Phone] = @Phone_3
    /// WHERE [Id] = @Id_4;
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
    /// SET [Email] = @Email_1 
    /// WHERE [Id] = @Id_2;
    /// ]]></code>
    /// </example>
    public SqlQuery UpdateById(T entity, ColumnCollectionBase<T>? columns = null) =>
        base.BaseUpdateById(entity, columns);


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
    ///  Customer updateValues = new()
    ///  {
    ///      Name = "John Doe",
    ///      Email = string.Empty
    ///  };
    /// 
    ///  IEnumerable<Customer> customerIds =
    ///      [
    ///          new() { Id = 42 },
    ///              new() { Id = 732 }
    ///      ];
    /// 
    ///  ColumnCollection<Customer> updateColumns = new(nameof(Customer.Name), nameof(Customer.Email));
    /// 
    ///  SqlQuery query = customerGenerator.UpdateByIds(updateValues, updateColumns, customerIds);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE [Customer] 
    /// SET [Name] = @Name_1, [Email] = @Email_2 
    /// WHERE (([Customer].[Id] = @Id_3) 
    ///    OR ([Customer].[Id] = @Id_4))
    /// ]]></code>
    /// </example>
    public SqlQuery UpdateByIds(T valuesEntity, ColumnCollectionBase<T>? columns, params IEnumerable<T> idEntities) =>
        base.BaseUpdateByIds(valuesEntity, columns, idEntities);

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
    /// Optional <see cref="Joins{T}"/> describing tables to join for the update.
    /// </param>
    /// <param name="predicates">
    /// Optional <see cref="PredicatesLogic.Predicates"/> describing the <c>WHERE</c> clause that
    /// determines which rows to update.
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
    /// SET [Order].[Total] = @Total_1 
    /// FROM [Order] 
    /// INNER JOIN [Customer] 
    /// ON ([Order].[CustomerId] = [Customer].[Id]) 
    /// WHERE ([Customer].[Email] = @Email_2)
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
    /// SET [Email] = @Email_1
    /// WHERE ([Customer].[Email] = @Email_2
    /// ]]></code>
    /// </example>
    public SqlQuery Update(T entity, ColumnCollectionBase<T>? columns, Joins<T>? joins, Predicates? predicates) =>
        base.BaseUpdate(entity, columns, null, joins, predicates);

    /// <summary>
    /// Builds an UPDATE SQL query for the supplied model data.
    /// </summary>
    /// <param name="updateQuery">The update builder to materialize.</param>
    /// <returns>A <see cref="SqlQuery"/> representing the UPDATE statement.</returns>
    public SqlQuery Update(UpdateBuilder<T> updateQuery) =>
        Update(updateQuery.Values, updateQuery.UpdateColumns, updateQuery.Joins, updateQuery.Where);
}
