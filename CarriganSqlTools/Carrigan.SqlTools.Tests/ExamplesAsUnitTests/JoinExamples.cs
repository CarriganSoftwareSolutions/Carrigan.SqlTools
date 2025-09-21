using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class JoinExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithInnerJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Customer, Order> join = new(predicate);

        SqlQuery query = customerGenerator.Select(join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        Join<Customer, Order> join = new(predicate);

        SqlQuery query = customerGenerator.Select(join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithLeftJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        LeftJoin<Customer, Order> join = new(predicate);

        SqlQuery query = customerGenerator.Select(join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithTwoJoins()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Customer, Order> join1 = new(customerIdEquals);

        ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
        InnerJoin<Order, PaymentMethod> join2 = new(paymentMethodIdEquals);

        Joins joins = new(join1, join2);

        SqlQuery query = customerGenerator.Select(joins, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) INNER JOIN [PaymentMethod] ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }
}
