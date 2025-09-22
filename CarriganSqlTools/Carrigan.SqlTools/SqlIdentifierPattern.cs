using System.Text.RegularExpressions;

namespace Carrigan.SqlTools;

//Ignore Spelling: Za

/// <summary>
/// Provides helper methods to validate SQL Server identifier names
/// (e.g., table names, column names, parameter names) against
/// SQL Server’s identifier naming rules.
/// </summary>
public static class SqlIdentifierPattern
{
    /// <summary>
    /// The regular expression pattern that enforces SQL Server identifier rules:
    /// must begin with a letter, underscore, @, or # and may contain letters,
    /// digits, underscores, @, $, or # thereafter.
    /// </summary>
    private static readonly string _pattern = @"^[A-Za-z_@#][A-Za-z0-9_@$#]*$";

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> complies
    /// with SQL Server identifier naming rules.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="identifier"/> matches the SQL Server
    /// naming pattern; otherwise, <c>false</c>.
    /// </returns>
    public static bool Passes(string identifier) =>
         Regex.IsMatch(identifier, _pattern);

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> violates
    /// SQL Server identifier naming rules.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="identifier"/> does not match the SQL Server
    /// naming pattern; otherwise, <c>false</c>.
    /// </returns>
    public static bool Fails(string identifier) =>
         Regex.IsMatch(identifier, _pattern) == false;
}
