using Carrigan.SqlTools.Extensions;
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
        OrderBy order = OrderBy.Empty;

        Assert.False(order.IsNotNullOrEmpty());
    }
    [Fact]
    public void OrderByExtension_IsNotNullOrEmpty_False_IsNull()
    {
        OrderBy? order = null;

        Assert.False(order.IsNotNullOrEmpty());
    }


    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsNull()
    {
        OrderBy? order = null;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_True_IsEmpty()
    {
        OrderBy order = OrderBy.Empty;

        Assert.True(order.IsNullOrEmpty());
    }

    [Fact]
    public void OrderByExtension_IsNullOrEmpty_False()
    {
        OrderBy order = new(new OrderByItem<ColumnTable>(nameof(ColumnTable.D000descruct0)));

        Assert.False(order.IsNullOrEmpty());
    }
}
