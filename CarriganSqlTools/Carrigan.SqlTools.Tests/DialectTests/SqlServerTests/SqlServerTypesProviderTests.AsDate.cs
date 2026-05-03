using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsDate_ReturnsDate() => 
        AssertTemporal(SqlServerTypesProvider.AsDate(), "DATE");

    [Fact]
    public void AsDate_ReturnsNullable_WhenNullableTrue() => 
        AssertTemporal(SqlServerTypesProvider.AsDate(true), "DATE", expectedNullable: true);
}
