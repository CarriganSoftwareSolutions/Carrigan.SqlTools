#pragma warning disable CS0618
using Carrigan.SqlTools.Dialects;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsImage_ReturnsImage() =>
        AssertBinary(SqlServerTypesProvider.AsImage(), "IMAGE", null, false, false);

    [Fact]
    public void AsImage_ReturnsNullable_WhenNullableTrue() => 
        AssertBinary(SqlServerTypesProvider.AsImage(true), "IMAGE", null, false, false, true);
}
#pragma warning restore CS0618
