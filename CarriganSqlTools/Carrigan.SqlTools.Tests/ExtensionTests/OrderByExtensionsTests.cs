using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.ExtensionTests;


public class OrderByExtensionsTests
{
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_True()
    {
        OrderBy order = new(new OrderByItem<ColumnTable>(nameof(ColumnTable.D000descruct0)));

        Assert.True(order.IsNotNullOrEmpty());
    }
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_False_IsEmpty()
    {
        IOrderByClause order = OrderBy.Empty;

        Assert.False(order.IsNotNullOrEmpty());
    }
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_False_IsNull()
    {
        IOrderByClause? order = null;

        Assert.False(order.IsNotNullOrEmpty());
    }


    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsNull()
    {
        IOrderByClause? order = null;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsEmpty()
    {
        IOrderByClause order = OrderBy.Empty;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_False()
    {
        IOrderByClause order = new OrderBy(new OrderByItem<ColumnTable>(nameof(ColumnTable.D000descruct0)));

        Assert.False(order.IsNullOrEmpty());
    }
}
