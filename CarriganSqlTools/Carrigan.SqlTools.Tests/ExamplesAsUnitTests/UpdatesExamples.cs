using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class UpdatesExamples
{
    private SqlGenerator<Customer> customerGenerator = new();
    private SqlGenerator<Order> orderGenerator = new();


    [Fact]
    public void UpdateById()
    {
        Customer entity = new()
        {
            Id = 42,
            Name = "Hank Hill",
            Email = "Hank.Hill@example.com",
            Phone = "+1(555)555-5555"
        };
        SqlQuery query = customerGenerator.UpdateById(entity);

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank Hill", (string)query.Parameters.Where(param => param.Key == "Name").Single().Value);
        Assert.Equal("Hank.Hill@example.com", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone").Single().Value);
    }

    [Fact]
    public void UpdateByIdSelectColumns()
    {
        //Note: SetColumns<T> validates the names of the properties, and throws an error if the property isn't valid
        SetColumns<Customer> columns = new(nameof(Customer.Email));
        Customer entity = new() { Id = 42, Name = "Hank", Email = "Hank@example.gov" };
        SqlQuery query = customerGenerator.UpdateById(entity, columns);

        Assert.Equal("UPDATE [Customer] SET [Email] = @Email WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank@example.gov", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
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

        SetColumns<Customer> updateColumns = new(nameof(Customer.Name), nameof(Customer.Email));

        SqlQuery query = customerGenerator.UpdateByIds(updateValues, updateColumns, customerIds);

        Assert.Equal("UPDATE [Customer] SET [Customer].[Name] = @ParameterSet_Name, [Customer].[Email] = @ParameterSet_Email FROM [Customer] WHERE (([Customer].[Id] = @Parameter_0_R_Id) OR ([Customer].[Id] = @Parameter_1_R_Id))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Parameter_0_R_Id").Single().Value);
        Assert.Equal(732, (int)query.Parameters.Where(param => param.Key == "@Parameter_1_R_Id").Single().Value);
        Assert.Equal("John Doe", (string)query.Parameters.Where(param => param.Key == "@ParameterSet_Name").Single().Value);
        Assert.Equal(string.Empty, (string)query.Parameters.Where(param => param.Key == "@ParameterSet_Email").Single().Value);
    }

    [Fact]
    public void UpdateWithSetColumnJoinsAndWhere()
    {
        //Note: SetColumns<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ByColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new() { Id = 10, Total = 123.45m };

        SetColumns<Order> setColumns = new(nameof(Order.Total));

        Columns<Customer> customerId = new(nameof(Customer.Id));
        Columns<Order> orderCustomerId = new(nameof(Order.CustomerId));
        Equal customerIdsEquals = new(orderCustomerId, customerId);
        InnerJoin<Order, Customer> joinOnCustomerId = new(customerIdsEquals);

        ByColumnValues<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Update(entity, setColumns, joinOnCustomerId, customerEmailEquals);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @ParameterSet_Total FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(123.45m, (decimal)query.Parameters.Where(param => param.Key == "@ParameterSet_Total").Single().Value);
        Assert.Equal("spam@example.com", (string)query.Parameters.Where(param => param.Key == "@Parameter_Email").Single().Value);
    }

    [Fact]
    public void UpdateWithSetColumnWhere()
    {
        //Note: SetColumns<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ByColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid

        Customer entity = new() { Email = "spam@example.com" };
        SetColumns<Customer> setColumns = new(nameof(Customer.Email));
        ByColumnValues<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");

        SqlQuery query = customerGenerator.Update(entity, setColumns, null, customerEmailEquals);

        Assert.Equal("UPDATE [Customer] SET [Customer].[Email] = @ParameterSet_Email FROM [Customer] WHERE ([Customer].[Email] = @Parameter_Email)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Parameter_Email").Single().Value);
        Assert.Equal("spam@example.com", (string)query.Parameters.Where(param => param.Key == "@ParameterSet_Email").Single().Value);
    }
}
