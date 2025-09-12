using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OffsetNexts;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.
using System.Text;


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class SelectCountExamples
{
    private SqlGenerator<Customer> customerGenerator = new();
    private SqlGenerator<Order> orderGenerator = new();
    private SqlGenerator<PaymentMethod> paymentmethodGenerator = new();


    [Fact]
    public void SelectCountAll()
    {
        SqlQuery query = orderGenerator.SelectCount(null, null);

        Assert.Equal("SELECT COUNT(*) FROM [Order]", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectCountWithWhere()
    {
        //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
        Columns<Order> totalCol = new(nameof(Order.Total));
        Parameters minTotal = new("Total", 500m);
        GreaterThan greaterThan = new(totalCol, minTotal);

        SqlQuery query = orderGenerator.SelectCount(null, greaterThan);

        Assert.Equal("SELECT COUNT(*) FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Single(query.Parameters);
        Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    }

    //[Fact]
    //public void SelectCountWithWhereAndJoin()
    //{
    //    //Note: Columns<T> validates the names of the properties, and throws an error if the property isn't valid
    //    Columns<Order> totalCol = new(nameof(Order.Total));
    //    Parameters minTotal = new("Total", 500m);
    //    GreaterThan greaterThan = new(totalCol, minTotal);

    //    Columns<Order> totalCol customerId
    //    Join join = new()

    //    SqlQuery query = customerGenerator.SelectCount(null, greaterThan);

    //    Assert.Equal("SELECT COUNT(*) FROM [Order] WHERE ([Order].[Total] > @Parameter_Total)", query.QueryText);
    //    Assert.Equal(System.Data.CommandType.Text, query.CommandType);
    //    Assert.Single(query.Parameters);
    //    Assert.Equal(500m, (decimal)query.Parameters.Where(param => param.Key == "@Parameter_Total").Single().Value);
    //}
}
