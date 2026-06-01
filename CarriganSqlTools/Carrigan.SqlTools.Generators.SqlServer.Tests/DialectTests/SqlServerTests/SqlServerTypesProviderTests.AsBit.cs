using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsBit_ReturnsBit() => 
        AssertSimple(SqlServerTypesProvider.AsBit(), "BIT");

    [Fact]
    public void AsBit_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsBit(true), "BIT", true);
}
