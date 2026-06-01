namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void DoesUpdateSupportsFullyQualifiedSets_ReturnsTrue()
    {

        bool actual = Dialect.DoesUpdateSupportsFullyQualifiedSets();

        Assert.True(actual);
    }
}
