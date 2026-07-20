using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public sealed class CastTests
{
    private static readonly ISqlDialects Dialect = new SqlServerDialect();

    [Fact]
    public void SelectTag_WithCast_RendersExpectedSql()
    {
        string expected = "CAST([Customer].[Id] AS NVARCHAR(100)) AS [IdText]";
        Cast cast = new
        (
            new Column<Customer>(nameof(Customer.Id)),
            SqlServerTypesProvider.AsNVarChar(100, true)
        );
        SelectTag select = new(cast, "IdText");

        string actual = select.ToSql(Dialect);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Select_WithCastAroundAggregate_RendersExpectedGroupedQuery()
    {
        string expected = "SELECT [Customer].[Name], AVG(CAST([Customer].[Id] AS DECIMAL(18, 2))) AS [AverageId] FROM [Customer] GROUP BY [Customer].[Name]";
        SqlGenerator<Customer> generator = new();
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));

        Column<Customer> customerIdColumn = new (nameof(Customer.Id));
        Cast customerId = new(customerIdColumn, SqlServerTypesProvider.AsDecimal(18, 2, true));

        Average averageId = new(customerId);
        SelectTags selects = new
        (
            SelectTagGenerator.Get<Customer>(nameof(Customer.Name)),
            new SelectTag(averageId, "AverageId")
        );

        SqlQuery actual = generator.Select(null, null, selects, null, null, groupBys, null, null);

        Assert.Equal(expected, actual.QueryText);
    }

    [Fact]
    public void IsAggregate_DelegatesToGroupedColumnExpression()
    {
        GroupBys groupBys = GroupBys.New<Customer>(nameof(Customer.Name));
        Cast cast = new
        (
            new Column<Customer>(nameof(Customer.Name)),
            SqlServerTypesProvider.AsNVarChar(100)
        );

        bool actual = cast.IsAggregate(groupBys);

        Assert.True(actual);
    }
}
