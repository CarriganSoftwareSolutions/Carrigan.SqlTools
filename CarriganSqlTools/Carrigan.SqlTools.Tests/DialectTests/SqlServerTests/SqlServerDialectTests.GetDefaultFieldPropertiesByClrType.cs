using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void GetDefaultFieldPropertiesByClrType_Int_ReturnsIntFieldProperties()
    {
        SqlServerDialect dialect = new();

        FieldProperties actual = dialect.GetDefaultFieldPropertiesByClrType(typeof(int));

        Assert.Equal("INT", actual.ProviderTypeName);
        Assert.False(actual.IsNullable);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_NullableInt_ReturnsNullableIntFieldProperties()
    {
        SqlServerDialect dialect = new();

        FieldProperties actual = dialect.GetDefaultFieldPropertiesByClrType(typeof(int?));

        Assert.Equal("INT", actual.ProviderTypeName);
        Assert.True(actual.IsNullable);
    }

    [Fact]
    public void GetDefaultFieldPropertiesByClrType_String_ReturnsNVarCharMaxFieldProperties()
    {
        SqlServerDialect dialect = new();

        FieldProperties actual = dialect.GetDefaultFieldPropertiesByClrType(typeof(string));

        Assert.Equal("NVARCHAR", actual.ProviderTypeName);
        Assert.True(actual.IsMax);
    }
}