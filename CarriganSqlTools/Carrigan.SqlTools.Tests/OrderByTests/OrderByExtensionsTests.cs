using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class OrderByExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_ReturnsTrue_ForNullOrderBy()
    {
        OrderBys? orderBy = null;

        Assert.True(orderBy.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_ReturnsTrue_ForEmptyOrderBy() =>
        Assert.True(OrderBys.Empty.IsNullOrEmpty());

    [Fact]
    public void IsNullOrEmpty_ReturnsFalse_ForNonEmptyOrderBy() =>
        Assert.False(new OrderBys(new OrderBy<Address>("City")).IsNullOrEmpty());

    [Fact]
    public void IsNotNullOrEmpty_ReturnsFalse_ForNullOrderBy()
    {
        OrderBys? orderBy = null;

        Assert.False(orderBy.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_ReturnsFalse_ForEmptyOrderBy() =>
        Assert.False(OrderBys.Empty.IsNotNullOrEmpty());

    [Fact]
    public void IsNotNullOrEmpty_ReturnsTrue_ForNonEmptyOrderBy() =>
        Assert.True(new OrderBys(new OrderBy<Address>("City")).IsNotNullOrEmpty());
}
