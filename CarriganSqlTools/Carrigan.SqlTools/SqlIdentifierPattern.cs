using System.Text.RegularExpressions;

namespace Carrigan.SqlTools;

//Ignore Spelling: Za

/// <summary>
/// Pattern match used to check certain identifier names.
/// </summary>
public static class SqlIdentifierPattern
{
    private static readonly string _pattern = @"^[A-Za-z_@#][A-Za-z0-9_@$#]*$";
    public static bool Passes(string identifier) =>
         Regex.IsMatch(identifier, _pattern);
    public static bool Fails(string identifier) =>
         Regex.IsMatch(identifier, _pattern) == false;
}
