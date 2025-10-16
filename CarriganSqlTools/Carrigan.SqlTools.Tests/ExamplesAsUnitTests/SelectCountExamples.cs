using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.

namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class SelectCountExamples
{
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void SelectCountAll()
    {
        SqlQuery query = orderGenerator.SelectCount(null, null);

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

        SqlQuery query = orderGenerator.SelectCount(null, greaterThan);

        Assert.Equal("SELECT COUNT([Order].*) FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    }

    [Fact]
    public void SelectCountWithWhereAndJoin()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Column<Order> totalCol = new(nameof(Order.Total));
        Parameter minTotal = new("Total", 500m);
        GreaterThan greaterThan = new(totalCol, minTotal);

        ColumnEqualsColumn<Order, Customer> columnCompare = new(nameof(Order.CustomerId), nameof(Customer.Id));
        Joins<Order> joins = Joins<Order>.Join<Customer>(columnCompare);

        SqlQuery query = orderGenerator.SelectCount(joins, greaterThan);

        Assert.Equal("SELECT COUNT([Order].*) FROM [Order] JOIN [Customer] ON ([Order].[CustomerId] = [Customer].[Id]) WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    }
}
