using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExceptionTests;

public sealed class AggregateExpressionInWhereClauseExceptionTests
{
    private static And GetAggregatePredicate() =>
        new 
        (
            new GreaterThan
            (
                new Column<Customer>(nameof(Customer.Id)),
                new Parameter(0)
            ),
            new LessThan
            (
                new Cast
                (
                    new Min(new Column<Customer>(nameof(Customer.Id))),
                    SqlServerTypesProvider.AsDecimal(18, 2)
                ),
                new Parameter(1m)
            )
        );

    [Fact]
    public void Select_WithAggregateInWhere_ThrowsExpectedException()
    {
        SqlGenerator<Customer> sqlGenerator = new();
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = GetAggregatePredicate()
        };

        AggregateExpressionInWhereClauseException exception =
            Assert.Throws<AggregateExpressionInWhereClauseException>(() => sqlGenerator.Select(selectBuilder));

        Assert.Equal
        (
            "WHERE clauses cannot contain aggregate expressions. Use HAVING to filter aggregate results.",
            exception.Message
        );
    }

    [Fact]
    [Obsolete("Tests the obsolete SelectCount API.")]
    public void SelectCount_WithAggregateInWhere_ThrowsExpectedException()
    {
        SqlGenerator<Customer> sqlGenerator = new();

        Assert.Throws<AggregateExpressionInWhereClauseException>
        (
            () => sqlGenerator.SelectCount(null, null, null, GetAggregatePredicate())
        );
    }

    [Fact]
    public void Update_WithAggregateInWhere_ThrowsExpectedException()
    {
        SqlGenerator<Customer> sqlGenerator = new();
        UpdateBuilder<Customer> updateBuilder = new()
        {
            Values = new Customer { Name = "Updated" },
            UpdateColumns = new ColumnCollection<Customer>(nameof(Customer.Name)),
            Where = GetAggregatePredicate()
        };

        Assert.Throws<AggregateExpressionInWhereClauseException>(() => sqlGenerator.Update(updateBuilder));
    }

    [Fact]
    public void Delete_WithAggregateInWhere_ThrowsExpectedException()
    {
        SqlGenerator<Customer> sqlGenerator = new();
        DeleteBuilder<Customer> deleteBuilder = new()
        {
            Where = GetAggregatePredicate()
        };

        Assert.Throws<AggregateExpressionInWhereClauseException>(() => sqlGenerator.Delete(deleteBuilder));
    }

    [Fact]
    public void Subquery_WithAggregateInWhere_ThrowsExpectedException()
    {
        SqlGenerator<Customer> sqlGenerator = new();

        Assert.Throws<AggregateExpressionInWhereClauseException>
        (
            () => sqlGenerator.Subquery(null, null, null, GetAggregatePredicate(), null, null, null)
        );
    }

    [Fact]
    public void Select_WithAggregateInsideSubqueryProjection_DoesNotTreatAggregateAsOuterWhereExpression()
    {
        SqlGenerator<Customer> customerGenerator = new();
        SqlGenerator<Order> orderGenerator = new();
        SelectTags aggregateSelect = new(new SelectTag(new Count(), "TotalCount"));
        Subquery<Order> subquery = orderGenerator.Subquery(null, aggregateSelect, null, null, null, null, null);
        SelectBuilder<Customer> selectBuilder = new()
        {
            Where = new Exists(subquery)
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal
        (
            "SELECT [Customer].* FROM [Customer] WHERE (EXISTS (SELECT COUNT(*) AS [TotalCount] FROM [Order]))",
            query.QueryText
        );
    }
}
