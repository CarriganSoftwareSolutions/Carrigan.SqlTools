using System.Text.RegularExpressions;

namespace Carrigan.SqlTools;

//Ignore Spelling: Za

/// <summary>
/// Pattern match used to check certain identifier names.
/// </summary>
public static class SqlIdentifierPattern
{
    /// <summary>
    /// Defines the pattern to check for, based on the SQL Server Identifier naming rules.
    /// </summary>
    private static readonly string _pattern = @"^[A-Za-z_@#][A-Za-z0-9_@$#]*$";
    /// <summary>
    /// Determine if <see <paramref name="identifier"/> passes the test.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns>true if  <see <paramref name="identifier"/> passes the test. Otherwise false</returns>
    public static bool Passes(string identifier) =>
         Regex.IsMatch(identifier, _pattern);
    /// <summary>
    /// Determine if <see <paramref name="identifier"/> failed the test.
    /// </summary>
    /// <returns>true if  <see <paramref name="identifier"/> fails the test. Otherwise false</returns>
    public static bool Fails(string identifier) =>
         Regex.IsMatch(identifier, _pattern) == false;
}
