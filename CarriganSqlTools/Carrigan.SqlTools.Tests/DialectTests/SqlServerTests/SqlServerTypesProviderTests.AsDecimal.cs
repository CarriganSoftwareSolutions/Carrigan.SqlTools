using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsDecimal_ReturnsDecimal() => 
        AssertDecimal(SqlServerTypesProvider.AsDecimal(), "DECIMAL", null, null);

    [Fact]
    public void AsDecimal_ReturnsNullable_WhenNullableTrue() =>
        AssertDecimal(SqlServerTypesProvider.AsDecimal(true), "DECIMAL", null, null, true);

    [Fact]
    public void AsDecimal_WithPrecision_ReturnsDecimalWithPrecision() => 
        AssertDecimal(SqlServerTypesProvider.AsDecimal(18), "DECIMAL", 18, null);

    [Fact]
    public void AsDecimal_WithPrecision_ReturnsNullable_WhenNullableTrue() =>
        AssertDecimal(SqlServerTypesProvider.AsDecimal(18, true), "DECIMAL", 18, null, true);

    [Fact]
    public void AsDecimal_WithPrecisionAndScale_ReturnsDecimalWithPrecisionAndScale() => 
        AssertDecimal(SqlServerTypesProvider.AsDecimal(18, 2), "DECIMAL", 18, 2);

    [Fact]
    public void AsDecimal_WithPrecisionAndScale_ReturnsNullable_WhenNullableTrue() => 
        AssertDecimal(SqlServerTypesProvider.AsDecimal(18, 2, true), "DECIMAL", 18, 2, true);

    [Fact]
    public void AsDecimal_Throws_WhenPrecisionIsZero() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsDecimal(0));

    [Fact]
    public void AsDecimal_Throws_WhenPrecisionIsTooLarge() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsDecimal(39));

    [Fact]
    public void AsDecimal_Throws_WhenScaleIsGreaterThanPrecision() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsDecimal(5, 6));
}
