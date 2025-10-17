using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class SetColumnsExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void NoSetsColumn()
    {
        Customer entity = new()
        {
            Id = 42,
            Name = "Hank",
            Email = "Hank@tx.gov",
            Phone = "+1(555)555-5555"
        };
        SqlQuery query = customerGenerator.UpdateById(entity);

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name, [Email] = @Email, [Phone] = @Phone WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "Name").Single().Value);
        Assert.Equal("Hank@tx.gov", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone").Single().Value);
    }

    [Fact]
    public void UsesSetsColumns()
    {
        //Note: SetColumns<T> validates the names of the properties, and throws an error if the property isn't valid
        SetColumns<Customer> columns = new(nameof(Customer.Email));
        Customer entity = new() 
        { 
            Id = 42, 
            Name = "Hank", 
            Email = "Hank@example.gov" 
        };
        SqlQuery query = customerGenerator.UpdateById(entity, columns);

        Assert.Equal("UPDATE [Customer] SET [Email] = @Email WHERE [Id] = @Id;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(2, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id").Single().Value);
        Assert.Equal("Hank@example.gov", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
    }
}
