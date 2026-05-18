using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.SqlServer.IntegrationTests.DataSets;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests;

public sealed class JoinsTests : IClassFixture<JoinsFixture>
{
    private readonly JoinsFixture _fixture;

    private readonly SqlGenerator<Customer> CustomerSqlGenerator = new();
    private readonly SqlGenerator<Order> OrderSqlGenerator = new();

    public JoinsTests(JoinsFixture fixture) =>
        _fixture = fixture;


    [Fact]
    public async Task Join()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new Join<Order>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = CustomerSqlGenerator.Select(selectTags, join, null, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<CustomerOrder> customerOrders = await CommandsAsync.ExecuteReaderAsync<CustomerOrder>(query, null, unitTestConnection);

        Assert.Equal(36, customerOrders.Count());
        ValidateJoin(customerOrders, 1, 1);
        ValidateJoin(customerOrders, 1, 2);
        ValidateJoin(customerOrders, 1, 3);
        ValidateJoin(customerOrders, 1, 4);
        ValidateJoin(customerOrders, 1, 5);
        ValidateJoin(customerOrders, 2, 6);
        ValidateJoin(customerOrders, 2, 7);
        ValidateJoin(customerOrders, 2, 8);
        ValidateJoin(customerOrders, 2, 9);
        ValidateJoin(customerOrders, 3, 10);
        ValidateJoin(customerOrders, 3, 11);
        ValidateJoin(customerOrders, 3, 12);
        ValidateJoin(customerOrders, 4, 13);
        ValidateJoin(customerOrders, 4, 14);
        ValidateJoin(customerOrders, 4, 15);
        ValidateJoin(customerOrders, 5, 16);
        ValidateJoin(customerOrders, 5, 17);
        ValidateJoin(customerOrders, 6, 18);
        ValidateJoin(customerOrders, 6, 19);
        ValidateJoin(customerOrders, 7, 20);
        ValidateJoin(customerOrders, 7, 21);
        ValidateJoin(customerOrders, 8, 22);
        ValidateJoin(customerOrders, 8, 23);
        ValidateJoin(customerOrders, 9, 24);
        ValidateJoin(customerOrders, 9, 25);
        ValidateJoin(customerOrders, 10, 26);
        ValidateJoin(customerOrders, 11, 27);
        ValidateJoin(customerOrders, 12, 28);
        ValidateJoin(customerOrders, 13, 29);
        ValidateJoin(customerOrders, 14, 30);
        ValidateJoin(customerOrders, 15, 31);
        ValidateJoin(customerOrders, 16, 32);
        ValidateJoin(customerOrders, 17, 33);
        ValidateJoin(customerOrders, 18, 34);
        ValidateJoin(customerOrders, 19, 35);
        ValidateJoin(customerOrders, 20, 36);
    }

    [Fact]
    public async Task InnerJoin()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new InnerJoin<Order>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = CustomerSqlGenerator.Select(selectTags, join, null, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<CustomerOrder> customerOrders = await CommandsAsync.ExecuteReaderAsync<CustomerOrder>(query, null, unitTestConnection);

        Assert.Equal(36, customerOrders.Count());
        ValidateJoin(customerOrders, 1, 1);
        ValidateJoin(customerOrders, 1, 2);
        ValidateJoin(customerOrders, 1, 3);
        ValidateJoin(customerOrders, 1, 4);
        ValidateJoin(customerOrders, 1, 5);
        ValidateJoin(customerOrders, 2, 6);
        ValidateJoin(customerOrders, 2, 7);
        ValidateJoin(customerOrders, 2, 8);
        ValidateJoin(customerOrders, 2, 9);
        ValidateJoin(customerOrders, 3, 10);
        ValidateJoin(customerOrders, 3, 11);
        ValidateJoin(customerOrders, 3, 12);
        ValidateJoin(customerOrders, 4, 13);
        ValidateJoin(customerOrders, 4, 14);
        ValidateJoin(customerOrders, 4, 15);
        ValidateJoin(customerOrders, 5, 16);
        ValidateJoin(customerOrders, 5, 17);
        ValidateJoin(customerOrders, 6, 18);
        ValidateJoin(customerOrders, 6, 19);
        ValidateJoin(customerOrders, 7, 20);
        ValidateJoin(customerOrders, 7, 21);
        ValidateJoin(customerOrders, 8, 22);
        ValidateJoin(customerOrders, 8, 23);
        ValidateJoin(customerOrders, 9, 24);
        ValidateJoin(customerOrders, 9, 25);
        ValidateJoin(customerOrders, 10, 26);
        ValidateJoin(customerOrders, 11, 27);
        ValidateJoin(customerOrders, 12, 28);
        ValidateJoin(customerOrders, 13, 29);
        ValidateJoin(customerOrders, 14, 30);
        ValidateJoin(customerOrders, 15, 31);
        ValidateJoin(customerOrders, 16, 32);
        ValidateJoin(customerOrders, 17, 33);
        ValidateJoin(customerOrders, 18, 34);
        ValidateJoin(customerOrders, 19, 35);
        ValidateJoin(customerOrders, 20, 36);
    }

