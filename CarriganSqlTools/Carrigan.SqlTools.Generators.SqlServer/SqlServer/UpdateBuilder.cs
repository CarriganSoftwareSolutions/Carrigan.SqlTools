
using Carrigan.Core.Interfaces;
using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds UPDATE query options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being updated.</typeparam>
/// <example>
/// <para>
/// Create Update SQL query with a Where clause.
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
/// InnerJoin<Customer> joinOnCustomerId = new(customerIdsEquals);
/// 
/// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");
/// 
/// UpdateBuilder<Order> updateBuilder = new()
/// {
///     Values = entity,
///     UpdateColumns = columnCollection,
///     Joins = joinOnCustomerId,
///     Where = customerEmailEquals
/// };
/// 
/// SqlQuery query = orderGenerator.Update(updateBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [Order] 
/// SET [Order].[Total] = @Total_1 FROM [Order] 
/// INNER JOIN [Customer] 
///   ON ([Order].[CustomerId] = [Customer].[Id]) 
/// WHERE ([Customer].[Email] = @Email_2)
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Create Update SQL query with Joins and a Where clause.
/// <see cref="ColumnCollectionBase{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// <see cref="ColumnValue{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Customer entity = new()
/// {
///     Email = "spam@example.com"
/// };
/// ColumnCollection<Customer> columnCollection = new(nameof(Customer.Email));
/// ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");
/// 
/// UpdateBuilder<Customer> updateBuilder = new()
/// {
///     Values = entity,
///     UpdateColumns = columnCollection,
///     Where = customerEmailEquals
/// };
/// 
/// SqlQuery query = customerGenerator.Update(updateBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [Customer] 
/// SET [Email] = @Email_1 
/// WHERE ([Customer].[Email] = @Email_2)
/// ]]></code>
/// </example>
public sealed record UpdateBuilder<T> : QueryBuilders.UpdateBuilderBase<T, T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator;
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public UpdateBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);
    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Update(this);
}
