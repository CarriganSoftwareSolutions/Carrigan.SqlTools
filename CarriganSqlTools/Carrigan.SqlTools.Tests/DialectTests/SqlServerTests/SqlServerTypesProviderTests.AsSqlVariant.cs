using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsSqlVariant_ReturnsSqlVariant() =>
        AssertSimple(SqlServerTypesProvider.AsSqlVariant(), "SQL_VARIANT");

    [Fact]
    public void AsSqlVariant_ReturnsNullable_WhenNullableTrue() =>
        AssertSimple(SqlServerTypesProvider.AsSqlVariant(true), "SQL_VARIANT", true);
}
