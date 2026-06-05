using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities; //this is where Customer and Order are defined.
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;


namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;
public class PagingExamples
{
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithDefinePage()
    {
        DefinePage definePage = new(2, 25);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Paging = definePage
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" ORDER BY "Customer"."Id" ASC LIMIT 25 OFFSET 25
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithDefinePageWithOrderBy()
    {
        DefinePage definePage = new(2, 25);
        OrderBy<Customer> orderBy = new(nameof(Customer.Name));
        SelectBuilder<Customer> selectBuilder = new()
        {
            OrderBys = orderBy,
            Paging = definePage
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expectedQueryText =
            """
            SELECT "Customer".* FROM "Customer" ORDER BY "Customer"."Name" ASC, "Customer"."Id" ASC LIMIT 25 OFFSET 25
            """;

        Assert.Equal(expectedQueryText, query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }


}
