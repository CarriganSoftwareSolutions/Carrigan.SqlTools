using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsGeography_ReturnsGeography() =>
        AssertSimple(SqlServerTypesProvider.AsGeography(), "GEOGRAPHY");

    [Fact]
    public void AsGeography_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsGeography(true), "GEOGRAPHY", true);
}
