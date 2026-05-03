using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsSmallInt_ReturnsSmallInt() =>
        AssertSimple(SqlServerTypesProvider.AsSmallInt(), "SMALLINT");

    [Fact]
    public void AsSmallInt_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsSmallInt(true), "SMALLINT", true);
}
