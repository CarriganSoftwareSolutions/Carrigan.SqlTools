using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void GetDefaultFieldPropertiesByClrType_Int_ReturnsInteger()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(int));

        Assert.Equal("INTEGER", actual.ProviderTypeName);
        Assert.Equal((bool?)false, actual.IsArray);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_IntArray_ReturnsIntegerArray()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(int[]));

        Assert.Equal("INTEGER", actual.ProviderTypeName);
        Assert.Equal((bool?)true, actual.IsArray);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_String_ReturnsText()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(string));

        Assert.Equal("TEXT", actual.ProviderTypeName);
        Assert.Equal((bool?)false, actual.IsArray);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_StringArray_ReturnsTextArray()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(string[]));

        Assert.Equal("TEXT", actual.ProviderTypeName);
        Assert.Equal((bool?)true, actual.IsArray);
    }
}
