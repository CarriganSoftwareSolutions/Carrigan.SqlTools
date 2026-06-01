#pragma warning disable CS0618
using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsNText_ReturnsNText() =>
        AssertCharacter(SqlServerTypesProvider.AsNText(), "NTEXT", null, false, true, false);

    [Fact]
    public void AsNText_ReturnsNullable_WhenNullableTrue() =>
        AssertCharacter(SqlServerTypesProvider.AsNText(true), "NTEXT", null, false, true, false, true);
}
#pragma warning restore CS0618
