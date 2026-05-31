//Ignore Spelling: SqlTools, Localdb, Respawn, Respawner, Carrigan, SqlServer

using Carrigan.SqlTools.IntegrationTests.DataSets;
using Carrigan.SqlTools.IntegrationTests.Models;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
using Microsoft.Data.SqlClient;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Tests;

public sealed class UpdateTests : IClassFixture<UpdatesFixture>
{
    private readonly UpdatesFixture _fixture;

    private readonly SqlGenerator<Customer> CustomerSqlGenerator = new();

    public UpdateTests(UpdatesFixture fixture) =>
        _fixture = fixture;

    #region get no tests
    private async Task<IEnumerable<Customer>> GetAllCustomersNoTest()
    {
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        //Validate DB initial state
        SelectBuilder<Customer> selectAllBuilder = new() { };
        return await CommandsAsync.ExecuteReaderAsync<Customer>(selectAllBuilder, null, connection);
    }

    private async Task<IEnumerable<Customer>> GetCustomerByIdNoTest(int id)
    {
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        SqlQuery sqlQuery = CustomerSqlGenerator.SelectById(new Customer() { Id = id });
        return await CommandsAsync.ExecuteReaderAsync<Customer>(sqlQuery, null, connection);
    }

    private async Task<IEnumerable<Customer>> GetCustomerByNotIdNoTest(int id)
    {
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        Predicates where = new NotEqual(new Column<Customer>(nameof(Customer.Id)), new Parameter(id, "Id"));
        SqlQuery sqlQuery = CustomerSqlGenerator.Select(new SelectBuilder<Customer>() { Where = where });
        return await CommandsAsync.ExecuteReaderAsync<Customer>(sqlQuery, null, connection);
    }

    private async Task<IEnumerable<Customer>> GetAllFemaleCustomersNoTest()
    {
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        Predicates where = new ColumnValue<Customer>(nameof(Customer.Gender), 'F');
        //Validate DB initial state
        SelectBuilder<Customer> selectAllBuilder = new() { Where = where };
        return await CommandsAsync.ExecuteReaderAsync<Customer>(selectAllBuilder, null, connection);
    }

    private async Task<IEnumerable<Customer>> GetAllMaleCustomersNoTest()
    {
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        Predicates where = new ColumnValue<Customer>(nameof(Customer.Gender), 'M');
        //Validate DB initial state
        SelectBuilder<Customer> selectAllBuilder = new() { Where = where };
        return await CommandsAsync.ExecuteReaderAsync<Customer>(selectAllBuilder, null, connection);
    }
    #endregion

    #region get pretest
    private async Task<IEnumerable<Customer>> GetAllCustomersPretest()
    {
        IEnumerable<Customer> allCustomers = await GetAllCustomersNoTest();
        Assert.Equal(25, allCustomers.Count());
        for (int i = 0; i < 25;)
            CustomerDataSet.ValidateById(allCustomers, ++i);

        return allCustomers;
    }

    private async Task<Customer> GetCustomerByIdPretest(int id)
    {
        Customer customer = (await GetCustomerByIdNoTest(id)).Single();
        //Validate data before using in a test. If the data is invalid, then the test is invalid.
        CustomerDataSet.Validate(customer, customer.Id);

        return customer;
    }

    private async Task<IEnumerable<Customer>> GetAllFemaleCustomersPretest()
    {
        IEnumerable<Customer> customers = await GetAllFemaleCustomersNoTest();

        //Validate data before using in a test. If the data is invalid, then the test is invalid.
        Assert.Equal(12, customers.Count());
        foreach (Customer customer in customers)
            CustomerDataSet.Validate(customer, customer.Id);

        return customers;
    }
    #endregion

    #region get post tests
    private async Task<IEnumerable<Customer>> GetAllFemaleCustomersPostTest()
    {
        IEnumerable<Customer> femaleCustomers = await GetAllFemaleCustomersNoTest();

        //Validate data before using in a test. If the data is invalid, then the test is invalid.
        Assert.Equal(12, femaleCustomers.Count());
        foreach (Customer customer in femaleCustomers)
        {
            Assert.Equal('F', customer.Gender);
            ValidateRename(customer, customer.Id);
        }

        return femaleCustomers;
    }

    private async Task<IEnumerable<Customer>> GetAllMaleCustomersPostTest()
    {
        IEnumerable<Customer> customers = await GetAllMaleCustomersNoTest();

        //Validate data before using in a test. If the data is invalid, then the test is invalid.
        Assert.Equal(13, customers.Count());
        foreach (Customer customer in customers)
            CustomerDataSet.Validate(customer, customer.Id);

        return customers;
    }

    private async Task<Customer> GetCustomerByIdPostTest(int id)
    {
        Customer customer = (await GetCustomerByIdNoTest(id)).Single();
        ValidateRename(customer, customer.Id);

        return customer;
    }

