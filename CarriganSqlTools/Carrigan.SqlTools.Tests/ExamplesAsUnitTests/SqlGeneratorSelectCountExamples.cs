using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class SqlGeneratorSelectCountExamples
{
    private static readonly SqlGenerator<Order> orderGenerator = new();
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectCountAll()
    {
        SqlQuery query = orderGenerator.SelectCount(null, null, null);

        Assert.Equal("SELECT COUNT([Order].*) FROM [Order]", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectCountWithWhere()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new("Total", 500m);
        GreaterThan greaterThan = new(totalCol, minTotal);

        SqlQuery query = orderGenerator.SelectCount(null, null, greaterThan);

        Assert.Equal("SELECT COUNT([Order].*) FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    }

    [Fact]
    public void SelectCountWithWhereAndJoin()
    {
        //Note: ColumnEqualsColumn<leftT, righT> validates the names of the properties, and throws an error if the property isn't valid
        //Note: Column<T> validates the names of the properties, and throws an error if the property isn't valid
        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new("Total", 500m);
        GreaterThan greaterThan = new(totalCol, minTotal);

        ColumnEqualsColumn<Order, Customer> columnCompare = new(nameof(Order.CustomerId), nameof(Customer.Id));
        Joins<Order> joins = Joins<Order>.Join<Customer>(columnCompare);

        SqlQuery query = orderGenerator.SelectCount(null, joins, greaterThan);

        Assert.Equal("SELECT COUNT([Order].*) FROM [Order] JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    }

    [Fact]
    public void SelectCountWithSelectsOnTheJoin()
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

        SqlQuery query = customerGenerator.SelectCount(selectTags, joins, null);

        Assert.Equal("SELECT COUNT([Customer].[Id] AS CustomerId, [Customer].[Name], [Customer].[Email], [Customer].[Phone], [Order].[Id] AS OrderId, [Order].[OrderDate], [Order].[Total], [PaymentMethod].[Id] AS PaymentMethodId, [PaymentMethod].[ZipCode]) FROM [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) INNER JOIN [PaymentMethod] ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }
}
