using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsDateTime_ReturnsDateTime() =>
        AssertTemporal(SqlServerTypesProvider.AsDateTime(), "DATETIME");

    [Fact]
    public void AsDateTime_ReturnsNullable_WhenNullableTrue() => 
        AssertTemporal(SqlServerTypesProvider.AsDateTime(true), "DATETIME", expectedNullable: true);
}
