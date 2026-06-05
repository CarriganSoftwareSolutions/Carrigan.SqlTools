using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;


namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;
public class OrderByExamples
{
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithWithOrderByItem()
    {
        OrderBy<Customer> orderBy = new(nameof(Customer.Name));
        SelectBuilder<Customer> selectBuilder = new()
        {
            OrderBys = orderBy
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" ORDER BY "Customer"."Name" ASC
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithWithTwoOrderByItems()
    {
        OrderBy<Customer> orderBy1 = new(nameof(Customer.Name));
        OrderBy<Customer> orderBy2 = new(nameof(Customer.Id), SortDirectionEnum.Descending);
        OrderBys orderBys = new (orderBy1, orderBy2);
        SelectBuilder<Customer> selectBuilder = new()
        {
            OrderBys = orderBys
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" ORDER BY "Customer"."Name" ASC, "Customer"."Id" DESC
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}
