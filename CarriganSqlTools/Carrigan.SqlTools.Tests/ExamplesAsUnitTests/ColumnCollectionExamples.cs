using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class ColumnCollectionExamples
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

        Assert.Equal("UPDATE [Customer] SET [Name] = @Name_1, [Email] = @Email_2, [Phone] = @Phone_3 WHERE [Id] = @Id_4;", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(4, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_4").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Name_1").Single().Value);
        Assert.Equal("Hank@tx.gov", (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "@Phone_3").Single().Value);
    }

    [Fact]
    public void UsesSetsColumns()
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
}