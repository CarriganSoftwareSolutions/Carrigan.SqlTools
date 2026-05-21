using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Generators.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.CompositeModels;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.DataSets;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;
using Carrigan.SqlTools.Tags;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests;

public sealed class JoinsTests : IClassFixture<JoinsFixture>
{
    private readonly JoinsFixture _fixture;

    private readonly SqlGenerator<Customer> CustomerSqlGenerator = new();
    private readonly SqlGenerator<Order> OrderSqlGenerator = new();
    private readonly SqlGenerator<Left> LeftSqlGenerator = new();

    public JoinsTests(JoinsFixture fixture) =>
        _fixture = fixture;


    [Fact]
    public async Task Join()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new Join<Order>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = CustomerSqlGenerator.Select(null, selectTags, join, null, null, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
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

    [Fact]
    public async Task InnerJoin()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new InnerJoin<Order>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = CustomerSqlGenerator.Select(null, selectTags, join, null, null, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
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

    [Fact]
    public async Task LeftJoin()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new LeftJoin<Order>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = CustomerSqlGenerator.Select(null, selectTags, join, null, null, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<CustomerOrder> customerOrders = await CommandsAsync.ExecuteReaderAsync<CustomerOrder>(query, null, unitTestConnection);

        Assert.Equal(41, customerOrders.Count());
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

        ValidateCustomerOrderJoin(customerOrders, 21, null);
        ValidateCustomerOrderJoin(customerOrders, 22, null);
        ValidateCustomerOrderJoin(customerOrders, 23, null);
        ValidateCustomerOrderJoin(customerOrders, 24, null);
        ValidateCustomerOrderJoin(customerOrders, 25, null);
    }

    [Fact]
    public async Task RightJoin()
    {
        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinBase join = new RightJoin<Customer>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<CustomerOrder>());
        SqlQuery query = OrderSqlGenerator.Select(null, selectTags, join, null, null, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<CustomerOrder> customerOrders = await CommandsAsync.ExecuteReaderAsync<CustomerOrder>(query, null, unitTestConnection);

        Assert.Equal(41, customerOrders.Count());
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

        ValidateCustomerOrderJoin(customerOrders, 21, null);
        ValidateCustomerOrderJoin(customerOrders, 22, null);
        ValidateCustomerOrderJoin(customerOrders, 23, null);
        ValidateCustomerOrderJoin(customerOrders, 24, null);
        ValidateCustomerOrderJoin(customerOrders, 25, null);
    }

    [Fact]
    public async Task FullJoin()
    {
        ColumnEqualsColumn<Left, Right> joinPredicate = new(nameof(Left.Id), nameof(Right.Id));
        JoinBase join = new FullJoin<Right>(joinPredicate);
        SelectTags selectTags = new(SelectTag.GetAll<LeftRight>());
        SqlQuery query = LeftSqlGenerator.Select(null, selectTags, join, null, null, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<LeftRight> leftRights = await CommandsAsync.ExecuteReaderAsync<LeftRight>(query, null, unitTestConnection);

        Assert.Equal(8, leftRights.Count());

        ValidateLeftRightJoin(leftRights, 1, null);
        ValidateLeftRightJoin(leftRights, 2, null);
        ValidateLeftRightJoin(leftRights, 3, null);
        ValidateLeftRightJoin(leftRights, 4, 4);
        ValidateLeftRightJoin(leftRights, 5, 5);
        ValidateLeftRightJoin(leftRights, null, 6);
        ValidateLeftRightJoin(leftRights, null, 7);
        ValidateLeftRightJoin(leftRights, null, 8);
    }

    [Fact]
    public async Task CrossJoin()
    {
        JoinBase join = new CrossJoin<Right>();
        SelectTags selectTags = new(SelectTag.GetAll<LeftRight>());
        SqlQuery query = LeftSqlGenerator.Select(null, selectTags, join, null, null, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
        IEnumerable<LeftRight> leftRights = await CommandsAsync.ExecuteReaderAsync<LeftRight>(query, null, unitTestConnection);

        Assert.Equal(25, leftRights.Count());

        ValidateLeftRightJoin(leftRights, 1, 4);
        ValidateLeftRightJoin(leftRights, 1, 5);
        ValidateLeftRightJoin(leftRights, 1, 6);
        ValidateLeftRightJoin(leftRights, 1, 7);
        ValidateLeftRightJoin(leftRights, 1, 8);

        ValidateLeftRightJoin(leftRights, 2, 4);
        ValidateLeftRightJoin(leftRights, 2, 5);
        ValidateLeftRightJoin(leftRights, 2, 6);
        ValidateLeftRightJoin(leftRights, 2, 7);
        ValidateLeftRightJoin(leftRights, 2, 8);

        ValidateLeftRightJoin(leftRights, 3, 4);
        ValidateLeftRightJoin(leftRights, 3, 5);
        ValidateLeftRightJoin(leftRights, 3, 6);
        ValidateLeftRightJoin(leftRights, 3, 7);
        ValidateLeftRightJoin(leftRights, 3, 8);

        ValidateLeftRightJoin(leftRights, 4, 4);
        ValidateLeftRightJoin(leftRights, 4, 5);
        ValidateLeftRightJoin(leftRights, 4, 6);
        ValidateLeftRightJoin(leftRights, 4, 7);
        ValidateLeftRightJoin(leftRights, 4, 8);

        ValidateLeftRightJoin(leftRights, 5, 4);
        ValidateLeftRightJoin(leftRights, 5, 5);
        ValidateLeftRightJoin(leftRights, 5, 6);
        ValidateLeftRightJoin(leftRights, 5, 7);
        ValidateLeftRightJoin(leftRights, 5, 8);
    }

    [Fact]
    public async Task ChainedJoins()
    {

        ColumnEqualsColumn<Customer, Order> customerOrderPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        ColumnEqualsColumn<Order, OrderedItem> orderOrderedItemPredicate = new(nameof(Order.Id), nameof(OrderedItem.OrderId));
        ColumnEqualsColumn<OrderedItem, Book> orderedItemBookPredicate = new(nameof(OrderedItem.BookId), nameof(Book.Id));
        Joins<Customer> joins = new Joins<Customer>(new Join<Order>(customerOrderPredicate))
            .Append(new Join<OrderedItem>(orderOrderedItemPredicate))
            .Append(new Join<Book>(orderedItemBookPredicate));
        SelectTags selectTags = new(SelectTag.GetAll<Book>());
        OrderByItem<Book> orderByItems = new (nameof(Book.Id));
        SqlQuery query = CustomerSqlGenerator.Select(null, selectTags, joins, null, orderByItems, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);

        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);
        Assert.Equal(54, books.Count());

        BookDataSet.ValidateByIndexRange(books, 0, 2, 1);
        BookDataSet.ValidateByIndexRange(books, 3, 6, 2);
        BookDataSet.ValidateByIndexRange(books, 7, 10, 3);
        BookDataSet.ValidateByIndexRange(books, 11, 17, 4);
        BookDataSet.ValidateByIndexRange(books, 18, 23, 5);
        BookDataSet.ValidateByIndexRange(books, 24, 30, 6);
        BookDataSet.ValidateByIndexRange(books, 31, 36, 7);
        BookDataSet.ValidateByIndexRange(books, 37, 42, 8);
        BookDataSet.ValidateByIndexRange(books, 43, 47, 9);
        BookDataSet.ValidateByIndexRange(books, 48, 53, 10);
    }

    [Fact]
    public async Task DistinctChainedJoins()
    {

        ColumnEqualsColumn<Customer, Order> customerOrderPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        ColumnEqualsColumn<Order, OrderedItem> orderOrderedItemPredicate = new(nameof(Order.Id), nameof(OrderedItem.OrderId));
        ColumnEqualsColumn<OrderedItem, Book> orderedItemBookPredicate = new(nameof(OrderedItem.BookId), nameof(Book.Id));
        Joins<Customer> joins = new Joins<Customer>(new Join<Order>(customerOrderPredicate))
            .Append(new Join<OrderedItem>(orderOrderedItemPredicate))
            .Append(new Join<Book>(orderedItemBookPredicate));
        SelectTags selectTags = new(SelectTag.GetAll<Book>());
        OrderByItem<Book> orderByItems = new(nameof(Book.Id));
        SqlQuery query = CustomerSqlGenerator.Select(true, selectTags, joins, null, orderByItems, null);
        await using NpgsqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);

        IEnumerable<Book> books = await CommandsAsync.ExecuteReaderAsync<Book>(query, null, unitTestConnection);
        Assert.Equal(10, books.Count());

        BookDataSet.ValidateById(books, 1);
        BookDataSet.ValidateById(books, 2);
        BookDataSet.ValidateById(books, 3);
        BookDataSet.ValidateById(books, 4);
        BookDataSet.ValidateById(books, 5);
        BookDataSet.ValidateById(books, 6);
        BookDataSet.ValidateById(books, 7);
        BookDataSet.ValidateById(books, 8);
        BookDataSet.ValidateById(books, 9);
        BookDataSet.ValidateById(books, 10);
    }

    private static void ValidateCustomerOrderJoin(IEnumerable<CustomerOrder> actualRecords, int? expectedCustomerId, int? expectedOrderId)
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

    private static void ValidateLeftRightJoin(IEnumerable<LeftRight> actualRecords, int? expectedLeftId, int? expectedRightId)
    {
        LeftRight actual = actualRecords
            .Where(record => record.LeftId == expectedLeftId && record.RightId == expectedRightId)
            .Single();

        Left? expectedLeft = null;
        Right? expectedRight = null;

        if (expectedLeftId is not null)
        {
            expectedLeft = LeftDataSet
                .Data
                .Where(left => left.Id == expectedLeftId)
                .Single();
        }

        if (expectedRightId is not null)
        {
            expectedRight = RightDataSet
                .Data
                .Where(right => right.Id == expectedRightId)
                .Single();
        }

        ValidateLeft(actual, expectedLeft);
        ValidateRight(actual, expectedRight);
    }
    private static void ValidateLeft(LeftRight actual, Left? expected)
    {
        if (expected is null)
        {
            Assert.Null(actual.LeftId);
            Assert.Null(actual.LeftWord);
        }
        else
        {
            Assert.Equal(expected.Id, actual.LeftId);
            Assert.Equal(expected.LeftWord, actual.LeftWord);
        }
    }
    private static void ValidateRight(LeftRight actual, Right? expected)
    {
        if (expected is null)
        {
            Assert.Null(actual.RightId);
            Assert.Null(actual.RightWord);
        }
        else
        {
            Assert.Equal(expected.Id, actual.RightId);
            Assert.Equal(expected.RightWord, actual.RightWord);
        }
    }
}
