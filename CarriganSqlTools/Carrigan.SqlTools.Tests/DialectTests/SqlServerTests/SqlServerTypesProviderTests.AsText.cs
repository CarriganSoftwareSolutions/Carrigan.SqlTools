#pragma warning disable CS0618
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsText_ReturnsText() => 
        AssertCharacter(SqlServerTypesProvider.AsText(), "TEXT", null, true, false, false);

    [Fact]
    public void AsText_ReturnsNullable_WhenNullableTrue() =>
        AssertCharacter(SqlServerTypesProvider.AsText(true), "TEXT", null, true, false, false, true);
}
#pragma warning restore CS0618
