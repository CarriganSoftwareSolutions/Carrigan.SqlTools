using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.QueryBuilderTests;

public class SelectBuilderSubqueryTests
{
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void Select_WithSubquerySource_RendersSubqueryAsFromSource()
    {
        Predicates subqueryPredicate = new Equal(new Column<Customer>(nameof(Customer.Name)), new Parameter("Hank", "Name"));
        Subquery<Customer> subquery = customerGenerator.Subquery(null, null, null, subqueryPredicate, null, null, null);
        SelectTags selects = SelectTagGenerator.GetMany<Customer>(nameof(Customer.Id), nameof(Customer.Email));
        Predicates outerPredicate = new Equal(new Column<Customer>(nameof(Customer.Email)), new Parameter("hank@example.com", "Email"));

        SqlQuery query = customerGenerator.Select(null, subquery, selects, null, outerPredicate, null, null, null);

        Assert.Equal("SELECT [Customer].[Id], [Customer].[Email] FROM (SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)) AS [Customer] WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "hank@example.com");
    }

    [Fact]
    public void SelectBuilder_WithSubquerySource_RendersSubqueryAsFromSource()
    {
        Predicates subqueryPredicate = new Equal(new Column<Customer>(nameof(Customer.Name)), new Parameter("Hank", "Name"));
        Predicates outerPredicate = new Equal(new Column<Customer>(nameof(Customer.Email)), new Parameter("hank@example.com", "Email"));
        Subquery<Customer> subquery = customerGenerator.Subquery(null, null, null, subqueryPredicate, null, null, null);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Subquery = subquery,
            Selects = SelectTagGenerator.GetMany<Customer>(nameof(Customer.Id), nameof(Customer.Email)),
            Where = outerPredicate
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].[Id], [Customer].[Email] FROM (SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)) AS [Customer] WHERE ([Customer].[Email] = @Email_2)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Email_2", "hank@example.com");
    }

    [Fact]
    public void SelectBuilder_WithDistinctSubqueryOrderByAndPaging_RendersExpectedSql()
    {
        Subquery<Customer> subquery = customerGenerator.Subquery
        (
            true,
            SelectTagGenerator.GetMany<Customer>(nameof(Customer.Id), nameof(Customer.Name)),
            null,
            null,
            null,
            new OrderBys(new OrderBy<Customer>(nameof(Customer.Name))), new DefinePage(2, 25)
);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Distinct = true,
            Subquery = subquery,
            Selects = SelectTagGenerator.GetMany<Customer>(nameof(Customer.Id), nameof(Customer.Name)),
            OrderBys = new OrderBys(new OrderBy<Customer>(nameof(Customer.Name))),
            Paging = new DefinePage(1, 10)
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT DISTINCT [Customer].[Id], [Customer].[Name] FROM (SELECT DISTINCT [Customer].[Id], [Customer].[Name] FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY) AS [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectBuilder_WithSubquerySourceAndJoin_RendersJoinedSql()
    {
        Predicates subqueryPredicate = new Equal(new Column<Customer>(nameof(Customer.Name)), new Parameter("Hank", "Name"));
        Subquery<Customer> subquery = customerGenerator.Subquery(null, null, null, subqueryPredicate, null, null, null);
        ColumnEqualsColumn<Customer, Order> customerIdEqualsOrderCustomerId = new(nameof(Customer.Id), nameof(Order.CustomerId));
        Joins<Customer> joins = new(new InnerJoin<Order>(customerIdEqualsOrderCustomerId));
        SelectTags selects = new SelectTags(SelectTagGenerator.Get<Customer>(nameof(Customer.Id), "CustomerId")).Append<Order>(nameof(Order.Total));
        Predicates where = new GreaterThan(new Column<Order>(nameof(Order.Total)), new Parameter(500m, "Total"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Subquery = subquery,
            Selects = selects,
            Joins = joins,
            Where = where
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        Assert.Equal("SELECT [Customer].[Id] AS [CustomerId], [Order].[Total] FROM (SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Name_1)) AS [Customer] INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) WHERE ([Order].[Total] > @Total_2)", query.QueryText);
        Assert.Equal(CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 2);
        SqlQueryTestHelper.AssertParameterValue(query, "@Name_1", "Hank");
        SqlQueryTestHelper.AssertParameterValue(query, "@Total_2", 500m);
    }
}
