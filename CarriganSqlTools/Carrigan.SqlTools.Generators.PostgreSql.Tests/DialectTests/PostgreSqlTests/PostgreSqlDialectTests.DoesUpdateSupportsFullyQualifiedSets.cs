namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void DoesUpdateSupportsFullyQualifiedSets_ReturnsFalse()
    {
        bool actual = Dialect.DoesUpdateSupportsFullyQualifiedSets();

        Assert.False(actual);
    }
}
