using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;

//IGNORE SPELLING: dbo

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class FromReadMeGettingStarted
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();


    [Fact]
    public void SelectAllRows()
    {
        SqlQuery query = customerGenerator.SelectAll();

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer"
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
        //Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectById()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.SelectById(entity);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" WHERE ("Customer"."Id" = $1)
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertSingleParameterValue(query, 42);
    }

    [Fact]
    public void Insert()
    {
        Customer entity = new()
        {
            Id = 42,
            Name = "Hank",
            Email = "Hank@example.com",
            Phone = "+1(555)555-5555"
        };
        InsertBuilder<Customer> insertBuilder = new()
        {
            Records = [entity]
        };

        SqlQuery query = customerGenerator.Insert(insertBuilder);

        string expectedQueryText =
            """
            INSERT INTO "Customer" ("Id", "Name", "Email", "Phone") VALUES ($1, $2, $3, $4);
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 4);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "$4", "+1(555)555-5555");
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
    public void UpdateById()
    {
        Customer entity = new()
        {
            Id = 42,
            Name = "Hank",
            Email = "Hank@example.com",
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

        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", "+1(555)555-5555");
        SqlQueryTestHelper.AssertParameterValue(query, "$4", 42);
    }

    [Fact]
    public void UpdateByIdSelectColumns()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnCollection<Customer> columns = new(nameof(Customer.Email));
        Customer entity = new() { Id = 42, Name = "Hank", Email = "Hank@example.gov" };
        SqlQuery query = customerGenerator.UpdateById(entity, columns);

        string expectedQueryText =
            """
            UPDATE "Customer" SET "Email" = $1 WHERE "Id" = $2;
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank@example.gov");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", 42);
    }

    [Fact]
    public void Delete()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.Delete(entity);

        string expectedQueryText =
            """
            DELETE FROM "Customer" WHERE ("Customer"."Id" = $1)
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 42);
    }

    [Fact]
    public void DeleteById()
    {
        Customer[] entities = [new() { Id = 1 }, new() { Id = 2 }];
        SqlQuery query = customerGenerator.DeleteById(entities);

        string expectedQueryText =
            """
            DELETE FROM "Customer" WHERE (("Customer"."Id" = $1) OR ("Customer"."Id" = $2))
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", 2);
    }
}