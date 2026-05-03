#pragma warning disable CS0618
using Carrigan.SqlTools.Dialects.SqlServer;
using Xunit;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerTypesProviderTests
{
    [Fact]
    public void AsImage_ReturnsImage() =>
        AssertBinary(SqlServerTypesProvider.AsImage(), "IMAGE", null, true, false);

    [Fact]
    public void AsImage_ReturnsNullable_WhenNullableTrue() => 
        AssertBinary(SqlServerTypesProvider.AsImage(true), "IMAGE", null, true, false, true);
}
#pragma warning restore CS0618
