using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using System.Text;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class InsertBuilderExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void Insert()
    {
        IEnumerable<Customer> customers =
            [
                new()
                {
                    Id = 42,
                    Name = "Hank",
                    Email = "Hank@example.com",
                    Phone = "+1(555)555-5555"
                },
                new()
                {
                    Id = 732,
                    Name = "Homer",
                    Email = "Homer@example.com",
                    Phone = "+1(555)555-1234"
                },
            ];
        InsertBuilder<Customer> insertBuilder = new()
        {
            Records = customers
        };

        SqlQuery query = customerGenerator.Insert(insertBuilder);

        Assert.Equal("INSERT INTO [Customer] ([Id], [Name], [Email], [Phone]) VALUES (@Id_1, @Name_2, @Email_3, @Phone_4), (@Id_5, @Name_6, @Email_7, @Phone_8);", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 8);

        SqlQueryTestHelper.AssertParameterValue(query, "@Id_1", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_3", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_4", "+1(555)555-5555");

        SqlQueryTestHelper.AssertParameterValue(query, "@Id_5", 732);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_6", "Homer");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_7", "Homer@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_8", "+1(555)555-1234");
    }
}
