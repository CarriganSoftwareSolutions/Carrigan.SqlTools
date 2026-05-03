#pragma warning disable CS0618
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsTimestamp_ReturnsTimestamp() => 
        AssertBinary(SqlServerTypesProvider.AsTimestamp(), "TIMESTAMP", 8, false, true);

    [Fact]
    public void AsTimestamp_ReturnsNullable_WhenNullableTrue() => 
        AssertBinary(SqlServerTypesProvider.AsTimestamp(true), "TIMESTAMP", 8, false, true, true);
}
#pragma warning restore CS0618
