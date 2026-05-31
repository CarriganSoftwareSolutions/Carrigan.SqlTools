using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.QueryBuilderTests;

public class SubqueryBuilderTests
{
    private static readonly SqlServerDialect Dialect = new();
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SubqueryBuilder_WithDistinctSelectsWhereOrderByAndPaging_RendersExpectedSql()
    {
        SubqueryBuilder<Customer> subqueryBuilder = new()
        {
            Distinct = true,
            Selects = SelectTagGenerator.GetMany<Customer>(nameof(Customer.Id), nameof(Customer.Name)),
            Where = new Equal(new Column<Customer>(nameof(Customer.Name)), new Parameter("Hank", "Name")),
            OrderBys = new OrderBys(new OrderBy<Customer>(nameof(Customer.Name))),
            Paging = new DefinePage(2, 25)
        };

        Subquery<Customer> subquery = customerGenerator.Subquery(subqueryBuilder);

        SqlQuery query = new(Dialect, CommandType.Text, subquery.Flatten());

        Assert.Equal("(SELECT DISTINCT [Customer].[Id], [Customer].[Name] FROM [Customer] WHERE ([Customer].[Name] = @Name_1) ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
    }
}
