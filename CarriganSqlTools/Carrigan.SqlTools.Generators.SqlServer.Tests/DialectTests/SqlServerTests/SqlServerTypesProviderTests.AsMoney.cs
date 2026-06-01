using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsMoney_ReturnsMoney() => 
        AssertSimple(SqlServerTypesProvider.AsMoney(), "MONEY");

    [Fact]
    public void AsMoney_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsMoney(true), "MONEY", true);
}
