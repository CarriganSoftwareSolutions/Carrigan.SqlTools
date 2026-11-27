
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.QueryBuilders;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds SELECT query options for the specified model type.
/// </summary>
/// <typeparam name="modelT">The model type whose C# properties represent SQL columns or parameters.</typeparam>
/// <example>
/// <para>Select with join example:</para>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new(predicate);
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = join
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// /// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// INNER JOIN [Order] 
///    ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
/// <example>
/// <para>Select with join and order by example:</para>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// Note: <see cref="OrderBy{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new(predicate);
/// 
/// OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = join,
///     OrderBys = orderByOrderDate
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// INNER JOIN [Order] 
///    ON ([Customer].[Id] = [Order].[CustomerId]) 
/// ORDER BY [Order].[OrderDate] ASC
/// ]]></code>
/// </example>
/// <example>
/// <para>Select with join, where, and order by example:</para>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// Note: <see cref="Column{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// Note: <see cref="OrderBy{T}"/> validates the names of the properties, and throws an error if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Order> join = new(predicate);
/// 
/// Column<Order> totalCol = new(nameof(Order.Total));
/// Parameter minTotal = new(500m, "Total");
/// GreaterThan greaterThan = new(totalCol, minTotal);
/// 
/// OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Joins = join,
///     Where = greaterThan,
///     OrderBys = orderByOrderDate
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// INNER JOIN [Order] 
///    ON ([Customer].[Id] = [Order].[CustomerId])
/// WHERE ([Order].[Total] > @Total_1) 
/// ORDER BY [Order].[OrderDate] ASC
/// ]]></code>
/// </example>
public sealed record SelectBuilder<modelT> : SelectBuilder, IQueryBuilder where modelT : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<modelT> SqlGenerator;

    public TableAlias? FromTableAlias { get; init; } 

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectBuilder{T}"/> class.
    /// </summary>
    /// <param name="tableAlias"></param>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public SelectBuilder(TableAlias? tableAlias, IEncryption? encryption = null)
    {
        FromTableAlias = tableAlias;
        base.From = new From<modelT>(tableAlias);
        SqlGenerator = new(encryption);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public SelectBuilder(IEncryption? encryption = null)
    {
        base.From = new From<modelT>();
        SqlGenerator = new(encryption);
    }

    /// <summary>
    /// Gets or sets the FROM clause for the query.
    /// </summary>
    public new From? From =>
        new From<modelT>(FromTableAlias);

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Select(this);
}
