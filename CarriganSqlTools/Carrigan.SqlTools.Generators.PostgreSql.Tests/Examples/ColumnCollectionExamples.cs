using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;


namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class ColumnCollectionExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

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

        string expectedQueryText =
            """
            UPDATE "Customer" SET "Email" = $1 WHERE "Id" = $2;
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "$2", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank@example.gov");
    }

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

        string expectedQueryText =
            """
            UPDATE "Customer" SET "Name" = $1, "Email" = $2, "Phone" = $3 WHERE "Id" = $4;
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 4);



        SqlQueryTestHelper.AssertParameterValue(query, "$4", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Hank@tx.gov");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", "+1(555)555-5555");
    }
}