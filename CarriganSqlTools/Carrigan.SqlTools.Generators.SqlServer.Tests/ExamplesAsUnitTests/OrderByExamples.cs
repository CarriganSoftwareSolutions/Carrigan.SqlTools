using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.SqlServer;


namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;
public class OrderByExamples
{
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithWithOrderByItem()
    {
        OrderBy<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBy, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithWithTwoOrderByItems()
    {
        OrderBy<Customer> orderBy1 = new(nameof(Customer.Name));
        OrderBy<Customer> orderBy2 = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBys orderBys = new(orderBy1, orderBy2);
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBys, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] DESC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
