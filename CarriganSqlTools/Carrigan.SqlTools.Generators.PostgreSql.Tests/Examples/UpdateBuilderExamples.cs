using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class UpdateBuilderExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

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

        UpdateBuilder<Order> updateBuilder = new()
        {
            Values = entity,
            UpdateColumns = columnCollection,
            From = [TableTag.Get<Customer>()],
            Where = new And(customerIdsEquals, customerEmailEquals)
        };

        SqlQuery query = orderGenerator.Update(updateBuilder);


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

        UpdateBuilder<Customer> updateBuilder = new()
        {
            Values = entity,
            UpdateColumns = columnCollection,
            Where = customerEmailEquals
        };

        SqlQuery query = customerGenerator.Update(updateBuilder);

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