    [Fact]
    public async Task LeftJoin()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new LeftJoin<Order>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = CustomerSqlGenerator.Select(selectTags, join, null, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<CustomerOrder> customerOrders = await CommandsAsync.ExecuteReaderAsync<CustomerOrder>(query, null, unitTestConnection);

        Assert.Equal(41, customerOrders.Count());
        ValidateJoin(customerOrders, 1, 1);
        ValidateJoin(customerOrders, 1, 2);
        ValidateJoin(customerOrders, 1, 3);
        ValidateJoin(customerOrders, 1, 4);
        ValidateJoin(customerOrders, 1, 5);
        ValidateJoin(customerOrders, 2, 6);
        ValidateJoin(customerOrders, 2, 7);
        ValidateJoin(customerOrders, 2, 8);
        ValidateJoin(customerOrders, 2, 9);
        ValidateJoin(customerOrders, 3, 10);
        ValidateJoin(customerOrders, 3, 11);
        ValidateJoin(customerOrders, 3, 12);
        ValidateJoin(customerOrders, 4, 13);
        ValidateJoin(customerOrders, 4, 14);
        ValidateJoin(customerOrders, 4, 15);
        ValidateJoin(customerOrders, 5, 16);
        ValidateJoin(customerOrders, 5, 17);
        ValidateJoin(customerOrders, 6, 18);
        ValidateJoin(customerOrders, 6, 19);
        ValidateJoin(customerOrders, 7, 20);
        ValidateJoin(customerOrders, 7, 21);
        ValidateJoin(customerOrders, 8, 22);
        ValidateJoin(customerOrders, 8, 23);
        ValidateJoin(customerOrders, 9, 24);
        ValidateJoin(customerOrders, 9, 25);
        ValidateJoin(customerOrders, 10, 26);
        ValidateJoin(customerOrders, 11, 27);
        ValidateJoin(customerOrders, 12, 28);
        ValidateJoin(customerOrders, 13, 29);
        ValidateJoin(customerOrders, 14, 30);
        ValidateJoin(customerOrders, 15, 31);
        ValidateJoin(customerOrders, 16, 32);
        ValidateJoin(customerOrders, 17, 33);
        ValidateJoin(customerOrders, 18, 34);
        ValidateJoin(customerOrders, 19, 35);
        ValidateJoin(customerOrders, 20, 36);

        ValidateJoin(customerOrders, 21, null);
        ValidateJoin(customerOrders, 22, null);
        ValidateJoin(customerOrders, 23, null);
        ValidateJoin(customerOrders, 24, null);
        ValidateJoin(customerOrders, 25, null);
    }

