using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsDateTimeOffset_ReturnsDateTimeOffset() => 
        AssertTemporal(SqlServerTypesProvider.AsDateTimeOffset(), "DATETIMEOFFSET");

    [Fact]
    public void AsDateTimeOffset_ReturnsNullable_WhenNullableTrue() => 
        AssertTemporal(SqlServerTypesProvider.AsDateTimeOffset(true), "DATETIMEOFFSET", expectedNullable: true);

    [Fact]
    public void AsDateTimeOffset_WithFractionalSecondsPrecision_ReturnsDateTimeOffsetWithPrecision() =>
        AssertTemporal(SqlServerTypesProvider.AsDateTimeOffset(7), "DATETIMEOFFSET", 7);

    [Fact]
    public void AsDateTimeOffset_WithFractionalSecondsPrecision_ReturnsNullable_WhenNullableTrue() =>
        AssertTemporal(SqlServerTypesProvider.AsDateTimeOffset(7, true), "DATETIMEOFFSET", 7, true);

    [Fact]
    public void AsDateTimeOffset_Throws_WhenFractionalSecondsPrecisionIsTooLarge() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsDateTimeOffset(8));
}
