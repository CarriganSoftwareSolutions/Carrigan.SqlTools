using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;


namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class SqlGeneratorSelectExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectAll()
    {
        SqlQuery query = customerGenerator.SelectAll();

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer"
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectAllWithOrderBy()
    {
        OrderBy<Customer> orderBy = new(nameof(Customer.Email));
        SqlQuery query = customerGenerator.SelectAll(orderBy);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" ORDER BY "Customer"."Email" ASC
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);
        Joins<Customer> joins = new(join);

        SqlQuery query = customerGenerator.Select(null, null, null, joins, null, null, null, null);

        string expectedQueryText =
            """
        SELECT "Customer".* FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId")
        """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);
        Joins<Customer> joins = new(join);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SqlQuery query = customerGenerator.Select(null, null, null, joins, null, null, orderByOrderDate, null);

        string expectedQueryText =
            """
        SELECT "Customer".* FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId") ORDER BY "Order"."OrderDate" ASC
        """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithJoinsWhereAndOrderBy()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Column<Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);
        Joins<Customer> joins = new(join);

        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new(500m, "Total");
        GreaterThan greaterThan = new(totalCol, minTotal);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SqlQuery query = customerGenerator.Select(null, null, null, joins, greaterThan, null, orderByOrderDate, null);

        string expectedQueryText =
            """
        SELECT "Customer".* FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId") WHERE ("Order"."Total" > $1) ORDER BY "Order"."OrderDate" ASC
        """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 500m);
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
}