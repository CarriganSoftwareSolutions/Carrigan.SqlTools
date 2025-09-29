using Carrigan.Core.Extensions;
using System.Text.RegularExpressions;

namespace Carrigan.SqlTools.RegularExpressions;

//Ignore Spelling: Za Nl
//TODO: proof read documentation
/// <summary>
/// Provides helper methods to validate SQL Server identifier names
/// (e.g., table names, column names, parameter names) against
/// SQL Server’s identifier naming rules.
/// Support for Unicode validation has been added. I am not 100% on the rules.
/// Decomposed normalizations of Unicode will be failed, as I don't know
/// what normalization method any given database may be using.
/// So please provide pre-normalized identifier names.
/// </summary>
public static class SqlIdentifierPattern
{
    /// <summary>
    /// The regular expression pattern that enforces SQL Server identifier rules:
    /// must begin with a letter, underscore, @, or # and may contain letters,
    /// digits, underscores, @, $, or # thereafter.
    /// Support for Unicode validation has been added. I am not 100% on the rules.
    /// Decomposed normalizations of Unicode will be failed, as I don't know
    /// what normalization method any given database may be using.
    /// So please provide pre-normalized identifier names.
    /// </summary>
    private static readonly Regex _regexPattern = new
    (
        @"^(?:[_#]|\p{L}|\p{Nl})(?:[_@#$]|\p{L}|\p{Nl}|\p{Nd})*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> complies
    /// with SQL Server identifier naming rules.
    /// Support for Unicode validation has been added. I am not 100% on the rules.
    /// Decomposed normalizations of Unicode will be failed, as I don't know
    /// what normalization method any given database may be using.
    /// So please provide pre-normalized identifier names.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="identifier"/> matches the SQL Server
    /// naming pattern; otherwise, <c>false</c>.
    /// </returns>
    public static bool Passes(string? identifier) =>
         identifier is not null && identifier.Length <= 128 && _regexPattern.IsMatch(identifier);

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> violates
    /// SQL Server identifier naming rules.
    /// Support for Unicode validation has been added. I am not 100% on the rules.
    /// Decomposed normalizations of Unicode will be failed, as I don't know
    /// what normalization method any given database may be using.
    /// So please provide pre-normalized identifier names.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="identifier"/> does not match the SQL Server
    /// naming pattern; otherwise, <c>false</c>.
    /// </returns>
    public static bool Fails(string? identifier) =>
        Passes(identifier) is false;
}
