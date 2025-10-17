using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class SqlGeneratorSelectExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectAll()
    {
        SqlQuery query = customerGenerator.SelectAll();

        Assert.Equal("SELECT [Customer].* FROM [Customer]", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectAllWithOrderBy()
    {
        OrderByItem<Customer> orderByItem = new(nameof(Customer.Email));
        SqlQuery query = customerGenerator.SelectAll(orderByItem);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Email] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        Joins<Customer> join = Joins<Customer>.InnerJoin<Order>(predicate);

        SqlQuery query = customerGenerator.Select(null, join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        Joins<Customer> join = Joins<Customer>.InnerJoin<Order>(predicate);

        OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SqlQuery query = customerGenerator.Select(null, join, null, orderByOrderDate, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithJoinsWhereAndOrderBy()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Columns<Order> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        Joins<Customer> join = Joins<Customer>.InnerJoin<Order>(predicate);

        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new("Total", 500m);
        GreaterThan greaterThan = new(totalCol, minTotal);

        OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SqlQuery query = customerGenerator.Select(null, join, greaterThan, orderByOrderDate, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Order].[Total] > @Parameter_Total) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    }

    [Fact]
    public void SelectWithOffsetNext()
    {
        OffsetNext offsetNext = new(50, 25);
        SqlQuery query = customerGenerator.Select(null, null, null, null, offsetNext);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Id] ASC OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectById()
    {
        Customer entity = new() { Id = 42 };
        SqlQuery query = customerGenerator.SelectById(entity);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Id] = @Parameter_Id)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(42, (int)query.Parameters.Single().Value);
    }
}
