using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsVector_ReturnsVectorWithDimensions()
    {
        FieldProperties actual = SqlServerTypesProvider.AsVector(3);

        Assert.Equal("VECTOR", actual.ProviderTypeName);
        Assert.Equal(3, actual.Length);
        Assert.Null(actual.BaseType);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void AsVector_ReturnsNullable_WhenNullableTrue()
    {
        FieldProperties actual = SqlServerTypesProvider.AsVector(3, true);

        Assert.Equal("VECTOR", actual.ProviderTypeName);
        Assert.Equal(3, actual.Length);
        Assert.Null(actual.BaseType);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void AsVector_WithBaseType_KeepsBaseProviderTypeName()
    {
        FieldProperties actual = SqlServerTypesProvider.AsVector(3, "FLOAT32");

        Assert.Equal("VECTOR", actual.ProviderTypeName);
        Assert.Equal(3, actual.Length);
        Assert.Equal("FLOAT32", actual.BaseType);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void AsVector_WithBaseType_ReturnsNullable_WhenNullableTrue()
    {
        FieldProperties actual = SqlServerTypesProvider.AsVector(3, "FLOAT16", true);

        Assert.Equal("VECTOR", actual.ProviderTypeName);
        Assert.Equal(3, actual.Length);
        Assert.Equal("FLOAT16", actual.BaseType);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void AsVector_Throws_WhenDimensionsTooSmall() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsVector(0));

    [Fact]
    public void AsVector_Throws_WhenDimensionsTooLarge() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsVector(1999));

    [Fact]
    public void AsVector_Throws_WhenBaseTypeInvalid() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => SqlServerTypesProvider.AsVector(3, "DOUBLE"));
}