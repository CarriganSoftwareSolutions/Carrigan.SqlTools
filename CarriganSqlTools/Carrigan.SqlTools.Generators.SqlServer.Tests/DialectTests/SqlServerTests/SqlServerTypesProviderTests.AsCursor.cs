using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsCursor_ReturnsCursor() => 
        AssertSimple(SqlServerTypesProvider.AsCursor(), "CURSOR");

    [Fact]
    public void AsCursor_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsCursor(true), "CURSOR", true);
}
