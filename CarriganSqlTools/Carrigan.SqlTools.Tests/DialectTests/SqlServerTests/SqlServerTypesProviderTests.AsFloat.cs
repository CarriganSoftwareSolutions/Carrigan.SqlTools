using System;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Types;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsFloat_ReturnsFloat() =>
        AssertSimple(SqlServerTypesProvider.AsFloat(), "FLOAT");

    [Fact]
    public void AsFloat_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsFloat(true), "FLOAT", true);

    [Fact]
    public void AsFloat_WithPrecision_ReturnsFloatWithPrecision()
    {
        FieldProperties actual = SqlServerTypesProvider.AsFloat(24);

        Assert.Equal("FLOAT", actual.ProviderTypeName);
        Assert.Equal((byte)24, actual.Precision);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void AsFloat_WithPrecision_ReturnsNullable_WhenNullableTrue()
    {
        FieldProperties actual = SqlServerTypesProvider.AsFloat(24, true);

        Assert.Equal("FLOAT", actual.ProviderTypeName);
        Assert.Equal((byte)24, actual.Precision);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void AsFloat_Throws_WhenPrecisionIsZero() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsFloat(0));

    [Fact]
    public void AsFloat_Throws_WhenPrecisionIsTooLarge() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsFloat(54));
}
