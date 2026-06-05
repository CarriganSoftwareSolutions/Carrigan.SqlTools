using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class SqlGeneratorUpdatesExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

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

        SqlQueryTestHelper.AssertParameterValue(query, "$4", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Hank@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "$3", "+1(555)555-5555");
    }

    [Fact]
    public void UpdateByIdSelectColumns()
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
    public void UpdateByIds()
    {
        Customer updateValues = new()
        {
            Name = "John Doe",
            Email = string.Empty
        };

        IEnumerable<Customer> customerIds =
            [
                new() { Id = 42 },
                new() { Id = 732 }
            ];

        ColumnCollection<Customer> updateColumns = new(nameof(Customer.Name), nameof(Customer.Email));

        SqlQuery query = customerGenerator.UpdateByIds(updateValues, updateColumns, customerIds);

        string expectedQueryText =
            """
            UPDATE "Customer" SET "Name" = $1, "Email" = $2 WHERE (("Customer"."Id" = $3) OR ("Customer"."Id" = $4))
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 4);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", "John Doe");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", string.Empty);
        SqlQueryTestHelper.AssertParameterValue(query, "$3", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$4", 732);
    }

    [Fact]
    public void UpdateWithSetColumnJoinsAndWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Column<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new()
        {
            Id = 10,
            Total = 123.45m
        };

        ColumnCollection<Order> columnCollection = new(nameof(Order.Total));

        Column<Customer> customerId = new(nameof(Customer.Id));
        Column<Order> orderCustomerId = new(nameof(Order.CustomerId));
        Equal customerIdsEquals = new(orderCustomerId, customerId);

        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Update<Customer>
        (
            entity,
            columnCollection,
            [TableTag.Get<Customer>()],
            null,
            new And(customerIdsEquals, customerEmailEquals)
        );

        string expectedQueryText =
            """
        UPDATE "Order" SET "Total" = $1 FROM "Customer" WHERE (("Order"."CustomerId" = "Customer"."Id") AND ("Customer"."Email" = $2))
        """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", 123.45m);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "spam@example.com");
    }

    [Fact]
    public void UpdateWithSetColumnWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid

        Customer entity = new()
        {
            Email = "spam@example.com"
        };

        ColumnCollection<Customer> columnCollection = new(nameof(Customer.Email));
        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "Hank@example.com");

        SqlQuery query = customerGenerator.Update<Customer>
        (
            entity,
            columnCollection,
            null,
            null,
            customerEmailEquals
        );

        string expectedQueryText =
            """
        UPDATE "Customer" SET "Email" = $1 WHERE ("Customer"."Email" = $2)
        """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "$1", "spam@example.com");
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Hank@example.com");
    }
}