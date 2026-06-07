using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: dbo

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class FromReadMeMoreComplexExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
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
    public void SelectWithTwoPartOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderBy<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Order> join = new(predicate);

        OrderBy<Order> orderByOrderDate = new(nameof(Order.OrderDate));
        OrderBy<Customer> orderByCustomerId = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBys orderBys = new(orderByCustomerId, orderByOrderDate);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Joins = join,
            OrderBys = orderBys
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" INNER JOIN "Order" ON ("Customer"."Id" = "Order"."CustomerId") ORDER BY "Customer"."Id" DESC, "Order"."OrderDate" ASC
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        DeleteBuilder<Order> deleteBuilder = new()
        {
            Usings = [TableTag.Get<Customer>()],
            Where = new And(predicate, customerEmail)
        };

        SqlQuery query = orderGenerator.Delete(deleteBuilder);

        string expectedQueryText =
            """
            DELETE FROM "Order" USING "Customer" WHERE (("Customer"."Id" = "Order"."CustomerId") AND ("Customer"."Email" = $1))
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "spam@example.com");
    }

    [Fact]
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
    public void UpdateWithJoinsAndWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new() { Id = 10, Total = 123.45m };

        ColumnCollection<Order> columnCollection = new(nameof(Order.Total));

        ColumnEqualsColumn<Order, Customer> predicate = new(nameof(Order.CustomerId), nameof(Customer.Id));
        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        UpdateBuilder<Order> updateBuilder = new()
        {
            Values = entity,
            UpdateColumns = columnCollection,
            From = [TableTag.Get<Customer>()],
            Where = new And(predicate, customerEmailEquals)
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
}