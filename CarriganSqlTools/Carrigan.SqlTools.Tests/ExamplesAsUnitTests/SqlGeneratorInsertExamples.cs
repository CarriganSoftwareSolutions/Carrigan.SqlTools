using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


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

        string expectedQueryText = ModifyInsertQueryWithReturn("INSERT INTO [Customer] ([Name], [Email], [Phone])", "VALUES (@Name_1, @Email_2, @Phone_3);", "INT");

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Equal(3, query.Parameters.Count);

        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Name_1").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_2").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "@Phone_3").Single().Value);
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
        Assert.Equal(8, query.Parameters.Count);

        Assert.Equal(42, (int)query.Parameters.Where(param => param.Key == "@Id_1").Single().Value);
        Assert.Equal("Hank", (string)query.Parameters.Where(param => param.Key == "@Name_2").Single().Value);
        Assert.Equal("Hank@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_3").Single().Value);
        Assert.Equal("+1(555)555-5555", (string)query.Parameters.Where(param => param.Key == "@Phone_4").Single().Value);

        Assert.Equal(732, (int)query.Parameters.Where(param => param.Key == "@Id_5").Single().Value);
        Assert.Equal("Homer", (string)query.Parameters.Where(param => param.Key == "@Name_6").Single().Value);
        Assert.Equal("Homer@example.com", (string)query.Parameters.Where(param => param.Key == "@Email_7").Single().Value);
        Assert.Equal("+1(555)555-1234", (string)query.Parameters.Where(param => param.Key == "@Phone_8").Single().Value);
    }
}