using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;
public class JoinExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithCrossJoin()
    {
        CrossJoin<Order> join = new();

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" CROSS JOIN "Order"
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithFullJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        FullJoin<Order> join = new(predicate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" FULL JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithInnerJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new (predicate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        Join<Order> join = new(predicate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithLeftJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        LeftJoin<Order> join = new(predicate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" LEFT JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithRightJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        RightJoin<Order> join = new(predicate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" RIGHT JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithTwoJoins()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join1 = new(predicate);

        ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
        InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);

        Joins<Customer> joins = new(join1, join2);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = joins
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId") INNER JOIN "PaymentMethod" ON ("Order"."PaymentMethodId" = "PaymentMethod"."Id")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithSelectsOnTheJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        SelectTags selectTags =
            SelectTagGenerator.Get<Customer>("Id", "CustomerId")
                .Concat<Customer>(["Name", "Email", "Phone"])
                .Append<Order>("Id", "OrderId")
                .Concat<Order>(["OrderDate", "Total"])
                .Append<PaymentMethod>("Id", "PaymentMethodId")
                .Append<PaymentMethod>("ZipCode");

        ColumnEqualsColumn<Customer, Order> customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join1 = new(customerIdEquals);

        ColumnEqualsColumn<Order, PaymentMethod> paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
        InnerJoin<PaymentMethod> join2 = new(paymentMethodIdEquals);

        Joins<Customer> joins = new(join1, join2);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selectTags,
            Joins = joins
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer"."Id" AS "CustomerId", "Customer"."Name", "Customer"."Email", "Customer"."Phone", "Order"."Id" AS "OrderId", "Order"."OrderDate", "Order"."Total", "PaymentMethod"."Id" AS "PaymentMethodId", "PaymentMethod"."ZipCode" FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId") INNER JOIN "PaymentMethod" ON ("Order"."PaymentMethodId" = "PaymentMethod"."Id")
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
