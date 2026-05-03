using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsTinyInt_ReturnsTinyInt() => 
        AssertSimple(SqlServerTypesProvider.AsTinyInt(), "TINYINT");

    [Fact]
    public void AsTinyInt_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsTinyInt(true), "TINYINT", true);
}
