using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class JoinExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithInnerJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinsBase join = Joins<Customer>.InnerJoin<Order>(predicate);

        SqlQuery query = customerGenerator.Select(null, join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }
    [Fact]
    public void SelectWithFullJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinsBase join = Joins<Customer>.FullJoin<Order>(predicate);

        SqlQuery query = customerGenerator.Select(null, join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] FULL JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinsBase join = Joins<Customer>.Join<Order>(predicate);

        SqlQuery query = customerGenerator.Select(null, join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithLeftJoin()
    {
        //Note: ColumnEqualsColumn<lefT, rightT> validates the names of the properties, and throws an error if the property isn't valid
        ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
        JoinsBase join = Joins<Customer>.LeftJoin<Order>(predicate);

        SqlQuery query = customerGenerator.Select(null, join, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] LEFT JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
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

        SqlQuery query = customerGenerator.Select(null, joins, null, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) INNER JOIN [PaymentMethod] ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithSelectsOnTheJoin()
    {
        //Note: ColumnEqualsColumn<Customer, Order> validates the names of the properties, and throws an error if the property isn't valid
        SelectTags selectTags =
            SelectTags.Get<Customer>("Id", "CustomerId")
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

        SqlQuery query = customerGenerator.Select(selectTags, joins, null, null, null);

        Assert.Equal("SELECT [Customer].[Id] AS CustomerId, [Customer].[Name], [Customer].[Email], [Customer].[Phone], [Order].[Id] AS OrderId, [Order].[OrderDate], [Order].[Total], [PaymentMethod].[Id] AS PaymentMethodId, [PaymentMethod].[ZipCode] FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) INNER JOIN [PaymentMethod] ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }
}
