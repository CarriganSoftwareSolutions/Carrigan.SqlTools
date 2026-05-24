using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.SqlServer;


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class SqlGeneratorInsertExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    private static string ModifyInsertQueryWithReturn(string queryPart1, string queryPart2, string type)
    {
        StringBuilder queryBuilder = new();
        queryBuilder.AppendLine($"DECLARE @OutputTable TABLE (Id {type});");
        queryBuilder.AppendLine(queryPart1);
        queryBuilder.AppendLine("OUTPUT INSERTED.Id INTO @OutputTable");
        queryBuilder.AppendLine(queryPart2);
        queryBuilder.AppendLine("SELECT Id FROM @OutputTable;");
        return queryBuilder.ToString();
    }

    [Fact]
    public void InsertWithAutoId()
    {
        Customer entity = new()
        {
            Name = "Hank",
            Email = "Hank@example.com",
            Phone = "+1(555)555-5555"
        };
        SqlQuery query = customerGenerator.InsertAutoId(entity);

        string expectedQueryText = ModifyInsertQueryWithReturn("INSERT INTO [Customer] ([Name], [Email], [Phone])", "VALUES (@Name_1, @Email_2, @Phone_3);", "INT NOT NULL");

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 3);

        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "@Phone_3", "+1(555)555-5555");
    }

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
        SqlQuery query = customerGenerator.Insert(null, null, customers);

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