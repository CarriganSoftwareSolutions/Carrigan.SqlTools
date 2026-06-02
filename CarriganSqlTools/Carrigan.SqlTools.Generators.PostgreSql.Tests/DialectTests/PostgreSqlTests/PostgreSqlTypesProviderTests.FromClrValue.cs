using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    private static readonly string[] firstSecondName = ["first", "second"];
    private static readonly int[] oneTwoThree = [1, 2, 3];

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
        Assert.Null(fieldProperties.IsArray);
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
        Assert.Null(fieldProperties.IsArray);
    }

    [Fact]
    public void FromClrValue_WithIntegerValue_ReturnsIntegerFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(123);

        Assert.Equal("INTEGER", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal((bool?)false, fieldProperties.IsArray);
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
        Assert.Equal((bool?)false, fieldProperties.IsArray);
    }

    [Fact]
    public void FromClrValue_WithGuidValue_ReturnsUuidFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(Guid.NewGuid());

        Assert.Equal("UUID", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal((bool?)false, fieldProperties.IsArray);
    }

    [Fact]
    public void FromClrValue_WithByteArrayValue_ReturnsByteaScalarFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(new byte[] { 1, 2, 3 });

        Assert.Equal("BYTEA", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal((bool?)false, fieldProperties.IsArray);
    }

    [Fact]
    public void FromClrValue_WithByteArrayArrayValue_ReturnsByteaArrayFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(new byte[][] { [1, 2], [3, 4] });

        Assert.Equal("BYTEA", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal((bool?)true, fieldProperties.IsArray);
    }

    [Fact]
    public void FromClrValue_WithIntegerArrayValue_ReturnsIntegerArrayFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(oneTwoThree);

        Assert.Equal("INTEGER", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal((bool?)true, fieldProperties.IsArray);
    }

    [Fact]
    public void FromClrValue_WithStringArrayValue_ReturnsTextArrayFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(firstSecondName);

        Assert.Equal("TEXT", fieldProperties.ProviderTypeName);
        Assert.True(fieldProperties.IsMax);
        Assert.True(fieldProperties.IsUnicode);
        Assert.False(fieldProperties.IsFixedLength);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal((bool?)true, fieldProperties.IsArray);
    }

    [Fact]
    public void FromClrValue_WithGuidArrayValue_ReturnsUuidArrayFieldProperties()
    {
        FieldProperties fieldProperties = PostgreSqlTypesProvider.FromClrValue(new[] { Guid.NewGuid(), Guid.NewGuid() });

        Assert.Equal("UUID", fieldProperties.ProviderTypeName);
        Assert.False(fieldProperties.IsNullable);
        Assert.Equal((bool?)true, fieldProperties.IsArray);
    }

}