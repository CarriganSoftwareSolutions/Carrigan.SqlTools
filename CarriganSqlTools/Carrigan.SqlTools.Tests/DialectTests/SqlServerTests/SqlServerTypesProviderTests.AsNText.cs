#pragma warning disable CS0618
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsNText_ReturnsNText() =>
        AssertCharacter(SqlServerTypesProvider.AsNText(), "NTEXT", null, true, true, false);

    [Fact]
    public void AsNText_ReturnsNullable_WhenNullableTrue() =>
        AssertCharacter(SqlServerTypesProvider.AsNText(true), "NTEXT", null, true, true, false, true);
}
#pragma warning restore CS0618
