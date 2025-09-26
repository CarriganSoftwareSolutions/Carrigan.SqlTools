using Carrigan.Core.Extensions;
using System.Text.RegularExpressions;

namespace Carrigan.SqlTools;

//Ignore Spelling: Za

/// <summary>
/// Provides helper methods to validate SQL Server identifier names
/// (e.g., table names, column names, parameter names) against
/// SQL Server’s identifier naming rules.
/// </summary>
public static class SqlIdentifierNullablePattern
{
    /// <summary>
    /// The regular expression pattern that enforces SQL Server identifier rules:
    /// must begin with a letter, underscore, @, or # and may contain letters,
    /// digits, underscores, @, $, or # thereafter. Allows null or empty.
    /// </summary>
    private static readonly string _pattern = @"^(?=.{1,128}$)[\p{L}_][\p{L}\p{N}_@$#]*$";

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> complies
    /// with SQL Server identifier naming rules.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="identifier"/> matches the SQL Server
    /// naming pattern; otherwise, <c>false</c>.
    /// </returns>
    public static bool Passes(string? identifier) =>
         //WHITE SPACE IS STILL NOT VALID HERE
         identifier.IsNullOrEmpty() || Regex.IsMatch(identifier, _pattern);

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> violates
    /// SQL Server identifier naming rules.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="identifier"/> does not match the SQL Server
    /// naming pattern; otherwise, <c>false</c>.
    /// </returns>
    public static bool Fails(string? identifier) =>
        //WHITE SPACE IS STILL NOT VALID HERE
        identifier.IsNullOrEmpty() || Regex.IsMatch(identifier, _pattern) == false;
}
