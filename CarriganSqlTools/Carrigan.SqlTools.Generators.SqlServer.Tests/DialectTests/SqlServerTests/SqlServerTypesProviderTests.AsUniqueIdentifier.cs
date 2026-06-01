using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsUniqueIdentifier_ReturnsUniqueIdentifier() => 
        AssertSimple(SqlServerTypesProvider.AsUniqueIdentifier(), "UNIQUEIDENTIFIER");

    [Fact]
    public void AsUniqueIdentifier_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsUniqueIdentifier(true), "UNIQUEIDENTIFIER", true);
}
