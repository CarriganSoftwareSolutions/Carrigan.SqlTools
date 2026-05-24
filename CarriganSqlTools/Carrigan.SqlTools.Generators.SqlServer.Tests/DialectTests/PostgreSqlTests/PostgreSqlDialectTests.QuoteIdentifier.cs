namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void QuoteIdentifier_SimpleIdentifier_ReturnsDoubleQuotedIdentifier()
    {
        string actual = Dialect.QuoteIdentifier("Customer");
        Assert.Equal("\"Customer\"", actual);
    }

    [Fact]
    public void QuoteIdentifier_IdentifierWithDoubleQuote_EscapesDoubleQuote()
    {
        string actual = Dialect.QuoteIdentifier("Customer\"Archive");
        Assert.Equal("\"Customer\"\"Archive\"", actual);
    }

    [Fact]
    public void QuoteIdentifier_NullIdentifier_Exception() =>
        Assert.Throws<ArgumentNullException>(() => Dialect.QuoteIdentifier(null!));
}
