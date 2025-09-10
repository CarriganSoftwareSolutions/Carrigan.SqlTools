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

    [Fact]
    public void SelectWithDefinePage()
    {
        DefinePage definePage = new(2, 25); 
        SqlQuery query = customerGenerator.Select(null, null, null, definePage);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithDefinePageWithOrderBy()
    {
        DefinePage definePage = new(2, 25);
        OrderByItem<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, orderBy, definePage);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithOffsetNext()
    {
        OffsetNext offsetNext = new(50, 25);
        SqlQuery query = customerGenerator.Select(null, null, null, offsetNext);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Id] ASC OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithOffsetNextPageWithOrderBy()
    {
        OffsetNext offsetNext = new(50, 25);
        OrderByItem<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, orderBy, offsetNext);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithWithOrderByItem()
    { 
        OrderByItem<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, orderBy, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithWithTwoOrderByItems()
    {
        OrderByItem<Customer> orderBy1 = new(nameof(Customer.Name));
        OrderByItem<Customer> orderBy2 = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBy orderBy = new(orderBy1, orderBy2);
        SqlQuery query = customerGenerator.Select(null, null, orderBy, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] DESC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }
    #endregion
}
