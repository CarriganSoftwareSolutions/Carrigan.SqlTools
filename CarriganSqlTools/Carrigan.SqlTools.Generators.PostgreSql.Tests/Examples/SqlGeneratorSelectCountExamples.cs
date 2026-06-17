using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class SqlGeneratorSelectCountExamples
{
    private static readonly SqlGenerator<Order> orderGenerator = new();
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SelectCountAll()
    {
        SqlQuery query = orderGenerator.SelectCount(null, null, null, null);

        string expectedQueryText =
            """
            SELECT COUNT("Order"."Id") FROM "Order"
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SelectCountWithWhere()
    {
        //Note: Column<T> validates the names of the properties, and throws an error if the property isn't valid
        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new(500m, "Total");
        GreaterThan greaterThan = new(totalCol, minTotal);

        SqlQuery query = orderGenerator.SelectCount(null, null, null, greaterThan);

        string expectedQueryText =
            """
            SELECT COUNT("Order"."Id") FROM "Order" WHERE ("Order"."Total" > $1)
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 500m);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SelectCountWithWhereAndJoin()
    {
        //Note: ColumnEqualsColumn<leftT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Column<T> validates the names of the properties, and throws an error if the property isn't valid
        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new(500m, "Total");
        GreaterThan greaterThan = new(totalCol, minTotal);

        ColumnEqualsColumn<Order, Customer> columnCompare = new(nameof(Order.CustomerId), nameof(Customer.Id));
        Join<Customer> join = new (columnCompare);

        SqlQuery query = orderGenerator.SelectCount(null, null, join, greaterThan);

        string expectedQueryText =
            """
            SELECT COUNT("Order"."Id") FROM "Order" JOIN "Customer" ON ("Order"."CustomerId" = "Customer"."Id") WHERE ("Order"."Total" > $1)
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 500m);
    }

    [Fact]
    [Obsolete("Use the Select Aggregate Count instead.")]
    public void SelectCountWithSelectsOnTheJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        SelectTagBase selectTag = SelectTagGenerator.Get<Customer>("Id", "CustomerId");

        ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join1 = new(customerIdEquals);

        ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
        InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);

        Joins<Customer> joins = new(join1, join2);

        SqlQuery query = customerGenerator.SelectCount(true, selectTag, joins, null);

        string expectedQueryText =
            """
            SELECT COUNT(DISTINCT "Customer"."Id") FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId") INNER JOIN "PaymentMethod" ON ("Order"."PaymentMethodId" = "PaymentMethod"."Id")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}