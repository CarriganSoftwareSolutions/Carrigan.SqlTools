using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Extensions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{

    /// <summary>
    /// Used to build an SQL Query that selects all records from a table.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <param name="orderBy">optional parameter to add an order by clause</param>
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
    public SqlQuery SelectAll(IOrderByClause? orderBy = null) =>
        Select(null, null, orderBy, null);

    /// <summary>
    /// Builds an SqlQuery object, which contains a parameterized Sql SELECT * with a Dictionary representing the parameter value pairs.
    /// </summary>
    /// <param name="joins">Defines the joins. Leave as null to leave out joins.</param>
    /// <param name="predicates">Defines the WHERE clause. Leave as null to leave out the WHERE clause.</param>
    /// <param name="OrderBy">Defines the ORDER BY clause. Leave as null to leave out the ORDER BY clause.</param>
    /// <param name="offsetNext">Defines the Offset Next clause. Leave as null to leave out the Offset Next clause.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <param name="joins"></param>
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
    /// SqlQuery query = customerGenerator.Select(join, greaterThan, orderByOrderDate, null);
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
    /// SqlQuery query = customerGenerator.Select(null, null, null, offsetNext);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT [Customer].* 
    /// FROM [Customer] 
    /// ORDER BY [Customer].[Id] ASC 
    /// OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY
    /// ]]></code>
    /// </example>
    public SqlQuery Select(IJoins? joins, PredicatesBase? predicates, IOrderByClause? orderBy, OffsetNext? offsetNext)
    {
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. (predicates?.Column?.Select(col => col.TableTag)?.Distinct() ?? [])];
        IEnumerable<TableTag> orderByTableTags = [.. (orderBy?.TableTags?.Distinct() ?? [])];
        IEnumerable<TableTag> invalidPredicateTags = predicateTableTags.Except(selectableTableTags);
        IEnumerable<TableTag> invalidOrderByTags = orderByTableTags.Except(selectableTableTags);
        StringBuilder queryBuilder = new($"SELECT {Table}.* FROM {Table}");

        if (invalidPredicateTags.Any())
        {
            throw new SqlIdentifierException(invalidPredicateTags);
        }
        if (invalidOrderByTags.Any())
        {
            throw new SqlIdentifierException(invalidOrderByTags);
        }

        if (offsetNext is not null)
        {
            //add the key to orderby when using an offset next, this is to overcome a limitation in SQL Server that has unexpected behavior if the order by values are not unique
            orderBy ??= new OrderBy();
            IEnumerable<OrderByItem<T>> oderByKeyItems = [.. Key.Select(key => new OrderByItem<T>(key.Name, SortDirectionEnum.Ascending)).Where(item => orderBy.Contains(item) == false)];
            orderBy = orderBy.WithConcat(oderByKeyItems);
        }

        if (orderBy.IsNullOrEmpty() && offsetNext is not null)
        {
            throw new ArgumentException($"Including an {nameof(offsetNext)} is not supported when {orderBy} is null or empty.");
        }

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {string.Join(' ', joins.ToSql())}");
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
            Parameters = predicates?.GetParameters() ?? [],
            CommandType = CommandType.Text
        };
        
    }

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
        Select(null, new Or(entities.Select(entity => new And(SqlGenerator<T>.GetByKeyPredicates(entity)))), null, null);
}