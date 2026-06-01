using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Tests;

public class DeleteTests : IClassFixture<DeleteFixture>
{
    private readonly DeleteFixture _fixture;

    private readonly SqlGenerator<Book> BookSqlGenerator = new();
    private readonly SqlGenerator<Customer> CustomerSqlGenerator = new();
    private readonly SqlGenerator<Order> OrderSqlGenerator = new();
    private readonly SqlGenerator<OrderedItem> OrderedItemSqlGenerator = new();
    private readonly SqlGenerator<Left> LeftSqlGenerator = new();

    public DeleteTests(DeleteFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task DeleteAll()
    {
        await _fixture.ResetAsync();

        SqlQuery readAllQuery = OrderedItemSqlGenerator.SelectAll();
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<OrderedItem> allOrderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(readAllQuery, null, connection);
        Assert.Equal(54, allOrderedItems.Count());
        OrderedItemDataSet.ValidateById(allOrderedItems, 1, 4);
        OrderedItemDataSet.ValidateById(allOrderedItems, 1, 5);
        OrderedItemDataSet.ValidateById(allOrderedItems, 1, 6);
        OrderedItemDataSet.ValidateById(allOrderedItems, 2, 7);
        OrderedItemDataSet.ValidateById(allOrderedItems, 2, 8);
        OrderedItemDataSet.ValidateById(allOrderedItems, 3, 10);
        OrderedItemDataSet.ValidateById(allOrderedItems, 4, 3);
        OrderedItemDataSet.ValidateById(allOrderedItems, 4, 4);
        OrderedItemDataSet.ValidateById(allOrderedItems, 4, 5);
        OrderedItemDataSet.ValidateById(allOrderedItems, 5, 6);
        OrderedItemDataSet.ValidateById(allOrderedItems, 5, 7);
        OrderedItemDataSet.ValidateById(allOrderedItems, 6, 9);
        OrderedItemDataSet.ValidateById(allOrderedItems, 7, 2);
        OrderedItemDataSet.ValidateById(allOrderedItems, 8, 5);
        OrderedItemDataSet.ValidateById(allOrderedItems, 8, 6);
        OrderedItemDataSet.ValidateById(allOrderedItems, 9, 8);
        OrderedItemDataSet.ValidateById(allOrderedItems, 9, 9);
        OrderedItemDataSet.ValidateById(allOrderedItems, 9, 10);
        OrderedItemDataSet.ValidateById(allOrderedItems, 10, 1);
        OrderedItemDataSet.ValidateById(allOrderedItems, 11, 4);
        OrderedItemDataSet.ValidateById(allOrderedItems, 11, 5);
        OrderedItemDataSet.ValidateById(allOrderedItems, 12, 7);
        OrderedItemDataSet.ValidateById(allOrderedItems, 13, 10);
        OrderedItemDataSet.ValidateById(allOrderedItems, 14, 3);
        OrderedItemDataSet.ValidateById(allOrderedItems, 14, 4);
        OrderedItemDataSet.ValidateById(allOrderedItems, 15, 6);
        OrderedItemDataSet.ValidateById(allOrderedItems, 15, 7);
        OrderedItemDataSet.ValidateById(allOrderedItems, 15, 8);
        OrderedItemDataSet.ValidateById(allOrderedItems, 16, 9);
        OrderedItemDataSet.ValidateById(allOrderedItems, 17, 2);
        OrderedItemDataSet.ValidateById(allOrderedItems, 18, 5);
        OrderedItemDataSet.ValidateById(allOrderedItems, 18, 6);
        OrderedItemDataSet.ValidateById(allOrderedItems, 19, 8);
        OrderedItemDataSet.ValidateById(allOrderedItems, 20, 1);
        OrderedItemDataSet.ValidateById(allOrderedItems, 21, 4);
        OrderedItemDataSet.ValidateById(allOrderedItems, 22, 7);
        OrderedItemDataSet.ValidateById(allOrderedItems, 22, 8);
        OrderedItemDataSet.ValidateById(allOrderedItems, 23, 10);
        OrderedItemDataSet.ValidateById(allOrderedItems, 24, 3);
        OrderedItemDataSet.ValidateById(allOrderedItems, 25, 6);
        OrderedItemDataSet.ValidateById(allOrderedItems, 26, 9);
        OrderedItemDataSet.ValidateById(allOrderedItems, 26, 10);
        OrderedItemDataSet.ValidateById(allOrderedItems, 27, 2);
        OrderedItemDataSet.ValidateById(allOrderedItems, 28, 5);
        OrderedItemDataSet.ValidateById(allOrderedItems, 29, 8);
        OrderedItemDataSet.ValidateById(allOrderedItems, 30, 1);
        OrderedItemDataSet.ValidateById(allOrderedItems, 30, 2);
        OrderedItemDataSet.ValidateById(allOrderedItems, 31, 4);
        OrderedItemDataSet.ValidateById(allOrderedItems, 32, 7);
        OrderedItemDataSet.ValidateById(allOrderedItems, 33, 10);
        OrderedItemDataSet.ValidateById(allOrderedItems, 34, 3);
        OrderedItemDataSet.ValidateById(allOrderedItems, 34, 4);
        OrderedItemDataSet.ValidateById(allOrderedItems, 35, 6);
        OrderedItemDataSet.ValidateById(allOrderedItems, 36, 9);



        SqlQuery deleteQuery = OrderedItemSqlGenerator.DeleteAll();
        await CommandsAsync.ExecuteNonQueryAsync(deleteQuery, null, connection);

        allOrderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(readAllQuery, null, connection);
        Assert.Empty(allOrderedItems);
    }

    [Fact]
    public async Task Delete()
    {
        await _fixture.ResetAsync();

        OrderedItem orderedItemId = new() { OrderId = 1, BookId = 4  };

        SqlQuery readAllQuery = OrderedItemSqlGenerator.SelectAll();
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<OrderedItem> orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(readAllQuery, null, connection);
        Assert.Equal(54, orderedItems.Count());
        OrderedItemDataSet.ValidateById(orderedItems, 1, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 3, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 6, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 7, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 10, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 12, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 13, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 16, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 17, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 19, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 20, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 21, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 23, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 24, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 25, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 27, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 28, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 29, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 31, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 32, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 33, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 35, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 36, 9);

        SqlQuery deleteQuery = OrderedItemSqlGenerator.Delete(orderedItemId);
        await CommandsAsync.ExecuteNonQueryAsync(deleteQuery, null, connection);

        SqlQuery selectById = OrderedItemSqlGenerator.SelectById(orderedItemId);
        orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(selectById, null, connection);
        Assert.Empty(orderedItems);

        orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(readAllQuery, null, connection);
        Assert.Equal(53, orderedItems.Count());
        OrderedItemDataSet.ValidateById(orderedItems, 1, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 3, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 6, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 7, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 10, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 12, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 13, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 16, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 17, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 19, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 20, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 21, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 23, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 24, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 25, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 27, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 28, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 29, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 31, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 32, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 33, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 35, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 36, 9);
    }
    [Fact]
    public async Task DeleteById()
    {
        await _fixture.ResetAsync();

        IEnumerable<OrderedItem> deleteById = 
            [
                new() { OrderId = 1, BookId = 4 },
                new() { OrderId = 1, BookId = 5 },
                new() { OrderId = 1, BookId = 6 },
                new() { OrderId = 2, BookId = 7 },
                new() { OrderId = 2, BookId = 8 },
                new() { OrderId = 3, BookId = 10 },
            ];

        SqlQuery selectAllQuery = OrderedItemSqlGenerator.SelectAll();
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<OrderedItem> orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(selectAllQuery, null, connection);
        Assert.Equal(54, orderedItems.Count());
        OrderedItemDataSet.ValidateById(orderedItems, 1, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 3, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 6, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 7, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 10, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 12, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 13, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 16, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 17, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 19, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 20, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 21, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 23, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 24, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 25, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 27, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 28, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 29, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 31, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 32, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 33, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 35, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 36, 9);

        SqlQuery deleteQuery = OrderedItemSqlGenerator.DeleteById(deleteById);
        await CommandsAsync.ExecuteNonQueryAsync(deleteQuery, null, connection);

        SqlQuery selectByIdQuery = OrderedItemSqlGenerator.SelectById(deleteById);
        orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(selectByIdQuery, null, connection);
        Assert.Empty(orderedItems);

        orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(selectAllQuery, null, connection);
        Assert.Equal(48, orderedItems.Count());
        OrderedItemDataSet.ValidateById(orderedItems, 4, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 6, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 7, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 10, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 12, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 13, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 16, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 17, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 19, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 20, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 21, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 23, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 24, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 25, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 27, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 28, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 29, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 31, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 32, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 33, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 35, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 36, 9);
    }

    [Fact]
    public async Task DeleteWithJoinAndPredicate()
    {
        await _fixture.ResetAsync();

        ColumnEqualsColumn<Order, OrderedItem> orderOrderedItemPredicate = new(nameof(Order.Id), nameof(OrderedItem.OrderId));
        ColumnEqualsColumn<Customer, Order> customerOrderPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        GreaterThan greaterThan = new
        (
            new Column<OrderedItem>(nameof(OrderedItem.Price)),
            new Parameter(13m, "Thirteen")
        );
        And and = new(greaterThan, orderOrderedItemPredicate);
        Join<Customer> joinCustomer = new(customerOrderPredicate);
        Joins<Order> joinsOnOrder = new(joinCustomer);

        SqlQuery selectAllQuery = OrderedItemSqlGenerator.SelectAll();
        await using NpgsqlConnection connection = new(_fixture.UnitTestConnectionString);

        IEnumerable<OrderedItem> orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(selectAllQuery, null, connection);
        Assert.Equal(54, orderedItems.Count());
        OrderedItemDataSet.ValidateById(orderedItems, 1, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 3, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 6, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 7, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 10, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 12, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 13, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 16, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 17, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 19, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 20, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 21, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 23, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 24, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 25, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 27, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 28, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 29, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 1);
        OrderedItemDataSet.ValidateById(orderedItems, 30, 2);
        OrderedItemDataSet.ValidateById(orderedItems, 31, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 32, 7);
        OrderedItemDataSet.ValidateById(orderedItems, 33, 10);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 35, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 36, 9);

        //SqlQuery deleteQuery = OrderedItemSqlGenerator.Delete(null, joinsOnOrder, and);
        SqlQuery deleteQuery = OrderedItemSqlGenerator.Delete(null, joinsOnOrder, and);
        await CommandsAsync.ExecuteNonQueryAsync(deleteQuery, null, connection);

        orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(selectAllQuery, null, connection);
        Assert.Equal(35, orderedItems.Count());
        OrderedItemDataSet.ValidateById(orderedItems, 1, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 1, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 2, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 4, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 5, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 6, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 8, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 9, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 11, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 14, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 15, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 16, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 18, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 19, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 21, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 22, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 24, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 25, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 26, 9);
        OrderedItemDataSet.ValidateById(orderedItems, 28, 5);
        OrderedItemDataSet.ValidateById(orderedItems, 29, 8);
        OrderedItemDataSet.ValidateById(orderedItems, 31, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 3);
        OrderedItemDataSet.ValidateById(orderedItems, 34, 4);
        OrderedItemDataSet.ValidateById(orderedItems, 35, 6);
        OrderedItemDataSet.ValidateById(orderedItems, 36, 9);

        IEnumerable<OrderedItem> deletedOrderedItems =
        [
            new() { OrderId = 2, BookId = 7 },
            new() { OrderId = 3, BookId = 10 },
            new() { OrderId = 5, BookId = 7 },
            new() { OrderId = 7, BookId = 2 },
            new() { OrderId = 9, BookId = 10 },
            new() { OrderId = 10, BookId = 1 },
            new() { OrderId = 12, BookId = 7 },
            new() { OrderId = 13, BookId = 10 },
            new() { OrderId = 15, BookId = 7 },
            new() { OrderId = 17, BookId = 2 },
            new() { OrderId = 20, BookId = 1 },
            new() { OrderId = 22, BookId = 7 },
            new() { OrderId = 23, BookId = 10 },
            new() { OrderId = 26, BookId = 10 },
            new() { OrderId = 27, BookId = 2 },
            new() { OrderId = 30, BookId = 1 },
            new() { OrderId = 30, BookId = 2 },
            new() { OrderId = 32, BookId = 7 },
            new() { OrderId = 33, BookId = 10 }
        ];
        SqlQuery selectByIdQuery = OrderedItemSqlGenerator.SelectById(deletedOrderedItems);
        orderedItems = await CommandsAsync.ExecuteReaderAsync<OrderedItem>(selectByIdQuery, null, connection);
        Assert.Empty(orderedItems);
    }
}
