using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.GroupByClause;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GroupByTests;

public class GroupByExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_ReturnsTrue_ForNullGroupBy()
    {
        GroupBys? groupBy = null;

        Assert.True(groupBy.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_ReturnsTrue_ForEmptyGroupBy() =>
        Assert.True(GroupBys.Empty.IsNullOrEmpty());

    [Fact]
    public void IsNullOrEmpty_ReturnsFalse_ForNonEmptyGroupBy() =>
        Assert.False(new GroupBys(new GroupBy<Address>("City")).IsNullOrEmpty());

    [Fact]
    public void IsNotNullOrEmpty_ReturnsFalse_ForNullGroupBy()
    {
        GroupBys? groupBy = null;

        Assert.False(groupBy.IsNotNullOrEmpty());
    }

    [Fact]
    public void IsNotNullOrEmpty_ReturnsFalse_ForEmptyGroupBy() =>
        Assert.False(GroupBys.Empty.IsNotNullOrEmpty());

    [Fact]
    public void IsNotNullOrEmpty_ReturnsTrue_ForNonEmptyGroupBy() =>
        Assert.True(new GroupBys(new GroupBy<Address>("City")).IsNotNullOrEmpty());
}
