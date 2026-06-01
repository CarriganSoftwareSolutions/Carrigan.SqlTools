using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsInt_ReturnsInt() => 
        AssertSimple(SqlServerTypesProvider.AsInt(), "INT");

    [Fact]
    public void AsInt_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsInt(true), "INT", true);
}
