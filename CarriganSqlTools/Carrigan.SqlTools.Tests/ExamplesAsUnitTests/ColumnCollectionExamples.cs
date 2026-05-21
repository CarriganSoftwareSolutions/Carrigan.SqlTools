using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Tests.Helpers;
using Carrigan.SqlTools.Generators.SqlServer;


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
        SqlQueryTestHelper.AssertParameterCount(query, 4);



        SqlQueryTestHelper.AssertParameterValue(query, "@Id_4", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Hank@tx.gov");
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_3", "+1(555)555-5555");
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
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id_2", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "Hank@example.gov");
    }
}