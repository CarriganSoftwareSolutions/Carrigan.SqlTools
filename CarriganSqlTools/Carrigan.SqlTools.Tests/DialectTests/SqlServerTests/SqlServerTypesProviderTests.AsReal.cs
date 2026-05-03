using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsReal_ReturnsReal() => 
        AssertSimple(SqlServerTypesProvider.AsReal(), "REAL");

    [Fact]
    public void AsReal_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsReal(true), "REAL", true);
}
