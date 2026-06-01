using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsNVarCharMax_ReturnsNVarCharMax() => 
        AssertCharacter(SqlServerTypesProvider.AsNVarCharMax(), "NVARCHAR", null, true, true, false);

    [Fact]
    public void AsNVarCharMax_ReturnsNullable_WhenNullableTrue() =>
        AssertCharacter(SqlServerTypesProvider.AsNVarCharMax(true), "NVARCHAR", null, true, true, false, true);
}
