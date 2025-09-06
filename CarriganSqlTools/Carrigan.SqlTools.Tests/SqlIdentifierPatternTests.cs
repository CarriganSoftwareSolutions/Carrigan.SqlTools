namespace Carrigan.SqlTools.Tests;

public class SqlIdentifierPatternTests
{
    [Theory]
    [InlineData("UserRole")]
    [InlineData("_Role1")]
    [InlineData("@Admin")]
    [InlineData("#TempRole")]
    [InlineData("Role$Name")]
    public void ValidIdentifiers_ShouldPass(string identifier)
    {
        // Assert that the identifier passes the pattern.
        Assert.True(SqlIdentifierPattern.Passes(identifier));
        // And that it does not fail.
        Assert.False(SqlIdentifierPattern.Fails(identifier));
    }

    [Theory]
    [InlineData("Invalid Role")]           // Contains a space.
    [InlineData("Role; DROP TABLE Users")] // Contains SQL injection payload.
    [InlineData("User-Role")]              // Contains a hyphen.
    [InlineData("123Invalid")]             // Starts with a digit.
    [InlineData("")]                       // Empty string.
    [InlineData(" ")]                      // A string with just whitespace.
    public void InvalidIdentifiers_ShouldFail(string identifier)
    {
        // Assert that the identifier does not pass the pattern.
        Assert.False(SqlIdentifierPattern.Passes(identifier));
        // And that it does fail.
        Assert.True(SqlIdentifierPattern.Fails(identifier));
    }
}
