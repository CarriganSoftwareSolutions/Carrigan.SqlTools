using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsVarBinaryMax_ReturnsVarBinaryMax() => 
        AssertBinary(SqlServerTypesProvider.AsVarBinaryMax(), "VARBINARY", null, true, false);

    [Fact]
    public void AsVarBinaryMax_ReturnsNullable_WhenNullableTrue() => 
        AssertBinary(SqlServerTypesProvider.AsVarBinaryMax(true), "VARBINARY", null, true, false, true);
}
