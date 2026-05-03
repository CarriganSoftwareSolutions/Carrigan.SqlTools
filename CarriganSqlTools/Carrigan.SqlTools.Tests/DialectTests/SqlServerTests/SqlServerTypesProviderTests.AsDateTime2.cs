using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsDateTime2_ReturnsDateTime2() =>
        AssertTemporal(SqlServerTypesProvider.AsDateTime2(), "DATETIME2");

    [Fact]
    public void AsDateTime2_ReturnsNullable_WhenNullableTrue() => 
        AssertTemporal(SqlServerTypesProvider.AsDateTime2(true), "DATETIME2", expectedNullable: true);

    [Fact]
    public void AsDateTime2_WithFractionalSecondsPrecision_ReturnsDateTime2WithPrecision() => 
        AssertTemporal(SqlServerTypesProvider.AsDateTime2(7), "DATETIME2", 7);

    [Fact]
    public void AsDateTime2_WithFractionalSecondsPrecision_ReturnsNullable_WhenNullableTrue() => 
        AssertTemporal(SqlServerTypesProvider.AsDateTime2(7, true), "DATETIME2", 7, true);

    [Fact]
    public void AsDateTime2_Throws_WhenFractionalSecondsPrecisionIsTooLarge() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsDateTime2(8));
}
