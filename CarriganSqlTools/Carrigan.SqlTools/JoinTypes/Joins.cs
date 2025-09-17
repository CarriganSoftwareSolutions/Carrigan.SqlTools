using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// This class represents multiple joins to be strung together.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Columns&lt;Customer&gt; id = new(nameof(Customer.Id));
/// Columns&lt;Order&gt; customerId = new(nameof(Order.CustomerId));
/// Equal customerIdEquals = new(id, customerId);
/// InnerJoin&lt;Customer, Order&gt; join1 = new(customerIdEquals);
/// 
/// Columns&lt;Order&gt; orderPaymentMethodId = new(nameof(Order.PaymentMethodId));
/// Columns&lt;PaymentMethod&gt; paymentMethodId = new(nameof(PaymentMethod.Id));
/// Equal paymentMethodIdEquals = new(orderPaymentMethodId, paymentMethodId);
/// InnerJoin&lt;Order, PaymentMethod&gt; join2 = new(paymentMethodIdEquals);
/// 
/// Joins joins = new(join1, join2);
/// 
/// SqlQuery query = customerGenerator.Select(joins, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) 
/// INNER JOIN [PaymentMethod] ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
/// ]]></code>
/// </example>
public class Joins : IJoins
{
    /// <summary>
    /// An enumeration of all the joins.
    /// </summary>
    public IEnumerable<IJoins> Joints { get; private set; }
    public Joins(params IEnumerable<IJoins> joins) =>
        Joints = joins;

    /// <summary>
    /// This enumeration provides a quick way to determine what all tables are involved in all of the Joins.
    /// </summary>
    public IEnumerable<TableTag> TableTags =>
        Joints.SelectMany(join => join.TableTags);

    /// <summary>
    /// This generates the SQL for the Joins as a string.
    /// </summary>
    /// <returns>A string for the Joins' SQL</returns>
    public string ToSql() =>
        string.Join(" ", Joints.Select(join => join.ToSql()));
}
