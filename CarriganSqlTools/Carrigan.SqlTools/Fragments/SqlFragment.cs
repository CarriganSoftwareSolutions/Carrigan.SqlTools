namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents fragments of an SQL statement
/// </summary>
internal abstract class SqlFragment
{
    /// <summary>
    /// Parses the fragment to string.
    /// </summary>
    /// <returns>returns a string representing the fragment's SQL</returns>
    internal abstract string ToSql();

    /// <summary>
    /// Turns ToString into an alias for ToSql
    /// </summary>
    /// <returns>returns a string representing the fragment's SQL</returns>
    public override string ToString() =>
        ToSql();
}
