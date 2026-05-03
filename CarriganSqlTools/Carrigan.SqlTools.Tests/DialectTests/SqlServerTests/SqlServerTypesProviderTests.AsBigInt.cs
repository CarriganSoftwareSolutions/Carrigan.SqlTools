using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsBigInt_ReturnsBigInt() => 
        AssertSimple(SqlServerTypesProvider.AsBigInt(), "BIGINT");

    [Fact]
    public void AsBigInt_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsBigInt(true), "BIGINT", true);
}
