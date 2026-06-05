using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;


namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class SqlGeneratorInsertExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();


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

        string expectedQueryText =
            """
            INSERT INTO "Customer" ("Name", "Email", "Phone")
            VALUES ($1, $2, $3)
            RETURNING "Id";
            """.ReplaceLineEndings(Environment.NewLine) + Environment.NewLine;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 3);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", "+1(555)555-5555");
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

        string expectedQueryText =
            """
            INSERT INTO "Customer" ("Id", "Name", "Email", "Phone") VALUES ($1, $2, $3, $4), ($5, $6, $7, $8);
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 8);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "$4", "+1(555)555-5555");

        SqlQueryTestHelper.AssertParameterValue(query, "$5", 732);
        SqlQueryTestHelper.AssertParameterValue(query, "$6", "Homer");
        SqlQueryTestHelper.AssertParameterValue(query, "$7", "Homer@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "$8", "+1(555)555-1234");
    }
}