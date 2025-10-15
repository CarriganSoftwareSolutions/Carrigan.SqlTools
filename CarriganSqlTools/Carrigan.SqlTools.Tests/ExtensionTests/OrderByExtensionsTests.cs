using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.ExtensionTests;


public class OrderByExtensionsTests
{
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_True()
    {
        OrderBy order = new(new OrderByItem<ColumnTable>(nameof(ColumnTable.D000destruct0)));

        Assert.True(order.IsNotNullOrEmpty());
    }
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_False_IsEmpty()
    {
        OrderByBase order = OrderBy.Empty;

        Assert.False(order.IsNotNullOrEmpty());
    }
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_False_IsNull()
    {
        OrderByBase? order = null;

        Assert.False(order.IsNotNullOrEmpty());
    }


    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsNull()
    {
        OrderByBase? order = null;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsEmpty()
    {
        OrderByBase order = OrderBy.Empty;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_False()
    {
        OrderByBase order = new OrderBy(new OrderByItem<ColumnTable>(nameof(ColumnTable.D000destruct0)));

        Assert.False(order.IsNullOrEmpty());
    }
}
