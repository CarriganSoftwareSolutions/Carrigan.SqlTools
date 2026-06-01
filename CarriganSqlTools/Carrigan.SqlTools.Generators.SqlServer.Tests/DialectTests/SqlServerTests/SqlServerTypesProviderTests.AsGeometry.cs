using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsGeometry_ReturnsGeometry() => 
        AssertSimple(SqlServerTypesProvider.AsGeometry(), "GEOMETRY");

    [Fact]
    public void AsGeometry_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsGeometry(true), "GEOMETRY", true);
}
