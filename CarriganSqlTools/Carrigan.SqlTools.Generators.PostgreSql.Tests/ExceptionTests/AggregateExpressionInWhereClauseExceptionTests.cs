using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExceptionTests;

public sealed class AggregateExpressionInWhereClauseExceptionTests
{
    [Fact]
    public void Select_WithAggregateInWhere_ThrowsExpectedException()
    {
        SqlGenerator<Customer> sqlGenerator = new();
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = new GreaterThan
            (
                new Average(new Column<Customer>(nameof(Customer.Id))),
                new Parameter(1m)
            )
        };

        Assert.Throws<AggregateExpressionInWhereClauseException>(() => sqlGenerator.Select(selectBuilder));
    }
}
