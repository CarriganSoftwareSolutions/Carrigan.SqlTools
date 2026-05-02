using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.

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
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_4").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Name_1").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "@Phone_3").Single().Value);
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
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_2").Single().Value);
        Assert.Equal("Hank@example.gov", (string)query.Parameters.Where(param => param.Key == "@Email_1").Single().Value);
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

        Assert.Equal("UPDATE [Customer] SET [Customer].[Name] = @Name_1, [Customer].[Email] = @Email_2 FROM [Customer] WHERE (([Customer].[Id] = @Id_3) OR ([Customer].[Id] = @Id_4))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_3").Single().Value);
        Assert.Equal(732, (int)query.Parameters.Where(param => param.Key == "@Id_4").Single().Value);
        Assert.Equal("John Doe", (string)query.Parameters.Where(param => param.Key == "@Name_1").Single().Value);
        Assert.Equal(string.Empty, (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
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
        Joins<Order> joinOnCustomerId = Joins<Order>.InnerJoin<Customer>(customerIdsEquals);

        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Update(entity, columnCollection, joinOnCustomerId, customerEmailEquals);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @Total_1 FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(123.45m, (decimal)query.Parameters.Where(param => param.Key == "@Total_1").Single().Value);
        Assert.Equal("spam@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
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

        Assert.Equal("UPDATE [Customer] SET [Customer].[Email] = @Email_1 FROM [Customer] WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
        Assert.Equal("spam@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_1").Single().Value);
    }
}