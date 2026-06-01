using Carrigan.SqlTools.Base.Tests.Helpers;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class SelectLimitOffsetTests
{
    private readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void SelectWithLimitOffset()
    {
        LimitOffset limitOffset = new(25, 50);
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, limitOffset);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" ORDER BY \"Customer\".\"Id\" ASC LIMIT 25 OFFSET 50", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithLimitOffsetWithOrderBy()
    {
        LimitOffset limitOffset = new(25, 50);
        OrderBy<Customer> orderBy = new(nameof(Customer.Name));
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBy, limitOffset);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" ORDER BY \"Customer\".\"Name\" ASC, \"Customer\".\"Id\" ASC LIMIT 25 OFFSET 50", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithLimitOffsetWithDescendingOrderBy()
    {
        LimitOffset limitOffset = new(25, 50);
        OrderBy<Customer> orderBy = new(nameof(Customer.Name), SortDirectionEnum.Descending);
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, orderBy, limitOffset);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" ORDER BY \"Customer\".\"Name\" DESC, \"Customer\".\"Id\" ASC LIMIT 25 OFFSET 50", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithLimitOffsetWithFirstPage()
    {
        LimitOffset limitOffset = new(25, 0);
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, limitOffset);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" ORDER BY \"Customer\".\"Id\" ASC LIMIT 25", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }

    [Fact]
    public void SelectWithLimitOffsetWithSingleRowLimit()
    {
        LimitOffset limitOffset = new(1, 50);
        SqlQuery query = customerGenerator.Select(null, null, null, null, null, null, limitOffset);

        Assert.Equal("SELECT \"Customer\".* FROM \"Customer\" ORDER BY \"Customer\".\"Id\" ASC LIMIT 1 OFFSET 50", query.QueryText);
        Assert.Equal(System.Data.CommandType.Text, query.CommandType);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
    }
}