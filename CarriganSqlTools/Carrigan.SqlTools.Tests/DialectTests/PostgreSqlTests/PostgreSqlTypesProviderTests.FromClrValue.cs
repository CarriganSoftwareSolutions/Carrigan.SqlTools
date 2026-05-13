using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void FromClrValue_WithNull_ReturnsNullableUnknownFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(null);

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
    public void FromClrValue_WithDbNull_ReturnsNullableUnknownFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(DBNull.Value);

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
    public void FromClrValue_WithIntegerValue_ReturnsIntegerFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(123);

        Assert.Equal("INTEGER", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
    }

    [Fact]
    public void FromClrValue_WithStringValue_ReturnsTextFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue("test value");

        Assert.Equal("TEXT", fieldProperties.ProviderTypeName);
        Assert.True(fieldProperties.IsMax);
        Assert.True(fieldProperties.IsUnicode);
        Assert.False(fieldProperties.IsFixedLength);
        Assert.False(fieldProperties.IsNullable);
    }

    [Fact]
    public void FromClrValue_WithGuidValue_ReturnsUuidFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(Guid.NewGuid());

        Assert.Equal("UUID", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
    }
}