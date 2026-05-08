using Carrigan.SqlTools.Dialects.SqlServer;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Theory]
    [InlineData("Customer", "[Customer]")]
    [InlineData("Order", "[Order]")]
    [InlineData("Customer Name", "[Customer Name]")]
    public void QuoteIdentifier_ReturnsBracketedIdentifier(string identifier, string expected)
    {
        SqlServerDialect dialect = new();

        string actual = dialect.QuoteIdentifier(identifier);

        Assert.Equal(expected, actual);
    }
}