using Carrigan.SqlTools.OrderByClause;
using Carrigan.SqlTools.Base.Tests.TestEntities;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExtensionTests;


public class OrderByExtensionsTests
{
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_True()
    {
        OrderBys order = new(new OrderBy<ColumnTable>(nameof(ColumnTable.D000destruct0)));

        Assert.True(order.IsNotNullOrEmpty());
    }
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_False_IsEmpty()
    {
        OrderBys order = OrderBys.Empty;

        Assert.False(order.IsNotNullOrEmpty());
    }
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_False_IsNull()
    {
        OrderBys? order = null;

        Assert.False(order.IsNotNullOrEmpty());
    }


    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsNull()
    {
        OrderBys? order = null;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsEmpty()
    {
        OrderBys order = OrderBys.Empty;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_False()
    {
        OrderBys order = new(new OrderBy<ColumnTable>(nameof(ColumnTable.D000destruct0)));

        Assert.False(order.IsNullOrEmpty());
    }
}
