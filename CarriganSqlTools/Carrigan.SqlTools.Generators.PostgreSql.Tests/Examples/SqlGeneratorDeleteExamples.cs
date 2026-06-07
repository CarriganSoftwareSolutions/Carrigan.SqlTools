using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class SqlGeneratorDeleteExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();


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
    public void DeleteAll()
    {
        SqlQuery query = customerGenerator.DeleteAll();

        string expectedQueryText =
            """
            DELETE FROM "Customer";
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteById()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.DeleteById(entity);

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
    public void DeleteNullNull()
    {
        SqlQuery query = customerGenerator.Delete<Customer>(null, null, null);

        string expectedQueryText =
            """
            DELETE FROM "Customer";
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithNullJoin()
    {
        ColumnValue<Customer> columnValue = new(nameof(Customer.Name), "Hank");

        SqlQuery query = customerGenerator.Delete<Customer>(null, null, columnValue);

        string expectedQueryText =
            """
            DELETE FROM "Customer" WHERE ("Customer"."Name" = $1)
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank");
    }

    [Fact]
    public void DeleteWithNullPredicate()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        SqlQuery query = orderGenerator.Delete<Customer>([TableTag.Get<Customer>()], null, predicate);

        string expectedQueryText =
            """
            DELETE FROM "Order" USING "Customer" WHERE ("Customer"."Id" = "Order"."CustomerId")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Delete<Customer>
        (
            [TableTag.Get<Customer>()],
            null,
            new And(predicate, customerEmail)
        );

        string expectedQueryText =
            """
            DELETE FROM "Order" USING "Customer" WHERE (("Customer"."Id" = "Order"."CustomerId") AND ("Customer"."Email" = $1))
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "spam@example.com");
    }
}