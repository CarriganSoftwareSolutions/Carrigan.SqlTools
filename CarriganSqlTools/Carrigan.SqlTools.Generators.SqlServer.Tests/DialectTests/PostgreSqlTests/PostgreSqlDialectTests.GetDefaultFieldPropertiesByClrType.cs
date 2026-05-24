using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void GetDefaultFieldPropertiesByClrType_Int_ReturnsInteger()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(int));
        Assert.Equal("INTEGER", actual.ProviderTypeName);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_String_ReturnsText()
    {
        FieldProperties actual = Dialect.GetDefaultFieldPropertiesByClrType(typeof(string));
        Assert.Equal("TEXT", actual.ProviderTypeName);
    }
}
