using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsVarCharMax_ReturnsVarCharMax() => 
        AssertCharacter(SqlServerTypesProvider.AsVarCharMax(), "VARCHAR", null, true, false, false);

    [Fact]
    public void AsVarCharMax_ReturnsNullable_WhenNullableTrue() => 
        AssertCharacter(SqlServerTypesProvider.AsVarCharMax(true), "VARCHAR", null, true, false, false, true);
}
