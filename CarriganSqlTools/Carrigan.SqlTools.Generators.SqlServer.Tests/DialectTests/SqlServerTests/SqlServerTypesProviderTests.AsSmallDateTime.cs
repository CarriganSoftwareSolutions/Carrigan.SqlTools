using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsSmallDateTime_ReturnsSmallDateTime() => 
        AssertTemporal(SqlServerTypesProvider.AsSmallDateTime(), "SMALLDATETIME");

    [Fact]
    public void AsSmallDateTime_ReturnsNullable_WhenNullableTrue() =>
        AssertTemporal(SqlServerTypesProvider.AsSmallDateTime(true), "SMALLDATETIME", expectedNullable: true);
}
