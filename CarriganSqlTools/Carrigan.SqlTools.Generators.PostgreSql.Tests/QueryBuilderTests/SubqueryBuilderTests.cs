using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.QueryBuilderTests;

public class SubqueryBuilderTests
{
    private static readonly PostgreSqlDialect Dialect = new();
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SubqueryBuilder_WithDistinctSelectsWhereOrderByAndPaging_RendersExpectedSql()
    {
        SubqueryBuilder<Customer> subqueryBuilder = new()
        {
            Distinct = true,
            Selects = SelectTags.GetMany<Customer>(nameof(Customer.Id), nameof(Customer.Name)),
            Where = new Equal(new Column<Customer>(nameof(Customer.Name)), new Parameter("Hank", "Name")),
            OrderBys = new OrderBy<Customer>(nameof(Customer.Name)),
            Paging = new LimitOffset(25, 50)
        };

        Subquery<Customer> subquery = customerGenerator.Subquery(subqueryBuilder);

        SqlQuery query = new(Dialect, CommandType.Text, subquery.Flatten());

        Assert.Equal("(SELECT DISTINCT \"Customer\".\"Id\", \"Customer\".\"Name\" FROM \"Customer\" WHERE (\"Customer\".\"Name\" = $1) ORDER BY \"Customer\".\"Name\" ASC, \"Customer\".\"Id\" ASC LIMIT 25 OFFSET 50)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", "Hank");
    }
}
