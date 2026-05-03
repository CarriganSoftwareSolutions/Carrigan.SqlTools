using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsTable_ReturnsTable() =>
        AssertSimple(SqlServerTypesProvider.AsTable(), "TABLE");

    [Fact]
    public void AsTable_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsTable(true), "TABLE", true);
}
