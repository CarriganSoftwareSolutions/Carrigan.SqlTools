using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsXml_ReturnsXml() => 
        AssertSimple(SqlServerTypesProvider.AsXml(), "XML");

    [Fact]
    public void AsXml_ReturnsNullable_WhenNullableTrue() => AssertSimple(SqlServerTypesProvider.AsXml(true), "XML", true);
}
