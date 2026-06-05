
using Carrigan.Core.Interfaces;
using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Builds DELETE query options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type being deleted.</typeparam>

/// <example>
/// <para>Example with null Joins and null predicates</para>
/// <code language="csharp"><![CDATA[
/// DeleteBuilder<Customer> deleteBuilder = new()
/// {
/// };
/// 
/// SqlQuery query = customerGenerator.Delete(deleteBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// DELETE FROM [Customer];
/// ]]></code>
/// </example>
/// <example>
/// <code language="csharp"><![CDATA[
/// ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");
/// DeleteBuilder<Customer> deleteBuilder = new()
/// {
///     Where = columnValue
/// };
/// 
/// SqlQuery query = customerGenerator.Delete(deleteBuilder);;
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// DELETE FROM [Customer] WHERE ([Customer].[Name] = @Name_1)
/// ]]></code>
/// </example>
/// <example>
/// <para>Note: <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if the property isn't valid</para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Customer> join = new(predicate);
/// 
/// DeleteBuilder<Order> deleteBuilder = new()
/// {
///     Joins = join
/// };
/// 
/// SqlQuery query = orderGenerator.Delete(deleteBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// DELETE [Order] 
/// FROM [Order] 
/// INNER JOIN [Customer]
///    ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
/// <example>
/// <para>Note: ColumnEqualsColumn&lt;Customer, Order&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
/// <para>Note: ColumnValues&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin<Customer> join = new(predicate);
/// 
/// ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");
/// 
/// DeleteBuilder<Order> deleteBuilder = new()
/// {
///     Joins = join,
///     Where = customerEmail
/// };
/// 
/// SqlQuery query = orderGenerator.Delete(deleteBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// DELETE [Order] 
/// FROM [Order] 
/// INNER JOIN [Customer] 
///    ON ([Customer].[Id] = [Order].[CustomerId]) 
/// WHERE ([Customer].[Email] = @Email_1)
/// ]]></code>
/// </example>
public sealed record DeleteBuilder<T> : QueryBuilders.DeleteBuilderBase<T, T>, IQueryBuilder where T : class
{
    /// <summary>
    /// Generates SQL for the builder state.
    /// </summary>
    private readonly SqlGenerator<T> SqlGenerator = new();
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBuilder{T}"/> class.
    /// </summary>
    /// <param name="encryption">The optional encryption service used for encrypted model properties.</param>
    public DeleteBuilder(IEncryption? encryption = null) =>
        SqlGenerator = new(encryption);

    /// <summary>
    /// Builds a SQL query from the current builder state.
    /// </summary>
    /// <returns>A <see cref="SqlQuery"/> generated from the current builder state.</returns>
    public SqlQuery AsSqlQuery() =>
        SqlGenerator.Delete(this);
}