    private async Task<IEnumerable<Customer>> GetCustomerByNotIdPostTest(int id)
    {
        IEnumerable<Customer> customers = await GetCustomerByNotIdNoTest(id);

        //Validate data before using in a test. If the data is invalid, then the test is invalid.
        Assert.Equal(24, customers.Count());
        foreach (Customer customer in customers)
            CustomerDataSet.Validate(customer, customer.Id);

        return customers;
    }
    #endregion

    [Fact]
    public async Task UpdateSingle()
    {
        //reset
        await _fixture.ResetAsync();

        //validate / get targets
        _ = await GetAllCustomersPretest();
        Customer customer = await GetCustomerByIdPretest(2);

        //update targets
        customer.FirstName = "Jane";
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        SqlQuery updateQuery = CustomerSqlGenerator.UpdateById(customer);
        int count = await CommandsAsync.ExecuteNonQueryAsync(updateQuery, null, connection);
        Assert.Equal(1, count);

        //post update unit tests
        _ = await GetCustomerByIdPostTest(2);
        _ = await GetCustomerByNotIdPostTest(2);
    }
    [Fact]
    public async Task UpdateMany()
    {
        await _fixture.ResetAsync();

        _ = await GetAllCustomersPretest();
        IEnumerable<Customer> femaleCustomers = await GetAllFemaleCustomersPretest();

        Customer values = new() { FirstName = "Jane", LastName = "Doe" };
        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        SqlQuery updateQuery = CustomerSqlGenerator.UpdateByIds(values, new ColumnCollection<Customer>(nameof(Customer.FirstName)), femaleCustomers);
        int count  = await CommandsAsync.ExecuteNonQueryAsync(updateQuery, null, connection);
        Assert.Equal(12, count);

        _ = GetAllMaleCustomersPostTest();
        _ = GetAllFemaleCustomersPostTest();
    }

    [Fact]
    public async Task UpdateWhere()
    {
        await _fixture.ResetAsync();
        _ = GetAllCustomersPretest();

        await using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        Customer values = new() { FirstName = "Jane" };
        UpdateBuilder<Customer> updateBuilder = new()
        {
            Values = values,
            UpdateColumns = new ColumnCollection<Customer>(nameof(Customer.FirstName)),
            Where = new ColumnValue<Customer>(nameof(Customer.Gender), "F")
        };
        int count = await CommandsAsync.ExecuteNonQueryAsync(updateBuilder, null, connection);
        Assert.Equal(12, count);

        _ = GetAllMaleCustomersPostTest();
        _ = GetAllFemaleCustomersPostTest();
    }

    [Fact]
    public async Task JoinOnSubquery()
    {
        await _fixture.ResetAsync();

        _ = await GetAllCustomersPretest();        
        
        using SqlConnection connection = new(_fixture.UnitTestConnectionString);
        ColumnEqualsColumn<Customer, Order> columnEqualsColumn = new (nameof(Customer.Id), nameof(Order.CustomerId));
        ColumnValueBase<Customer> customerThe = new ColumnValue<Customer>(nameof(Customer.Id), 3);

        Join<Order> joinOrderOn = new(columnEqualsColumn);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = joinOrderOn,
            Where = customerThe
        };
        IEnumerable<Customer> targetRecords = await CommandsAsync.ExecuteReaderAsync<Customer>(selectBuilder, null, connection);
        Assert.Equal(3, targetRecords.Count());
        CustomerDataSet.ValidateByIndexRange(targetRecords, 0, 2, 3);

        Customer values = new() { FirstName = "Jane" };
        UpdateBuilder<Customer> updateBuilder = new()
        {
            Values = values,
            UpdateColumns = new ColumnCollection<Customer>(nameof(Customer.FirstName)),
            Where = new ColumnValue<Order>(nameof(Order.CustomerId), 3),
            Joins = joinOrderOn
        };

        int count = await CommandsAsync.ExecuteNonQueryAsync(updateBuilder, null, connection);
        //int count = (int)(await CommandsAsync.ExecuteScalarAsync(updateQuery, null, connection) ?? throw new NullReferenceException());
        Assert.Equal(1, count);

        //post update unit tests
        _ = await GetCustomerByIdPostTest(3);
        _ = await GetCustomerByNotIdPostTest(3);

    }
    static void ValidateRename(Customer actual, int? expectedId)
    {
        //obligatory null handling
        Assert.NotNull(expectedId); //technically, we could get a null, but not if everything goes according to plan

        Customer expected = CustomerDataSet.Data.Where(customer => customer.Id == expectedId).Single();

        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal("Jane", actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
        Assert.Equal(expected.Age, actual.Age);
        Assert.Equal(expected.Gender, actual.Gender);
    }
}