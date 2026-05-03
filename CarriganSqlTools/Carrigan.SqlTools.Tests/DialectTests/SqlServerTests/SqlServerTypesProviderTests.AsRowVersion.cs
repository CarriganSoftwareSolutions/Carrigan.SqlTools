using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsRowVersion_ReturnsRowVersion() => 
        AssertBinary(SqlServerTypesProvider.AsRowVersion(), "ROWVERSION", 8, false, true);

    [Fact]
    public void AsRowVersion_ReturnsNullable_WhenNullableTrue() => 
        AssertBinary(SqlServerTypesProvider.AsRowVersion(true), "ROWVERSION", 8, false, true, true);
}
