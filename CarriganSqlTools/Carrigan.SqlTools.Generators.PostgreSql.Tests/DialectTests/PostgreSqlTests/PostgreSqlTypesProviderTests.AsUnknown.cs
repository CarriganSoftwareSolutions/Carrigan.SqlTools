using Carrigan.SqlTools.Dialects.PostgreSql;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsUnknown_WithNullableTrue_ReturnsUnknownNullableFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.AsUnknown(nullable: true);

        Assert.Equal("UNKNOWN", fieldProperties.ProviderTypeName);
        Assert.True(fieldProperties.IsNullable);
        Assert.Null(fieldProperties.Length);
        Assert.Null(fieldProperties.IsMax);
        Assert.Null(fieldProperties.IsUnicode);
        Assert.Null(fieldProperties.IsFixedLength);
        Assert.Null(fieldProperties.Precision);
        Assert.Null(fieldProperties.Scale);
        Assert.Null(fieldProperties.FractionalSecondsPrecision);
        Assert.Null(fieldProperties.BaseType);
    }

    [Fact]
    public void AsUnknown_WithNullableFalse_ReturnsUnknownNonNullableFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.AsUnknown(nullable: false);

        Assert.Equal("UNKNOWN", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
        Assert.Null(fieldProperties.Length);
        Assert.Null(fieldProperties.IsMax);
        Assert.Null(fieldProperties.IsUnicode);
        Assert.Null(fieldProperties.IsFixedLength);
        Assert.Null(fieldProperties.Precision);
        Assert.Null(fieldProperties.Scale);
        Assert.Null(fieldProperties.FractionalSecondsPrecision);
        Assert.Null(fieldProperties.BaseType);
    }

    [Fact]
    public void AsUnknown_WithNullableNull_UsesDefaultNullability()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.AsUnknown(nullable: null);

        Assert.Equal("UNKNOWN", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
    }
}