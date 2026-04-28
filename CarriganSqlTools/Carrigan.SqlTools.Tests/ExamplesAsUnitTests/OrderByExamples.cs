using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tests.TestEntities; //this is where Customer and Order are defined.


namespace Carrigan.SqlTools.Tests.ExamplesAsUnitTests;
public class OrderByExamples
{
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithWithOrderByItem()
    {
        OrderByItem<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, null, orderBy, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }

    [Fact]
    public void SelectWithWithTwoOrderByItems()
    {
        OrderByItem<Customer> orderBy1 = new(nameof(Customer.Name));
        OrderByItem<Customer> orderBy2 = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBy orderBy = new(orderBy1, orderBy2);
        SqlQuery query = customerGenerator.Select(null, null, null, orderBy, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] DESC", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        Assert.Empty(query.Parameters);
    }
}
