namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Theory]
    [InlineData("Customer", "[Customer]")]
    [InlineData("Order", "[Order]")]
    [InlineData("Customer Name", "[Customer Name]")]
    public void QuoteIdentifier_ReturnsBracketedIdentifier(string identifier, string expected)
    {
        string actual = Dialect.QuoteIdentifier(identifier);

        Assert.Equal(expected, actual);
    }
}