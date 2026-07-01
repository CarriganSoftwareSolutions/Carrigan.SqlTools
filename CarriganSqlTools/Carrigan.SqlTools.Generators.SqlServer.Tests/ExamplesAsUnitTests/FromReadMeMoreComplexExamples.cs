using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

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

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
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

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Customer].[Id] DESC, [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValue<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Customer> join = new(predicate);

        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        DeleteBuilder<Order> deleteBuilder = new()
        {
            Joins = join,
            Where = customerEmail
        };

        SqlQuery query = orderGenerator.Delete(deleteBuilder);

        Assert.Equal("DELETE [Order] FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Email_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "spam@example.com");
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

        Assert.Equal("SELECT COUNT([Order].[Id]) FROM [Order] WHERE ([Order].[Total] > @Total_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 500m);
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

        InnerJoin<Customer> join = new(predicate);

        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        UpdateBuilder<Order> updateBuilder = new()
        {
            Values = entity,
            UpdateColumns = columnCollection,
            Joins = join,
            Where = customerEmailEquals
        };

        SqlQuery query = orderGenerator.Update(updateBuilder);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @Total_1 FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 123.45m);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "spam@example.com");
    }
}