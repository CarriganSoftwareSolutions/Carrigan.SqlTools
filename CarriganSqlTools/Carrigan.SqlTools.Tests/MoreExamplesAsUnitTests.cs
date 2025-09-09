using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


namespace Carrigan.SqlTools.Tests;
public class MoreExamplesAsUnitTests
{
    private SqlGenerator<Customer> customerGenerator = new();
    private SqlGenerator<Order> orderGenerator = new();

    #region Joins


    [Fact]
    public void SelectWithInnerJoin()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Columns<Customer> id = new(nameof(Customer.Id));
        Columns<Order> customerId = new(nameof(Order.CustomerId));
        Equal equals = new(id, customerId);
        InnerJoin<Customer, Order> join = new(equals);

        SqlQuery query = customerGenerator.Select(join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithJoin()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Columns<Customer> id = new(nameof(Customer.Id));
        Columns<Order> customerId = new(nameof(Order.CustomerId));
        Equal equals = new(id, customerId);
        Join<Customer, Order> join = new(equals);

        SqlQuery query = customerGenerator.Select(join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithLeftJoin()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Columns<Customer> id = new(nameof(Customer.Id));
        Columns<Order> customerId = new(nameof(Order.CustomerId));
        Equal equals = new(id, customerId);
        LeftJoin<Customer, Order> join = new(equals);

        SqlQuery query = customerGenerator.Select(join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithTwoJoins()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Columns<Customer> id = new(nameof(Customer.Id));
        Columns<Order> customerId = new(nameof(Order.CustomerId));
        Equal customerIdEquals = new(id, customerId);
        InnerJoin<Customer, Order> join1 = new(customerIdEquals);

        Columns<Order> orderPaymentMethodId = new(nameof(Order.PaymentMethodId));
        Columns<PaymentMethod> paymentMethodId = new(nameof(PaymentMethod.Id));
        Equal paymentMethodIdEquals = new(orderPaymentMethodId, paymentMethodId);
        InnerJoin<Order, PaymentMethod> join2 = new(paymentMethodIdEquals);

        Joins joins = new(join1, join2);

        SqlQuery query = customerGenerator.Select(joins, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) INNER JOIN [PaymentMethod] ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }
    #endregion
}
