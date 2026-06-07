using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PostgreSql;

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
    /// UPDATE "Customer"
    /// SET "Name" = $1, "Email" = $2, "Phone" = $3
    /// WHERE "Id" = $4;
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
    /// UPDATE "Customer"
    /// SET "Email" = $1
    /// WHERE "Id" = $2;
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
    /// UPDATE "Customer"
    /// SET "Name" = $1, "Email" = $2
    /// WHERE (("Customer"."Id" = $3) OR ("Customer"."Id" = $4))
    /// ]]></code>
    /// </example>
    public SqlQuery UpdateByIds(T valuesEntity, ColumnCollectionBase<T>? columns, params IEnumerable<T> idEntities) =>
        base.BaseUpdateByIds(valuesEntity, columns, idEntities);

    /// <summary>
    /// Generates a SQL <c>UPDATE</c> statement that modifies one or more rows,
    /// with optional <c>JOIN</c> and <c>WHERE</c> conditions.
    /// </summary>
    /// <typeparam name="joinsT">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="entity">
    /// The data model instance whose property values supply the column values to set.
    /// </param>
    /// <param name="columns">
    /// Optional column selection. When provided, only these columns are updated; when
    /// <c>null</c>, all non-key columns are updated.
    /// </param>
    /// <param name="from">
    /// Optional PostgreSQL <c>FROM</c> sources used by predicates or joins. Do not include the update target table unless an aliased self-join is intentionally modeled elsewhere.
    /// </param>
    /// <param name="joins">
    /// Optional <see cref="Joins{joinsT}"/> describing joins to append to the PostgreSQL <c>FROM</c> source chain.
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
    /// Thrown when the update target table is included in <paramref name="from"/>, when a join is supplied without the required root <c>FROM</c> source, or when predicates reference tables that do not participate in the statement.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is <c>null</c>.
    /// </exception>
    /// <example>
    /// <para>
    /// Create a PostgreSQL update that uses a <c>FROM</c> source and <c>WHERE</c> predicate.
    /// <see cref="ColumnCollectionBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnValue{T}"/> validates the names of the property, and throws an error if the property isn't valid
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
    ///
    /// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE "Order" 
    /// SET "Total" = $1 
    /// FROM "Customer"
    /// WHERE (("Order"."CustomerId" = "Customer"."Id") 
    ///   AND ("Customer"."Email" = $2))
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>
    /// Create a PostgreSQL update that filters rows with a <c>WHERE</c> predicate.
    /// <see cref="ColumnCollectionBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// <see cref="ColumnValue{T}"/> validates the names of the property, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new()
    /// {
    ///     Email = "spam@example.com"
    /// };
    /// 
    /// ColumnCollection<Customer> columnCollection = new(nameof(Customer.Email));
    /// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");
    /// 
    /// SqlQuery query = customerGenerator.Update<Customer>
    /// (
    ///     entity,
    ///     columnCollection,
    ///     null,
    ///     null,
    ///     customerEmailEquals
    /// );
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// UPDATE "Customer" 
    /// SET "Email" = $1 
    /// WHERE ("Customer"."Email" = $2)
    /// ]]></code>
    /// </example>
    /// <param name="predicates">
    /// Optional <see cref="PredicatesLogic.Predicates"/> describing the <c>WHERE</c> clause that
    /// determines which rows to update.
    /// </param>
    public SqlQuery Update<joinsT>(T entity, ColumnCollectionBase<T>? columns, IEnumerable<TableTag>? from, Joins<joinsT>? joins, Predicates? predicates) where joinsT : class
    {
        TableTag joinsOn = SqlToolsReflectorCache<joinsT>.Table;

        if (from.IsNotNullOrEmpty() && from.Contains(Table))
        {
            throw new InvalidTableException(Table);
        }

        if (joins.IsNotNullOrEmpty() && (from?.DoesNotContain(joinsOn) ?? true))
        {
            throw new InvalidTableException(joinsOn);
        }

        return base.BaseUpdate(entity, columns, from, joins, predicates);
    }
    /// <summary>
    /// Creates a PostgreSQL <c>UPDATE</c> statement with optional <c>FROM</c> sources and a <c>WHERE</c> predicate.
    /// </summary>
    /// <param name="entity">The model instance that supplies values for the <c>SET</c> clause.</param>
    /// <param name="columns">The model properties representing the SQL columns to update.</param>
    /// <param name="from">The PostgreSQL <c>FROM</c> sources available to the predicate.</param>
    /// <param name="predicates">The predicates used to build the SQL <c>WHERE</c> clause.</param>
    /// <returns>A <see cref="SqlQuery"/> representing the generated PostgreSQL <c>UPDATE</c> statement.</returns>
    public SqlQuery Update(T entity, ColumnCollectionBase<T>? columns, IEnumerable<TableTag>? from, Predicates? predicates) 
    {
        if (from.IsNotNullOrEmpty() && from.Contains(Table))
        {
            throw new InvalidTableException(Table);
        }

        return base.BaseUpdate(entity, columns, from, null, predicates);
    }

    /// <summary>
    /// Creates a PostgreSQL <c>UPDATE</c> statement from a builder that can include <c>FROM</c> sources, joins, and a <c>WHERE</c> predicate.
    /// </summary>
    /// <typeparam name="joinsT">The model type used as the root table for the supplied joins.</typeparam>
    /// <param name="updateQuery">The builder containing values, update columns, <c>FROM</c> sources, joins, and predicates.</param>
    /// <returns>A <see cref="SqlQuery"/> representing the generated PostgreSQL <c>UPDATE</c> statement.</returns>
    public SqlQuery Update<joinsT>(UpdateBuilder<T, joinsT> updateQuery) where joinsT : class =>
        Update(updateQuery.Values, updateQuery.UpdateColumns, updateQuery.From, updateQuery.Joins, updateQuery.Where);

    /// <summary>
    /// Creates a PostgreSQL <c>UPDATE</c> statement from a builder that does not include a typed join chain.
    /// </summary>
    /// <param name="updateQuery">The builder containing values, update columns, <c>FROM</c> sources, and predicates.</param>
    /// <returns>A <see cref="SqlQuery"/> representing the generated PostgreSQL <c>UPDATE</c> statement.</returns>
    public SqlQuery Update(UpdateBuilder<T> updateQuery) =>
        Update(updateQuery.Values, updateQuery.UpdateColumns, updateQuery.From, updateQuery.Where);
}
