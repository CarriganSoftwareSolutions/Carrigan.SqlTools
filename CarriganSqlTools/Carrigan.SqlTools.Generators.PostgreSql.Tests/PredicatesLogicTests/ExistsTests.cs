using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.PredicatesLogicTests;

public class ExistsTests
{
    private readonly SqlGenerator<Customer> customerGenerator = new();
    private readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void Select_WithExistsPredicate_RendersExistsSubquery()
    {
        Predicates subQueryPredicate = new GreaterThan
        (
            new Column<Order>(nameof(Order.Total)),
            new Parameter(100.00m, "Total")
        );
        Subquery<Order> subQuery = orderGenerator.InternalSubquery(null, null, null, subQueryPredicate, null, null, null, null);
        Exists exists = new(subQuery);

        SqlQuery query = customerGenerator.InternalSelect(null, null, null, null, exists, null, null, null, null);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" WHERE (EXISTS (SELECT \"Order\".* FROM \"Order\" WHERE (\"Order\".\"Total\" > $1)))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 100.00m);
    }

    [Fact]
    public void Select_WithExistsPredicateAndOuterPredicate_FinalizesParametersInRenderOrder()
    {
        Predicates subQueryPredicate = new Equal
        (
            new Column<Order>(nameof(Order.CustomerId)),
            new Parameter(42, "CustomerId")
        );
        Subquery<Order> subQuery = orderGenerator.InternalSubquery(null, null, null, subQueryPredicate, null, null, null, null);
        Exists exists = new(subQuery);
        Predicates outerPredicate = new Equal
        (
            new Column<Customer>(nameof(Customer.Name)),
            new Parameter("Jonathan", "Name")
        );
        And and = new(exists, outerPredicate);

        SqlQuery query = customerGenerator.InternalSelect(null, null, null, null, and, null, null, null, null);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" WHERE ((EXISTS (SELECT \"Order\".* FROM \"Order\" WHERE (\"Order\".\"CustomerId\" = $1))) AND (\"Customer\".\"Name\" = $2))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "$1", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "$2", "Jonathan");
    }

}
