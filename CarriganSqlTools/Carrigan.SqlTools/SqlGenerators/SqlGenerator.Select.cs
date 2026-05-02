using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
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
    public SqlQuery SelectAll(OrderByBase? orderBy = null) =>
        Select(null, null, null, orderBy, null);

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
    /// <param name="orderBy">
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
    /// <paramref name="orderBy"/> is not the base table nor included by <paramref name="joins"/>.
    /// </exception>
    /// <example>
    /// <para>Select with join example:</para>
    /// <para>
    /// Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// Joins<Customer> join = Joins<Customer>.InnerJoin<Order>(predicate);
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
    /// Note: <see cref="OrderByItem{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// Joins<Customer> join = Joins<Customer>.InnerJoin<Order>(predicate);
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
    /// Note: <see cref="OrderByItem{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// Joins<Customer> join = Joins<Customer>.InnerJoin<Order>(predicate);
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
    /// SqlQuery query = customerGenerator.Select(null, null, null, null, offsetNext);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// ORDER BY [Customer].[Id] ASC 
    /// OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY
    /// ]]></code>
    /// </example>
    public SqlQuery Select(SelectTagsBase? selects, JoinsBase? joins, Predicates? predicates, OrderByBase? orderBy, OffsetNext? offsetNext)
    {
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
        IEnumerable<TableTag> selectedTableTags = [.. selects?.GetTableTags() ?? []];
        IEnumerable<TableTag> invalidSelectedTags = selectedTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> predicateTableTags = [.. predicates?.DescendantColumns?.Select(static col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidPredicateTags = predicateTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> orderByTableTags = [.. orderBy?.TableTags?.Distinct() ?? []];
        IEnumerable<TableTag> invalidOrderByTags = orderByTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> invalidTags = invalidSelectedTags.Concat(invalidPredicateTags).Concat(invalidOrderByTags).Distinct();

        AmbiguousResultColumnException? ambiguousResultColumns = AmbiguousResultColumnException.CheckNames(selects);
        if (ambiguousResultColumns is not null)
            throw ambiguousResultColumns;

        if (invalidTags.Any())
            throw new InvalidTableException(invalidTags);

        if (offsetNext is not null)
        {
            // add the key to orderby when using an offset next, this is to overcome a limitation in SQL Server
            // that has unexpected behavior if the order by values are not unique
            orderBy ??= new OrderBy();
            IEnumerable<OrderByItem<T>> orderByKeyItems =
            [
                .. KeyColumnInfo
                    .Select(static key => new OrderByItem<T>(key.PropertyName, SortDirectionEnum.Ascending))
                    .Where(item => orderBy.Contains(item) == false)
            ];
            orderBy = orderBy.WithConcat(orderByKeyItems);
        }

        IEnumerable<SqlFragment> queryFragments = [];
        if (selects is not null && selects.Any())
            queryFragments = queryFragments.Append(new SqlFragmentText($"SELECT {selects.ToSql()} FROM {Table}"));
        else if (HasAliasedColumns)
            queryFragments = queryFragments.Append(new SqlFragmentText($"SELECT {SelectTags.ToSql()} FROM {Table}"));
        else
            queryFragments = queryFragments.Append(new SqlFragmentText($"SELECT {Table}.* FROM {Table}"));

        if (joins?.IsNotNullOrEmpty() ?? false)
            queryFragments = queryFragments.Concat(joins.ToSqlFragments());

        if (predicates is not null)
            queryFragments = queryFragments.Append(new SqlFragmentText($" WHERE ")).Concat(predicates.ToSqlFragments());

        if (orderBy.IsNotNullOrEmpty())
            queryFragments = queryFragments.Append(new SqlFragmentText($" {orderBy.AsOrderBy().ToSql()}"));


        if (offsetNext is not null)
            queryFragments = queryFragments.Append(new SqlFragmentText($" {offsetNext.ToSql()}"));

        return queryFragments.ToSqlQuery(Dialect);
    }

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
    public SqlQuery SelectById(params IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        if (HasKeyProperty is false)
            throw new NoPrimaryKeyPropertyException<T>();
        else
            return Select(null, null, new Or(entities.Select(entity => new And(SqlGenerator<T>.GetByKeyPredicates(entity)))), null, null);
    }
}