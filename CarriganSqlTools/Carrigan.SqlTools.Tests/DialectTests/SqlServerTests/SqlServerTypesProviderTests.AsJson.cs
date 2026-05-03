using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsJson_ReturnsJson() => 
        AssertSimple(SqlServerTypesProvider.AsJson(), "JSON");

    [Fact]
    public void AsJson_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsJson(true), "JSON", true);
}
