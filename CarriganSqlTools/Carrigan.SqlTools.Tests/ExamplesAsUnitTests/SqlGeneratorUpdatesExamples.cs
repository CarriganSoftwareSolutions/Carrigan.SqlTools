using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Tests.Helpers;
using Carrigan.SqlTools.Generators.SqlServer;

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class SqlGeneratorUpdatesExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void UpdateById()
    {
        Customer entity = new()
        {
            Id = 42,
            Name = "Hank",
            Email = "Hank@example.com",
            Phone = "+1(555)555-5555"
        };
        SqlQuery query = customerGenerator.UpdateById(entity);

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name_1, [Email] = @Email_2, [Phone] = @Phone_3 WHERE [Id] = @Id_4;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 4);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id_4", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_3", "+1(555)555-5555");
    }

    [Fact]
    public void UpdateByIdSelectColumns()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnCollection<Customer> columns = new(nameof(Customer.Email));
        Customer entity = new()
        {
            Id = 42,
            Name = "Hank",
            Email = "Hank@example.gov"
        };
        SqlQuery query = customerGenerator.UpdateById(entity, columns);

        Assert.Equal("UPDATE [Customer] SET [Email] = @Email_1 WHERE [Id] = @Id_2;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id_2", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "Hank@example.gov");
    }

    [Fact]
    public void UpdateByIds()
    {
        Customer updateValues = new()
        {
            Name = "John Doe",
            Email = string.Empty
        };

        IEnumerable<Customer> customerIds =
            [
                new() { Id = 42 },
                new() { Id = 732 }
            ];

        ColumnCollection<Customer> updateColumns = new(nameof(Customer.Name), nameof(Customer.Email));

        SqlQuery query = customerGenerator.UpdateByIds(updateValues, updateColumns, customerIds);

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name_1, [Email] = @Email_2 WHERE (([Customer].[Id] = @Id_3) OR ([Customer].[Id] = @Id_4))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 4);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "John Doe");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", string.Empty);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_3", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Id_4", 732);
    }

    [Fact]
    public void UpdateWithSetColumnJoinsAndWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new()
        {
            Id = 10,
            Total = 123.45m
        };

        ColumnCollection<Order> columnCollection = new(nameof(Order.Total));

        Column<Customer> customerId = new(nameof(Customer.Id));
        Column<Order> orderCustomerId = new(nameof(Order.CustomerId));
        Equal customerIdsEquals = new(orderCustomerId, customerId);
        InnerJoin<Customer> joinOnCustomerId = new(customerIdsEquals);

        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Update(entity, columnCollection, joinOnCustomerId, customerEmailEquals);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @Total_1 FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 123.45m);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "spam@example.com");
    }

    [Fact]
    public void UpdateWithSetColumnWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid

        Customer entity = new()
        {
            Email = "spam@example.com"
        };
        ColumnCollection<Customer> columnCollection = new(nameof(Customer.Email));
        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");

        SqlQuery query = customerGenerator.Update(entity, columnCollection, null, customerEmailEquals);

        Assert.Equal("UPDATE [Customer] SET [Email] = @Email_1 WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "spam@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Hank@example.com");
    }
}