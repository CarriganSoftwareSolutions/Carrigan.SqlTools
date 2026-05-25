using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.SqlServer;


namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;
public class PagingExamples
{
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithDefinePage()
    {
        DefinePage definePage = new(2, 25);
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, definePage);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithDefinePageWithOrderBy()
    {
        DefinePage definePage = new(2, 25);
        OrderBy<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBy, definePage);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
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
    public void SelectWithOffsetNextPageWithOrderBy()
    {
        OffsetFetchNext offsetNext = new(50, 25);
        OrderBy<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBy, offsetNext);

        Assert.Equal("SELECT [Customer].* FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
