using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
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
    /// <exception cref="InvalidTableException">
    /// Thrown if the generated query references invalid or unrecognized table identifiers.
    /// </exception>
    /// <remarks>
    /// The data model type must be <c>public</c>, and any properties intended to map
    /// to columns must be public instance properties with a public getter.
    /// </remarks>
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
    /// OrderByItem&lt;Customer&gt; orderByItem = new(nameof(Customer.Email));
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
    /// <c>SELECT *</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>JOIN</c>, <c>WHERE</c>, <c>ORDER BY</c>, and
    /// <c>OFFSET … FETCH NEXT</c> clauses.
    /// </summary>
    /// <param name="selects"></param>
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
    /// <returns>
    /// An <see cref="SqlQuery"/> whose <c>QueryText</c> is the generated SQL and whose
    /// <c>Parameters</c> contain values from <paramref name="predicates"/>.
    /// </returns>
    /// <exception cref="InvalidTableException">
    /// Thrown if the generated query references invalid or unrecognized table identifiers.
    /// </exception>
    /// <exception cref="AmbiguousResultColumnException">
    /// Thrown if there is an ambiguous select tag.
    /// </exception>
    /// <remarks>
    /// The data model type <typeparamref name="T"/> must be <c>public</c>, and any properties
    /// intended to map to columns must be public instance properties with a public getter.
    /// </remarks>
    /// <example>
    /// <para>Select with join example:</para>
    /// <para>
    /// Note: ColumnEqualsColumn&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: InnerJoin&lt;Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn&lt;Customer, Order&gt; columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin&lt;Customer, Order&gt; join = new(columnEqualsColumn);
    /// 
    /// SqlQuery query = customerGenerator.Select(join, null, null, null);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// INNER JOIN [Order] 
    /// ON ([Customer].[Id] = [Order].[CustomerId])
    /// ]]></code>
    /// </example>
    /// <example>
    /// <para>Select with join and order by example:</para>
    /// <para>
    /// Note: ColumnEqualsColumn&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: InnerJoin&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: OrderByItem&lt;Order&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn&lt;Customer, Order&gt; columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin&lt;Customer, Order&gt; join = new(columnEqualsColumn);
    /// 
    /// OrderByItem&lt;Order&gt; orderByOrderDate = new(nameof(Order.OrderDate));
    /// 
    /// SqlQuery query = customerGenerator.Select(join, null, orderByOrderDate, null);
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
    /// <para>Select with join and order by example:</para>
    /// <para>
    /// Note: ColumnEqualsColumn&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid
    /// Note: InnerJoin&lt;Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
    /// Note: Columns&lt;Order> validates the names of the properties, and throws an error if the property isn't valid
    /// Note: OrderByItem&lt;Order> validates the names of the properties, and throws an error if the property isn't valid
    /// </para>
    /// <code language="csharp"><![CDATA[
    /// ColumnEqualsColumn&lt;Customer, Order&gt; columnEqualsColumn = new(nameof(Customer.Id), nameof(Order.CustomerId));
    /// InnerJoin&lt;Customer, Order&gt; join = new(columnEqualsColumn);
    /// 
    /// Columns&lt;Order&gt; totalCol = new(nameof(Order.Total));
    /// Parameters minTotal = new("Total", 500m);
    /// GreaterThan greaterThan = new(totalCol, minTotal);
    /// 
    /// OrderByItem&lt;Order&gt; orderByOrderDate = new(nameof(Order.OrderDate));
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
    /// <param name="offsetNext">
    /// Optional paging clause (<c>OFFSET … FETCH NEXT</c>).
    /// </param>
    public SqlQuery Select(SelectTagsBase? selects, JoinsBase? joins, Predicates? predicates, OrderByBase? orderBy, OffsetNext? offsetNext)
    {
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
        IEnumerable<TableTag> selectedTableTags = [.. selects?.GetTableTags() ?? []];
        IEnumerable<TableTag> invalidSelectedTags = selectedTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> predicateTableTags = [.. predicates?.Columns?.Select(col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidPredicateTags = predicateTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> orderByTableTags = [.. orderBy?.TableTags?.Distinct() ?? []];
        IEnumerable<TableTag> invalidOrderByTags = orderByTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> invalidTags = invalidSelectedTags.Concat(invalidPredicateTags).Concat(invalidOrderByTags).Distinct();
        StringBuilder queryBuilder;
        AmbiguousResultColumnException? ambiguousResultColumns = AmbiguousResultColumnException.CheckNames(selects);
        if (ambiguousResultColumns is not null)
            throw ambiguousResultColumns;

        if (invalidTags.Any())
            throw new InvalidTableException(invalidTags);


        if (offsetNext is not null)
        {
            //add the key to orderby when using an offset next, this is to overcome a limitation in SQL Server that has unexpected behavior if the order by values are not unique
            orderBy ??= new OrderBy();
            IEnumerable<OrderByItem<T>> oderByKeyItems = [.. KeyColumnInfo.Select(key => new OrderByItem<T>(key.PropertyName, SortDirectionEnum.Ascending)).Where(item => orderBy.Contains(item) == false)];
            orderBy = orderBy.WithConcat(oderByKeyItems);
        }


        if(selects is not null &&  selects.Any())
            queryBuilder = new($"SELECT {selects.GetSelects()} FROM {Table}");
        else
            queryBuilder = new($"SELECT {Table}.* FROM {Table}");

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {joins.ToSql()}");
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
        }
        if(orderBy.IsNotNullOrEmpty())
        {
            queryBuilder.Append($" {orderBy.AsOrderBy().ToSql()}");
        }
        if(offsetNext is not null)
        {
            queryBuilder.Append($" {offsetNext.ToSql()}");
        }
        return new SqlQuery()
        {
            QueryText = queryBuilder.ToString(),
            Parameters = [.. (joins?.Parameters ?? []).Concat(predicates?.GetParameters() ?? [])],
            CommandType = CommandType.Text
        };        
    }

    /// <summary>
    /// Generates a SQL <c>SELECT *</c> statement that returns rows matching the key
    /// fields of the specified entities.
    /// </summary>
    /// <param name="entities">
    /// One or more data model instances used only as ID holders; their key field values
    /// are combined into a predicate that selects matching rows.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> representing the generated <c>SELECT</c> statement.
    /// </returns>
    /// <remarks>
    /// The generated <c>WHERE</c> clause is composed as an <c>OR</c> of per-entity
    /// <c>AND</c> predicates over the key columns.
    /// The data model type must be <c>public</c>, and any properties intended to map
    /// to columns must be public instance properties with a public getter.
    /// </remarks>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Customer entity = new() { Id = 42 };
    /// SqlQuery query = customerGenerator.SelectById(entity);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* FROM [Customer] 
    /// WHERE ([Customer].[Id] = @Parameter_Id)
    /// ]]></code>
    /// </example>
    public SqlQuery SelectById(params T[] entities) =>
        Select(null, null, new Or(entities.Select(entity => new And(SqlGenerator<T>.GetByKeyPredicates(entity)))), null, null);
}