using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsXml_ReturnsXml() => 
        AssertSimple(SqlServerTypesProvider.AsXml(), "XML");

    [Fact]
    public void AsXml_ReturnsNullable_WhenNullableTrue() => AssertSimple(SqlServerTypesProvider.AsXml(true), "XML", true);
}
