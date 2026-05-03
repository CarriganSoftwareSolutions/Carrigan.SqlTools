using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsSmallMoney_ReturnsSmallMoney() => 
        AssertSimple(SqlServerTypesProvider.AsSmallMoney(), "SMALLMONEY");

    [Fact]
    public void AsSmallMoney_ReturnsNullable_WhenNullableTrue() => 
        AssertSimple(SqlServerTypesProvider.AsSmallMoney(true), "SMALLMONEY", true);
}
