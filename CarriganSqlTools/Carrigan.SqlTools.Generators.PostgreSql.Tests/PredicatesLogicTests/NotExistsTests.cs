using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.PredicateLogicTests;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.PredicatesLogicTests;

public class NotExistsTests : PredicateLogicBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
        [];

    private readonly SqlGenerator<Customer> customerGenerator = new();
    private readonly SqlGenerator<Order> orderGenerator = new();



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new NotExists(new SubqueryBase([], new PostgreSqlDialect())),
        new NotExists(new SubqueryBase([], new PostgreSqlDialect())),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual
    {
        get
        {
            SubqueryBase subquery = new([], new PostgreSqlDialect());

            return
            [
                new NotExists(subquery),
                new NotExists(subquery),
            ];
        }
    }

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new NotExists(new SubqueryBase([], new PostgreSqlDialect())),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new NotExists(new SubqueryBase([], new PostgreSqlDialect())),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
        [];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new NotExists(new SubqueryBase([], new PostgreSqlDialect())),
    ];

    [Fact]
    public void Select_WithNotExistsPredicate_RendersNotExistsSubquery()
    {
        Predicates subQueryPredicate = new GreaterThan
        (
            new Column<Order>(nameof(Order.Total)),
            new Parameter(100.00m, "Total")
        );
        Subquery<Order> subQuery = orderGenerator.InternalSubquery(null, null, null, subQueryPredicate, null, null, null, null);
        NotExists notExists = new(subQuery);

        SqlQuery query = customerGenerator.InternalSelect(null, null, null, null, notExists, null, null, null, null);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" WHERE (NOT EXISTS (SELECT \"Order\".* FROM \"Order\" WHERE (\"Order\".\"Total\" > $1)))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 100.00m);
    }

    [Fact]
    public void Select_WithNotExistsPredicateAndOuterPredicate_FinalizesParametersInRenderOrder()
    {
        Predicates subQueryPredicate = new Equal
        (
            new Column<Order>(nameof(Order.CustomerId)),
            new Parameter(42, "CustomerId")
        );
        Subquery<Order> subQuery = orderGenerator.InternalSubquery(null, null, null, subQueryPredicate, null, null, null, null);
        NotExists notExists = new(subQuery);
        Predicates outerPredicate = new Equal
        (
            new Column<Customer>(nameof(Customer.Name)),
            new Parameter("Jonathan", "Name")
        );
        And and = new(notExists, outerPredicate);

        SqlQuery query = customerGenerator.InternalSelect(null, null, null, null, and, null, null, null, null);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" WHERE ((NOT EXISTS (SELECT \"Order\".* FROM \"Order\" WHERE (\"Order\".\"CustomerId\" = $1))) AND (\"Customer\".\"Name\" = $2))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Jonathan");
    }
}
