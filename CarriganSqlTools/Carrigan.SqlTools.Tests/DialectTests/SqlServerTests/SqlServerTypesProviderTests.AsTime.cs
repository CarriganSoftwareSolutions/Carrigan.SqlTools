using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsTime_ReturnsTime() =>
        AssertTemporal(SqlServerTypesProvider.AsTime(), "TIME");

    [Fact]
    public void AsTime_ReturnsNullable_WhenNullableTrue() => 
        AssertTemporal(SqlServerTypesProvider.AsTime(true), "TIME", expectedNullable: true);

    [Fact]
    public void AsTime_WithFractionalSecondsPrecision_ReturnsTimeWithPrecision() => 
        AssertTemporal(SqlServerTypesProvider.AsTime(7), "TIME", 7);

    [Fact]
    public void AsTime_WithFractionalSecondsPrecision_ReturnsNullable_WhenNullableTrue() => 
        AssertTemporal(SqlServerTypesProvider.AsTime(7, true), "TIME", 7, true);

    [Fact]
    public void AsTime_Throws_WhenFractionalSecondsPrecisionIsTooLarge() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsTime(8));
}
