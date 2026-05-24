using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public class SqlGenerator_SubQueryTests
{
    private static readonly SqlServerDialect Dialect = new();

    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SubQuery_SelectAll_WrapsSelectAndUsesModelMetadataAsAlias()
    {
        SqlGenerator<EntityWithTableAttribute> generator = new();

        SubQuery<EntityWithTableAttribute> subQuery = generator.SubQuery(null, null, null, null, null, null);

        Assert.Equal("(SELECT [Test].* FROM [Test])", subQuery.ToSql(Dialect));
    }

    [Fact]
    public void SubQuery_WithDistinctSelectsOrderByAndPaging_RendersExpectedSql()
    {
        SelectTags selects = SelectTags.GetMany<Customer>
        (
            nameof(Customer.Id),
            nameof(Customer.Name)
        );

        OrderBy<Customer> orderBy = new(nameof(Customer.Name));
        DefinePage paging = new(2, 25);

        SubQuery<Customer> subQuery = customerGenerator.SubQuery(true, selects, null, null, orderBy, paging);

        Assert.Equal(
            "(SELECT DISTINCT [Customer].[Id], [Customer].[Name] FROM [Customer] ORDER BY [Customer].[Name] ASC, [Customer].[Id] ASC OFFSET 25 ROWS FETCH NEXT 25 ROWS ONLY)",
            subQuery.ToSql(Dialect));
    }

    [Fact]
    public void SubQuery_WithPredicate_ReturnsFinalizedParameters()
    {
        Predicates predicate = new Equal
        (
            new Column<Customer>(nameof(Customer.Id)),
            new Parameter("CustomerId", 42)
        );

        SubQuery<Customer> subQuery = customerGenerator.SubQuery(null, null, null, predicate, null, null);

        IEnumerable<SqlFragmentParameter> parameters = [.. subQuery.GetSqlFragmentParameters()];

        Assert.Single(parameters);
        SqlQueryTestHelper.AssertParameterValue(parameters, "@CustomerId_1", 42);
    }

    [Fact]
    public void SubQuery_WhenRenderedInsideOuterFragmentSequence_FinalizesInnerAndOuterParametersTogether()
    {
        Predicates predicate = new Equal
        (
            new Column<Customer>(nameof(Customer.Id)),
            new Parameter("CustomerId", 42)
        );

        SubQuery<Customer> subQuery = customerGenerator.SubQuery(null, null, null, predicate, null, null);

        IEnumerable<ISqlFragment> fragments =
        [
            new SqlFragmentText("SELECT * FROM "),
            subQuery,
            new SqlFragmentText(" AS [Customer] WHERE [Customer].[Name] = "),
            new SqlFragmentParameter(new Parameter("Name", "Jonathan"))
        ];

        string sql = fragments.ToSql(Dialect);
        IEnumerable<SqlFragmentParameter> parameters = [.. fragments.GetSqlFragmentParameters(Dialect)];

        Assert.Equal(
            "SELECT * FROM (SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Id] = @CustomerId_1)) AS [Customer] WHERE [Customer].[Name] = @Name_2",
            sql);

        Assert.Equal(2, parameters.Count());
        SqlQueryTestHelper.AssertParameterValue(parameters, "@CustomerId_1", 42);
        SqlQueryTestHelper.AssertParameterValue(parameters, "@Name_2", "Jonathan");
    }

    [Fact]
    public void SubQuery_WithSelectFromUnjoinedTable_ThrowsInvalidTableException()
    {
        SelectTags selects = SelectTags.Get<Order>(nameof(Order.Id));

        Assert.Throws<InvalidTableException>(() =>
            customerGenerator.SubQuery(null, selects, null, null, null, null));
    }
}