using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class SelectBuilderExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);

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
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join,
            OrderBys = orderByOrderDate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

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
        //Note: Columns<Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);

        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new(500m, "Total");
        GreaterThan greaterThan = new(totalCol, minTotal);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join,
            Where = greaterThan,
            OrderBys = orderByOrderDate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId") WHERE ("Order"."Total" > $1) ORDER BY "Order"."OrderDate" ASC
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 500m);
    }
}
