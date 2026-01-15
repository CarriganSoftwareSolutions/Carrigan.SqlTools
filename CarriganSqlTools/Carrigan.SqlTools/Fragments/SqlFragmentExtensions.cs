using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Provides helper methods for working with collections of <see cref="SqlFragment"/>.
/// </summary>
internal static class SqlFragmentExtensions
{
    /// <summary>
    /// Extracts parameters from the provided SQL fragments and returns them as a flattened dictionary.
    /// </summary>
    /// <param name="sqlFragments">The fragment sequence to analyze.</param>
    /// <returns>
    /// A dictionary mapping each emitted <see cref="ParameterTag"/> to its runtime value.
    /// Null values are normalized to <see cref="DBNull.Value"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="sqlFragments"/> contains a <c>null</c> fragment, or when duplicate
    /// <see cref="ParameterTag"/> keys are encountered.
    /// </exception>
    internal static Dictionary<ParameterTag, object> GetParameters(this IEnumerable<SqlFragment> sqlFragments) =>
        sqlFragments
            .OfType<SqlFragmentParameter>()
            .Select(fragment => fragment.Parameter)
            .ToDictionary(pair => pair.Name, pair => pair.Value ?? DBNull.Value);

    /// <summary>
    /// Converts the provided SQL fragments to a single SQL string.
    /// </summary>
    /// <param name="sqlFragments">The fragment sequence to concatenate.</param>
    /// <returns>A concatenated SQL string produced by calling <see cref="SqlFragment.ToSql"/> on each fragment.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="sqlFragments"/> contains a <c>null</c> fragment.</exception>
    internal static string ToSql(this IEnumerable<SqlFragment> sqlFragments) =>
        string.Concat(sqlFragments.Select(fragment => fragment.ToSql()));
}
