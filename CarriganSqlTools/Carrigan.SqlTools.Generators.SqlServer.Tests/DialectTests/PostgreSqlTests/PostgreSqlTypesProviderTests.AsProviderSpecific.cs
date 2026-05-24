using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsProviderSpecific_ValidProviderTypeName_ReturnsProviderSpecificType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsProviderSpecific("citext");

        AssertFieldProperties(actual, "CITEXT");
    }

    [Fact]
    public void AsProviderSpecific_NullableTrue_ReturnsNullableProviderSpecificType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsProviderSpecific("inet", true);

        AssertFieldProperties(actual, "INET", isNullable: true);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AsProviderSpecific_InvalidProviderTypeName_Exception(string? providerTypeName)
    {
        ArgumentException argumentException = Assert.Throws<ArgumentException>(() => PostgreSqlTypesProvider.AsProviderSpecific(providerTypeName!));
    }

    [Fact]
    public void AsProviderSpecific_InvalidProviderTypeName_NullException() => 
        Assert.Throws<ArgumentNullException>(() => PostgreSqlTypesProvider.AsProviderSpecific(null!));
}
