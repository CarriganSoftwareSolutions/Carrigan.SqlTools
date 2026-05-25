using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.PostgreSql;

public partial class SqlGenerator<T> : SqlGeneratorBase<T> where T : class
{
    /// <summary>
    /// Generates a SQL <c>SELECT *</c> statement that returns all rows
    /// from the table represented by <typeparamref name="T"/>.
    /// </summary>
    /// <param name="orderBy">
    /// Optional ordering to include in the <c>ORDER BY</c> clause.
    /// If omitted, the rows are returned without an explicit order.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>SELECT</c> statement.
    /// </returns>
    /// <remarks>
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="InvalidTableException">
    /// Thrown if any table referenced by <paramref name="orderBy"/> does not participate
    /// in the query (i.e., is not the base table and not included by joins).
    /// </exception>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = customerGenerator.SelectAll();
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* FROM [Customer]
    /// ]]></code>
    /// </example>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// OrderByItem<Customer> orderByItem = new(nameof(Customer.Email));
    /// SqlQuery query = customerGenerator.SelectAll(orderByItem);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// ORDER BY [Customer].[Email] ASC
    /// ]]></code>
    /// </example>
    public SqlQuery SelectAll(OrderBys? orderBy = null) =>
        base.BaseSelectAll(orderBy);

    /// <summary>
    /// Builds an <see cref="SqlQuery"/> containing a parameterized SQL
    /// <c>SELECT</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>JOIN</c>, <c>WHERE</c>, <c>ORDER BY</c>, and
    /// <c>OFFSET … FETCH NEXT</c> clauses.
    /// </summary>
    /// <param name="selects">
    /// Optional projected columns (and result aliases). If omitted or empty, <c>[T].*</c> is selected.
    /// </param>
    /// <param name="joins">
    /// Optional joins to include in the query. Omit to select only from the base table.
    /// </param>
    /// <param name="predicates">
    /// Optional filter predicates to compose the <c>WHERE</c> clause.
    /// </param>
    /// <param name="orderBys">
    /// Optional ordering to compose the <c>ORDER BY</c> clause.
    /// When <paramref name="offsetNext"/> is provided, key columns are appended to
    /// the ordering (if not already present) to ensure stable paging semantics.
    /// </param>
    /// <param name="offsetNext">
    /// Optional paging clause (<c>OFFSET … FETCH NEXT</c>).
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> whose <c>QueryText</c> is the generated SQL and whose
    /// <c>Parameters</c> contain values from <paramref name="predicates"/> and any joins.
    /// </returns>
    /// <remarks>
    /// When providing <paramref name="selects"/>, you will almost certainly need a different model
    /// matching the projected columns to materialize results correctly (since they may no longer map
    /// back to <typeparamref name="T"/>).
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="AmbiguousResultColumnException">
    /// Thrown when <paramref name="selects"/> defines duplicate or ambiguous result column names.
    /// </exception>
    /// <exception cref="InvalidTableException">
    /// Thrown when any table referenced by <paramref name="selects"/>, <paramref name="predicates"/>, or
    /// <paramref name="orderBys"/> is not the base table nor included by <paramref name="joins"/>.
    /// </exception>
    /// <example>
    /// <para>Select with join example:</para>
    /// <para>
    /// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin<Order> join = new (predicate);
    /// 
    /// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Select with join and order by example:</para>
    /// <para>
    /// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// Note: <see cref="OrderBy{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin<Order> join = new(predicate);
    /// 
    /// OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));
    /// 
    /// SqlQuery query = customerGenerator.Select(null, join, null, orderByOrderDate, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// INNER JOIN [Order] 
    /// ON ([Customer].[Id] = [Order].[CustomerId]) 
    /// ORDER BY [Order].[OrderDate] ASC
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Select with join, where, and order by example:</para>
    /// <para>
    /// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// Note: <see cref="Column{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// Note: <see cref="OrderBy{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin<Order> join = new(predicate);
    /// 
    /// Column<Order> totalCol = new(nameof(Order.Total));
    /// Parameter minTotal = new("Total", 500m);
    /// GreaterThan greaterThan = new(totalCol, minTotal);
    /// 
    /// OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));
    /// 
    /// SqlQuery query = customerGenerator.Select(null, join, greaterThan, orderByOrderDate, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// INNER JOIN [Order] 
    /// ON ([Customer].[Id] = [Order].[CustomerId]) 
    /// WHERE ([Order].[Total] > @Parameter_Total) 
    /// ORDER BY [Order].[OrderDate] ASC
    /// ]]></code>
    /// </example>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// OffsetNext offsetNext = new(50, 25);
    /// SqlQuery query = customerGenerator.Select(null, null, null, null, null, offsetNext);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// ORDER BY [Customer].[Id] ASC 
    /// OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY
    /// ]]></code>
    /// </example>
    public SqlQuery Select(bool? distinct, Subquery<T>? subQuery, SelectTags? selects, Joins<T>? joins, Predicates? predicates, OrderBys? orderBys, PagingBase? paging) =>
        base.BaseSelect(distinct, subQuery, selects, joins, predicates, orderBys, paging);

    public SqlQuery Select(SelectBuilder<T> selectQuery) =>
        Select(selectQuery.Distinct, selectQuery.Subquery, selectQuery.Selects, selectQuery.Joins, selectQuery.Where, selectQuery.OrderBys, selectQuery.Paging);

    /// <summary>
    /// Generates a SQL <c>SELECT *</c> statement that returns rows matching the key
    /// properties of the specified entities.
    /// </summary>
    /// <param name="entities">
    /// One or more data model instances used only as ID holders; their key property values
    /// are combined into a predicate that selects matching rows.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>SELECT</c> statement.
    /// </returns>
    /// <remarks>
    /// The generated <c>WHERE</c> clause is composed as an <c>OR</c> of per-entity
    /// <c>AND</c> predicates over the key columns.
    /// Only properties that can be publicly read from accessible types are considered.
    /// Members not visible outside their defining assembly are ignored.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entities"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="entities"/> is empty.
    /// </exception>
    /// <exception cref="NoPrimaryKeyPropertyException{T}">
    /// Thrown when <typeparamref name="T"/> has no key annotations (neither the SQL generator’s
    /// <c>PrimaryKey</c> nor <c>Key</c> attributes) and a “By Id” operation is invoked.
    /// </exception>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new() { Id = 42 };
    /// SqlQuery query = customerGenerator.SelectById(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// WHERE ([Customer].[Id] = @Parameter_Id)
    /// ]]></code>
    /// </example>
    public SqlQuery SelectById(params IEnumerable<T> entities) =>
        base.BaseSelectById(entities);
}