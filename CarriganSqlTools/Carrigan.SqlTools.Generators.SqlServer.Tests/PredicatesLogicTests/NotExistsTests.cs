using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;


namespace Carrigan.SqlTools.Generators.SqlServer.Tests.PredicatesLogicTests;

public class NotExistsTests
{
    private readonly SqlGenerator<Customer> customerGenerator = new();
    private readonly SqlGenerator<Order> orderGenerator = new();


    [Fact]
    public void Select_WithNotExistsPredicate_RendersNotExistsSubquery()
    {
        Predicates subQueryPredicate = new GreaterThan
        (
            new Column<Order>(nameof(Order.Total)),
            new Parameter("Total", 100.00m)
        );
        Subquery<Order> subQuery = orderGenerator.Subquery(null, null, null, subQueryPredicate, null, null);
        NotExists notExists = new(subQuery);

        SqlQuery query = customerGenerator.Select(null, null, null, null, notExists, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE (NOT EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[Total] > @Total_1)))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_1", 100.00m);
    }

    [Fact]
    public void Select_WithNotExistsPredicateAndOuterPredicate_FinalizesParametersInRenderOrder()
    {
        Predicates subQueryPredicate = new Equal
        (
            new Column<Order>(nameof(Order.CustomerId)),
            new Parameter("CustomerId", 42)
        );
        Subquery<Order> subQuery = orderGenerator.Subquery(null, null, null, subQueryPredicate, null, null);
        NotExists notExists = new(subQuery);
        Predicates outerPredicate = new Equal
        (
            new Column<Customer>(nameof(Customer.Name)),
            new Parameter("Name", "Jonathan")
        );
        And and = new(notExists, outerPredicate);

        SqlQuery query = customerGenerator.Select(null, null, null, null, and, null, null);

        Assert.Equal("SELECT [Customer].* FROM [Customer] WHERE ((NOT EXISTS (SELECT [Order].* FROM [Order] WHERE ([Order].[CustomerId] = @CustomerId_1))) AND ([Customer].[Name] = @Name_2))", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@CustomerId_1", 42);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_2", "Jonathan");
    }
}
