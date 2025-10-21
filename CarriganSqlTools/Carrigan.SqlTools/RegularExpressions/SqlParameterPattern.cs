using Carrigan.Core.Extensions;
using System.Text.RegularExpressions;

namespace Carrigan.SqlTools.RegularExpressions;

//Ignore Spelling: Za Nl

/// <summary>
/// Validates SQL Server parameter names (e.g., <c>@CustomerId</c>) against identifier rules
/// adapted for T-SQL parameters.
/// </summary>
/// <remarks>
/// This library typically prepends the leading <c>@</c> itself, so parameter names passed
/// here may or may not start with <c>@</c>. To validate consistently:
/// <list type="bullet">
///   <item>We strip all leading <c>@</c> characters for the regex check.</item>
///   <item>We additionally reject names that begin with <c>@@</c> (reserved T-SQL “global” identifiers).</item>
///   <item>We require at least one non-<c>@</c> character after trimming.</item>
/// </list>
/// Unicode support is enabled via <c>\p{L}</c>, <c>\p{Nl}</c>, and <c>\p{Nd}</c>. Decomposed
/// Unicode forms are not validated—provide pre-normalized (Form C) identifiers.
/// Maximum length enforced is 128 characters (including any leading <c>@</c> characters).
/// </remarks>
public static partial class SqlParameterPattern
{
    /// <summary>
    /// Compiled regex that validates characters allowed in parameter names
    /// (after stripping leading <c>@</c> characters).
    /// </summary>
    /// <remarks>
    /// Pattern: <code>^(?:[_@#$]|\p{L}|\p{Nl}|\p{Nd})*$</code><br/>
    /// Allows underscore, <c>@</c>, <c>#</c>, <c>$</c>, letters (<c>\p{L}</c>), letter numbers (<c>\p{Nl}</c>),
    /// and decimal digits (<c>\p{Nd}</c>) anywhere in the name once trimmed.
    /// </remarks>
    private static readonly Regex _regexPattern = SqlParameterRegex();

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="identifier"/> is a valid SQL
    /// parameter name under the rules described in the remarks.
    /// </summary>
    /// <param name="identifier">The candidate parameter name (may or may not start with <c>@</c>).</param>
    /// <returns><see langword="true"/> if valid; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This method does not throw under normal circumstances:
    /// input is checked for <c>null</c>, and the regex is compiled and safe for typical inputs.
    /// </remarks>
    public static bool Passes(string? identifier) =>
         identifier is not null 
            && identifier.TrimStart('@').Length >= 1 
            && identifier.Length <= 128 
            && _regexPattern.IsMatch(identifier.TrimStart('@'))
            && identifier.StartsWith("@@") is false; //Only reserved words start with @@

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="identifier"/> is NOT valid
    /// per <see cref="Passes(string?)"/>.
    /// </summary>
    /// <param name="identifier">The candidate parameter name.</param>
    /// <returns><see langword="true"/> if invalid; otherwise, <see langword="false"/>.</returns>
    /// <remarks>No exceptions are expected.</remarks>
    public static bool Fails(string? identifier) =>
        Passes(identifier) is false;

    [GeneratedRegex(@"^(?:[_@#$]|\p{L}|\p{Nl}|\p{Nd})*$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex SqlParameterRegex();
}
