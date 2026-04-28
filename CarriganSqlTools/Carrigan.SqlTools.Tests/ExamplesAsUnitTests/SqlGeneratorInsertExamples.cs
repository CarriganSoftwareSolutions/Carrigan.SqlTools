using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class SqlGeneratorInsertExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new(new SqlServerDialect());

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

        string expectedQueryText = ModifyInsertQueryWithReturn("INSERT INTO [Customer] ([Name], [Email], [Phone])", "VALUES (@Name, @Email, @Phone);", "INT");

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
        SqlQuery query = customerGenerator.Insert(null, null, customers);

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
