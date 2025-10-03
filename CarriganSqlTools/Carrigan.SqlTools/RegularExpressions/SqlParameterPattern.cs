using Carrigan.Core.Extensions;
using System.Text.RegularExpressions;

namespace Carrigan.SqlTools.RegularExpressions;

//Ignore Spelling: Za Nl

//TODO: proof read documentation
/// <summary>
/// Provides helper methods to validate SQL Server Parameters names against
/// SQL Server’s identifier naming rules for Parameters
/// Support for Unicode validation has been added. I am not 100% on the rules.
/// Decomposed normalizations of Unicode will be failed, as I don't know
/// what normalization method any given database may be using.
/// So please provide pre-normalized identifier names.
/// </summary>
public static partial class SqlParameterPattern
{
    /// <summary>
    /// Provides helper methods to validate SQL Server Parameters names against
    /// SQL Server’s identifier naming rules for Parameters
    /// Support for Unicode validation has been added. I am not 100% on the rules.
    /// Decomposed normalizations of Unicode will be failed, as I don't know
    /// what normalization method any given database may be using.
    /// So please provide pre-normalized identifier names.
    /// </summary>
    private static readonly Regex _regexPattern = SqlParameterRegex();

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> complies
    /// with SQL Server Parameter naming rules.
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
         identifier is not null && identifier.TrimStart('@').Length >= 1 && identifier.Length <= 128 && _regexPattern.IsMatch(identifier.TrimStart('@'));

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> violates
    /// with SQL Server Parameter naming rules.
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

    [GeneratedRegex(@"^(?:[_@#$]|\p{L}|\p{Nl}|\p{Nd})*$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex SqlParameterRegex();
}
