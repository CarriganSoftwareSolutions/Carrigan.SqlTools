//using Carrigan.SqlTools.JoinTypes;
//using Carrigan.SqlTools.PredicatesLogic;
//using Carrigan.SqlTools.SqlGenerators;
//using Carrigan.SqlTools.SqlServer.IntegrationTests.CompositeModels;
//using Carrigan.SqlTools.SqlServer.IntegrationTests.DataSets;
//using Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;
//using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;
//using Microsoft.Data.SqlClient;

//namespace Carrigan.SqlTools.SqlServer.IntegrationTests;

//internal sealed class JoinsTests : IClassFixture<JoinsFixture>
//{
//    private readonly JoinsFixture _fixture;

//    private readonly SqlGenerator<Customer> CustomerSqlGenerator = new();

//    public JoinsTests(JoinsFixture fixture) =>
//        _fixture = fixture;


//    [Fact]
//    public async Task Join()
//    {
//        ColumnEqualsColumn<Customer, Order> joinPredicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
//        JoinBase join = new Join<Order>(joinPredicate);
//        SqlQuery query = CustomerSqlGenerator.Select(null, join, null, null, null);
//        await using SqlConnection unitTestConnection = new(_fixture.UnitTestConnectionString);
//        IEnumerable<BookStatsJoin> books = await CommandsAsync.ExecuteReaderAsync<BookStatsJoin>(query, null, unitTestConnection);

//        Assert.Equal(3, books.Count());
//    }
//}