    [Fact]
    public async Task RightJoin()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new RightJoin<Customer>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = OrderSqlGenerator.Select(selectTags, join, null, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<CustomerOrder> customerOrders = await CommandsAsync.ExecuteReaderAsync<CustomerOrder>(query, null, unitTestConnection);

        Assert.Equal(41, customerOrders.Count());
        ValidateJoin(customerOrders, 1, 1);
        ValidateJoin(customerOrders, 1, 2);
        ValidateJoin(customerOrders, 1, 3);
        ValidateJoin(customerOrders, 1, 4);
        ValidateJoin(customerOrders, 1, 5);
        ValidateJoin(customerOrders, 2, 6);
        ValidateJoin(customerOrders, 2, 7);
        ValidateJoin(customerOrders, 2, 8);
        ValidateJoin(customerOrders, 2, 9);
        ValidateJoin(customerOrders, 3, 10);
        ValidateJoin(customerOrders, 3, 11);
        ValidateJoin(customerOrders, 3, 12);
        ValidateJoin(customerOrders, 4, 13);
        ValidateJoin(customerOrders, 4, 14);
        ValidateJoin(customerOrders, 4, 15);
        ValidateJoin(customerOrders, 5, 16);
        ValidateJoin(customerOrders, 5, 17);
        ValidateJoin(customerOrders, 6, 18);
        ValidateJoin(customerOrders, 6, 19);
        ValidateJoin(customerOrders, 7, 20);
        ValidateJoin(customerOrders, 7, 21);
        ValidateJoin(customerOrders, 8, 22);
        ValidateJoin(customerOrders, 8, 23);
        ValidateJoin(customerOrders, 9, 24);
        ValidateJoin(customerOrders, 9, 25);
        ValidateJoin(customerOrders, 10, 26);
        ValidateJoin(customerOrders, 11, 27);
        ValidateJoin(customerOrders, 12, 28);
        ValidateJoin(customerOrders, 13, 29);
        ValidateJoin(customerOrders, 14, 30);
        ValidateJoin(customerOrders, 15, 31);
        ValidateJoin(customerOrders, 16, 32);
        ValidateJoin(customerOrders, 17, 33);
        ValidateJoin(customerOrders, 18, 34);
        ValidateJoin(customerOrders, 19, 35);
        ValidateJoin(customerOrders, 20, 36);

        ValidateJoin(customerOrders, 21, null);
        ValidateJoin(customerOrders, 22, null);
        ValidateJoin(customerOrders, 23, null);
        ValidateJoin(customerOrders, 24, null);
        ValidateJoin(customerOrders, 25, null);
    }

    private static void ValidateJoin(IEnumerable<CustomerOrder> actualRecords, int? expectedCustomerId, int? expectedOrderId)
    {
        CustomerOrder actual = actualRecords
            .Where(record => (record.CustomerId == expectedCustomerId && record.OrderId == expectedOrderId))
            .Single();
        Customer? expectedCustomer = null;
        Order? expectedOrder = null;
        if(expectedCustomerId is not null)
        {
            expectedCustomer = CustomerDataSet
                .Data
                .Where(customer => customer.Id == expectedCustomerId)
                .Single();
        }
        
        if(expectedOrderId is not null)
        {
            expectedOrder = OrderDataSet
                .Data
                .Where(order => order.Id == expectedOrderId)
                .Single();
        }
        ValidateCustomer(actual, expectedCustomer);
        ValidateOrder(actual, expectedOrder);
    }
    private static void ValidateCustomer(CustomerOrder actual, Customer? expected)
    {
        if (expected is null)
        {
            Assert.Null(actual.CustomerId);
            Assert.Null(actual.FirstName);
            Assert.Null(actual.LastName);
            Assert.Null(actual.Age);
            Assert.Null(actual.Gender);
        }
        else
        {
            Assert.Equal(actual.CustomerId, expected.Id);
            Assert.Equal(actual.FirstName, expected.FirstName);
            Assert.Equal(actual.LastName, expected.LastName);
            Assert.Equal(actual.Age, expected.Age);
            Assert.Equal(actual.Gender, expected.Gender);
        }
    }
    private static void ValidateOrder(CustomerOrder actual, Order? expected)
    {
        if (expected is null)
        {
            Assert.Null(actual.OrderId);
            Assert.Null(actual.OrderCustomerId);
            Assert.Null(actual.AddressId);
            Assert.Null(actual.Date); 
            Assert.Null(actual.SalesTaxPercent);
        }
        else
        {
            Assert.Equal(actual.OrderId, expected.Id);
            Assert.Equal(actual.OrderCustomerId, expected.CustomerId);
            Assert.Equal(actual.AddressId, expected.AddressId);
            Assert.Equal(actual.Date, expected.Date);
            Assert.Equal(actual.SalesTaxPercent, expected.SalesTaxPercent);
        }
    }
}
