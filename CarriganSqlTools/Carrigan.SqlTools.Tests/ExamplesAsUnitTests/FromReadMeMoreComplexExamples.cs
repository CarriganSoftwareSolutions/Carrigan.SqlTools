using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer, Order, PhoneModel, EmailModel and ProcedureExec defined.
using Carrigan.SqlTools.Tests.Helpers;

//IGNORE SPELLING: dbo

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;

public class FromReadMeMoreComplexExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void SelectWithJoinsAndOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        InnerJoin<Order> join = new(predicate);

        OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));

        SqlQuery query = customerGenerator.Select(null, null, join, null, orderByOrderDate, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithTwoPartOrderBy()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: OrderByItem<Order> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Order> join = new(predicate);

        OrderByItem<Order> orderByOrderDate = new(nameof(Order.OrderDate));
        OrderByItem<Customer> orderByCustomerId = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBy orderBy = new(orderByCustomerId, orderByOrderDate);

        SqlQuery query = customerGenerator.Select(null, null, join, null, orderBy, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) ORDER BY [Customer].[Id] DESC, [Order].[OrderDate] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void DeleteWithJoinAndWhere()
    {
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));

        InnerJoin<Customer> join = new(predicate);

        ColumnValue<Customer> customerEmail = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Delete(join, customerEmail);

<<<<<<< HEAD
        Assert.Equal("DELETE FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Email_1)", query.QueryText);
=======
        Assert.Equal("DELETE [Order] FROM [Order] INNER JOIN [Customer] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Customer].[Email] = @Parameter_Email)", query.QueryText);
>>>>>>> fix-joined-delete-syntax-issue
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_1", "spam@example.com");
    }

    [Fact]
    public void SelectCountWithWhere()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new("Total", 500m);
        GreaterThan greaterThan = new(totalCol, minTotal);

        SqlQuery query = orderGenerator.SelectCount(null, null, greaterThan);

        Assert.Equal("SELECT COUNT([Order].*) FROM [Order] WHERE ([Order].[Total] > @Total_1)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 500m);
    }

    [Fact]
    public void UpdateWithJoinsAndWhere()
    {
        //Note: ColumnCollection<T> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnEqualsColumn<LeftT, RightT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: ColumnValues<T> validates the names of the properties, and throws an error if the property isn't valid

        Order entity = new() { Id = 10, Total = 123.45m };

        ColumnCollection<Order> columnCollection = new(nameof(Order.Total));

        ColumnEqualsColumn<Order, Customer> predicate = new(nameof(Order.CustomerId), nameof(Customer.Id));

        InnerJoin<Customer> join = new(predicate);

        ColumnValue<Customer> customerEmailEquals = new(nameof(Customer.Email), "spam@example.com");

        SqlQuery query = orderGenerator.Update(entity, columnCollection, join, customerEmailEquals);


        Assert.Equal("UPDATE [Order] SET [Order].[Total] = @Total_1 FROM [Order] INNER JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);

        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 123.45m);
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "spam@example.com");
    }
}