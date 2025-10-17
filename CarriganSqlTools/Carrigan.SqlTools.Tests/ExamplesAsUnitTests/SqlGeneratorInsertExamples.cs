using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class SqlGeneratorInsertExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    private static string ModifyInsertQueryToReturnScalar(string queryText)
    {
        // This method mirrors the logic used to transform an insert query to return the id.
        // Build the final query using a temporary table to store the GUID
        StringBuilder sqlQuery = new();
        sqlQuery.AppendLine("DECLARE @OutputTable TABLE (InsertedId UNIQUEIDENTIFIER);");
        sqlQuery.AppendLine(queryText.Replace("VALUES", "OUTPUT INSERTED.Id INTO @OutputTable VALUES"));
        sqlQuery.AppendLine("SELECT InsertedId FROM @OutputTable;");
        return sqlQuery.ToString();
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

        string expectedQueryText = ModifyInsertQueryToReturnScalar("INSERT INTO [Customer] ([Name], [Email], [Phone]) VALUES (@Name, @Email, @Phone);");

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(3, query.Parameters.Count);

        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "Name").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "Email").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone").Single().Value);
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
        SqlQuery query = customerGenerator.Insert(customers);

        Assert.Equal("INSERT INTO [Customer] ([Id], [Name], [Email], [Phone]) VALUES (@Id_0, @Name_0, @Email_0, @Phone_0), (@Id_1, @Name_1, @Email_1, @Phone_1);", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(8, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "Id_0").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "Name_0").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "Email_0").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "Phone_0").Single().Value);

        Assert.Equal(732, (int)query.Parameters.Where(param => param.Key == "Id_1").Single().Value);
        Assert.Equal("Homer", (string)query.Parameters.Where(param => param.Key == "Name_1").Single().Value);
        Assert.Equal("Homer@example.com", (string)query.Parameters.Where(param => param.Key == "Email_1").Single().Value);
        Assert.Equal("+1(555)555-1234", (string)query.Parameters.Where(param => param.Key == "Phone_1").Single().Value);
    }
}
