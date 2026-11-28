using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.PredicateLogicTests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class ExistsTests : PredicateLogicBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
        [];

    private readonly SqlGenerator<Customer> customerGenerator = new();
    private readonly SqlGenerator<Order> orderGenerator = new();


    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Exists(new Subquery(new SelectTags(), [])),
        new Exists(new Subquery(new SelectTags(), [])),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual
    {
        get
        {
            Subquery subquery = new(new SelectTags(), []);

            return
            [
                new Exists(subquery),
                new Exists(subquery),
            ];
        }
    }

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Exists(new Subquery(new SelectTags(), [])),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Exists(new Subquery(new SelectTags(), [])),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Exists(new Subquery(new SelectTags(), [])),
    ];

    [Fact]
    public void Select_WithExistsPredicate_RendersExistsSubquery()
    {
        Predicates subQueryPredicate = new GreaterThan
        (
            new Column<Order>(nameof(Order.Total)),
            new Parameter(100.00m, "Total")
        );
        Subquery subQuery = orderGenerator.Subquery(null, null, new From<Order>(), null, subQueryPredicate, null, null, null, null);
        Exists exists = new(subQuery);

        SqlQuery query = customerGenerator.Select(new SelectBuilder<Customer> { Where = exists });

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Total_1)))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 100.00m);
    }

    [Fact]
    public void Select_WithExistsPredicateAndOuterPredicate_FinalizesParametersInRenderOrder()
    {
        Predicates subQueryPredicate = new Equal
        (
            new Column<Order>(nameof(Order.CustomerId)),
            new Parameter(42, "CustomerId")
        );
        Subquery subQuery = orderGenerator.Subquery(null, null, new From<Order>(), null, subQueryPredicate, null, null, null, null);
        Exists exists = new(subQuery);
        Predicates outerPredicate = new Equal
        (
            new Column<Customer>(nameof(Customer.Name)),
            new Parameter("Jonathan", "Name")
        );
        And and = new(exists, outerPredicate);

        SqlQuery query = customerGenerator.Select(new SelectBuilder<Customer> { Where = and });

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ((EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[CustomerId] = @CustomerId_1))) AND ([Customer].[Name] = @Name_2))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@CustomerId_1", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", "Jonathan");
    }

}
