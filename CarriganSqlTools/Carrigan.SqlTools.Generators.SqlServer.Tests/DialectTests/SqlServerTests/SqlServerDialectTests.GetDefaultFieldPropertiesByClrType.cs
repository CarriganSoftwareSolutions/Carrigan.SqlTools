using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void GetDefaultFieldPropertiesByClrType_Int_ReturnsIntFieldProperties()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(int));

        Assert.Equal("INT", actual.ProviderTypeName);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_NullableInt_ReturnsNullableIntFieldProperties()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(int?));

        Assert.Equal("INT", actual.ProviderTypeName);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_String_ReturnsNVarCharMaxFieldProperties()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(string));

        Assert.Equal("NVARCHAR", actual.ProviderTypeName);
        Assert.True(actual.IsMax);
    }
}