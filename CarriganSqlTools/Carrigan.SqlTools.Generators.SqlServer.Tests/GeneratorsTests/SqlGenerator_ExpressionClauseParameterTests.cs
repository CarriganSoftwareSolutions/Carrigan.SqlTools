using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public class SqlGenerator_ExpressionClauseParameterTests
{
    private static readonly SqlGenerator<Customer> CustomerGenerator = new();

    [Fact]
    public void Select_WithParameterizedGroupBy_PreservesParameterAndQueryWideOrdering()
    {
        SelectTags selects = new(new SelectTag(new Count(), "TotalCount"));
        Predicates where = new GreaterThan
        (
            new Column<Customer>(nameof(Customer.Id)),
            new Parameter(0, "MinimumId")
        );
        GroupBys groupBys = new
        (
            new GroupBy
            (
                new Add
                (
                    new Column<Customer>(nameof(Customer.Id)),
                    new Parameter(1, "GroupOffset")
                )
            )
        );

        SqlQuery query = CustomerGenerator.InternalSelect
        (
            null, null, selects, null, where, groupBys, null, null, null
        );

        Assert.Equal
        (
            "SELECT COUNT(*) AS [TotalCount] FROM [Customer] WHERE ([Customer].[Id] > @MinimumId_1) GROUP BY ([Customer].[Id] + @GroupOffset_2)",
            query.QueryText
        );
        Assert.Collection
        (
            query.Parameters,
            parameter =>
            {
                Assert.Equal("@MinimumId_1", parameter.ParameterTag.ToString());
                Assert.Equal(0, parameter.Value);
            },
            parameter =>
            {
                Assert.Equal("@GroupOffset_2", parameter.ParameterTag.ToString());
                Assert.Equal(1, parameter.Value);
            }
        );
    }

    [Fact]
    public void Select_WithParameterizedOrderBy_PreservesParameterAndQueryWideOrdering()
    {
        Predicates where = new GreaterThan
        (
            new Column<Customer>(nameof(Customer.Id)),
            new Parameter(0, "MinimumId")
        );
        OrderBys orderBys = new
        (
            new OrderBy
            (
                new Add
                (
                    new Column<Customer>(nameof(Customer.Id)),
                    new Parameter(1, "OrderOffset")
                )
            )
        );

        SqlQuery query = CustomerGenerator.InternalSelect
        (
            null, null, null, null, where, null, null, orderBys, null
        );

        Assert.Equal
        (
            "SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Id] > @MinimumId_1) ORDER BY ([Customer].[Id] + @OrderOffset_2) ASC",
            query.QueryText
        );
        Assert.Collection
        (
            query.Parameters,
            parameter =>
            {
                Assert.Equal("@MinimumId_1", parameter.ParameterTag.ToString());
                Assert.Equal(0, parameter.Value);
            },
            parameter =>
            {
                Assert.Equal("@OrderOffset_2", parameter.ParameterTag.ToString());
                Assert.Equal(1, parameter.Value);
            }
        );
    }
}
