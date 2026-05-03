using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsBit_ReturnsBit() => 
        AssertSimple(SqlServerTypesProvider.AsBit(), "BIT");

    [Fact]
    public void AsBit_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsBit(true), "BIT", true);
}
