//Ignore Spelling: Localdb, Respawn, Respawner

using Carrigan.SqlTools.Clients.SqlServer;
using Carrigan.SqlTools.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Carrigan.SqlTools.Tags;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class SubqueryTests : IClassFixture<SubqueryFixture>
{
    private readonly SubqueryFixture _fixture;

    private readonly SqlGenerator<Customer> CustomerSqlGenerator = new();
    private readonly SqlGenerator<Order> OrderSqlGenerator = new();

    public SubqueryTests(SubqueryFixture fixture) =>
        _fixture = fixture;


    [Fact]
    public async Task SelectFromSubquery()
    {
        SubqueryBuilder<Customer> subqueryBuilder = new() { };
        Subquery<Customer> subquery = CustomerSqlGenerator.Subquery(subqueryBuilder);
        SelectBuilder<Customer> select = new() { Subquery = subquery };

        SqlQuery query = CustomerSqlGenerator.Select(select);

        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<Customer> customers = await CommandsAsync.ExecuteReaderAsync<Customer>(query, null, unitTestConnection);

        Assert.Equal(25, customers.Count());
        for(int i = 0; i < 25;)
            CustomerDataSet.ValidateById(customers, ++i);
    }

    [Fact]
    public async Task JoinOnSubquery()
    {
        SubqueryBuilder<Order> subqueryBuilder = new() { };
        Subquery<Order> subquery = OrderSqlGenerator.Subquery(subqueryBuilder);
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new Join<Order>(joinPredicate, subquery);
        SelectTags selectTags = SelectTagGenerator.GetAll<CustomerOrder>();
        SqlQuery query = CustomerSqlGenerator.Select(null, null, selectTags, join, null, null, null, null);
        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<CustomerOrder> customerOrders = await CommandsAsync.ExecuteReaderAsync<CustomerOrder>(query, null, unitTestConnection);

        Assert.Equal(36, customerOrders.Count());
        ValidateCustomerOrderJoin(customerOrders, 1, 1);
        ValidateCustomerOrderJoin(customerOrders, 1, 2);
        ValidateCustomerOrderJoin(customerOrders, 1, 3);
        ValidateCustomerOrderJoin(customerOrders, 1, 4);
        ValidateCustomerOrderJoin(customerOrders, 1, 5);
        ValidateCustomerOrderJoin(customerOrders, 2, 6);
        ValidateCustomerOrderJoin(customerOrders, 2, 7);
        ValidateCustomerOrderJoin(customerOrders, 2, 8);
        ValidateCustomerOrderJoin(customerOrders, 2, 9);
        ValidateCustomerOrderJoin(customerOrders, 3, 10);
        ValidateCustomerOrderJoin(customerOrders, 3, 11);
        ValidateCustomerOrderJoin(customerOrders, 3, 12);
        ValidateCustomerOrderJoin(customerOrders, 4, 13);
        ValidateCustomerOrderJoin(customerOrders, 4, 14);
        ValidateCustomerOrderJoin(customerOrders, 4, 15);
        ValidateCustomerOrderJoin(customerOrders, 5, 16);
        ValidateCustomerOrderJoin(customerOrders, 5, 17);
        ValidateCustomerOrderJoin(customerOrders, 6, 18);
        ValidateCustomerOrderJoin(customerOrders, 6, 19);
        ValidateCustomerOrderJoin(customerOrders, 7, 20);
        ValidateCustomerOrderJoin(customerOrders, 7, 21);
        ValidateCustomerOrderJoin(customerOrders, 8, 22);
        ValidateCustomerOrderJoin(customerOrders, 8, 23);
        ValidateCustomerOrderJoin(customerOrders, 9, 24);
        ValidateCustomerOrderJoin(customerOrders, 9, 25);
        ValidateCustomerOrderJoin(customerOrders, 10, 26);
        ValidateCustomerOrderJoin(customerOrders, 11, 27);
        ValidateCustomerOrderJoin(customerOrders, 12, 28);
        ValidateCustomerOrderJoin(customerOrders, 13, 29);
        ValidateCustomerOrderJoin(customerOrders, 14, 30);
        ValidateCustomerOrderJoin(customerOrders, 15, 31);
        ValidateCustomerOrderJoin(customerOrders, 16, 32);
        ValidateCustomerOrderJoin(customerOrders, 17, 33);
        ValidateCustomerOrderJoin(customerOrders, 18, 34);
        ValidateCustomerOrderJoin(customerOrders, 19, 35);
        ValidateCustomerOrderJoin(customerOrders, 20, 36);
    }

    private static void ValidateCustomerOrderJoin(IEnumerable<CustomerOrder> actualRecords, int? expectedCustomerId, int? expectedOrderId)
    {
        CustomerOrder actual = actualRecords
            .Where(record => (record.CustomerId == expectedCustomerId && record.OrderId == expectedOrderId))
            .Single();
        Customer? expectedCustomer = null;
        Order? expectedOrder = null;
        if (expectedCustomerId is not null)
        {
            expectedCustomer = CustomerDataSet
                .Data
                .Where(customer => customer.Id == expectedCustomerId)
                .Single();
        }

        if (expectedOrderId is not null)
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