using Carrigan.Core.Extensions;
using System.Text.RegularExpressions;

namespace Carrigan.SqlTools.RegularExpressions;

//Ignore Spelling: Za Nl

/// <summary>
/// Provides helper methods to validate SQL Server identifier names
/// (e.g., table names, column names, alias names) against
/// SQL Server’s identifier naming rules.
/// </summary>
/// <remarks>
/// Unicode support has been added using <c>\p{L}</c> and <c>\p{Nl}</c> categories
/// to allow letters and letter numbers.  
/// Decomposed Unicode sequences are intentionally disallowed to avoid
/// unpredictable normalization behavior between database configurations.  
/// **Provide pre-normalized identifier names.**
/// </remarks>
public static partial class SqlIdentifierPattern
{
    /// <summary>
    /// The regular expression pattern that enforces SQL Server identifier rules:
    /// must begin with a letter, underscore, <c>@</c>, or <c>#</c> and may contain letters,
    /// digits, underscores, <c>@</c>, <c>$</c>, or <c>#</c> thereafter.
    /// </summary>
    private static readonly Regex _regexPattern = SqlIdentifierRegex();

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> complies
    /// with SQL Server identifier naming rules.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="identifier"/> matches the SQL Server
    /// naming pattern; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="RegexMatchTimeoutException">
    /// Thrown if the regular expression evaluation exceeds its internal match timeout.
    /// </exception>
    public static bool Passes(string? identifier) =>
         identifier is not null && identifier.Length >= 1 && identifier.Length <= 128 && _regexPattern.IsMatch(identifier);

    /// <summary>
    /// Determines whether the specified <paramref name="identifier"/> violates
    /// SQL Server identifier naming rules.
    /// </summary>
    /// <param name="identifier">The identifier name to validate.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="identifier"/> does <em>not</em> match
    /// the SQL Server naming pattern; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="RegexMatchTimeoutException">
    /// Thrown if the regular expression evaluation exceeds its internal match timeout.
    /// </exception>
    public static bool Fails(string? identifier) =>
        Passes(identifier) is false;

    /// <summary>
    /// Defines the compiled regular expression used for identifier validation.
    /// </summary>
    /// <remarks>
    /// The expression:
    /// <code>
    /// ^(?:[_#]|\p{L}|\p{Nl})(?:[_@#$]|\p{L}|\p{Nl}|\p{Nd})*$
    /// </code>
    /// - **Start Character:** underscore (<c>_</c>), hash (<c>#</c>), letter (<c>\p{L}</c>), or letter number (<c>\p{Nl}</c>)  
    /// - **Subsequent Characters:** underscore, at-sign (<c>@</c>), hash, dollar sign (<c>$</c>), letter, letter number, or decimal digit (<c>\p{Nd}</c>)
    /// </remarks>
    [GeneratedRegex(@"^(?:[_#]|\p{L}|\p{Nl})(?:[_@#$]|\p{L}|\p{Nl}|\p{Nd})*$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex SqlIdentifierRegex();
}
