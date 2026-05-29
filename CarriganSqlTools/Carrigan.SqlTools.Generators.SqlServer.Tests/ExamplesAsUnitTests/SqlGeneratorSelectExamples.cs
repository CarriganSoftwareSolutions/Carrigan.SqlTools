using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.SqlServer;


namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class SqlGeneratorSelectExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectAll()
    {
        SqlQuery query = customerGenerator.SelectAll();

        Assert.Equal("SELECT [Customer].* FROM [Customer]", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectAllWithOrderBy()
    {
        OrderBy<Customer> orderBy = new(nameof(Customer.Email));
        SqlQuery query = customerGenerator.SelectAll(orderBy);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Email] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new (predicate);

        SqlQuery query = customerGenerator.Select(null, null, null, join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
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

        SqlQuery query = customerGenerator.Select(null, null, null, join, null, orderByOrderDate, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
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

        SqlQuery query = customerGenerator.Select(null, null, null, join, greaterThan, orderByOrderDate, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Order].[Total] > @Total_1) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 500m);
    }

    [Fact]
    public void SelectWithOffsetNext()
    {
        OffsetFetchNext offsetNext = new(50, 25);
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, offsetNext);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Id] ASC OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectById()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.SelectById(entity);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Id] = @Id_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertSingleParameterValue(query, 42);
    }
}