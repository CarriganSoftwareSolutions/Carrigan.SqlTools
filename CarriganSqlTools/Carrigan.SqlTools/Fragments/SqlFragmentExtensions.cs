using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Extension methods for <see cref="SqlFragment"/> sequences.
/// </summary>
internal static class SqlFragmentExtensions
{
    /// <summary>
    /// Collects all SQL parameters from a sequence of SQL fragments.
    /// </summary>
    /// <param name="sqlFragments">
    /// The sequence of fragments to scan for <see cref="SqlFragmentParameter"/> entries.
    /// </param>
    /// <returns>
    /// A dictionary mapping each <see cref="ParameterTag"/> to its runtime value. Null values are materialized as
    /// <see cref="DBNull.Value"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlFragments"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when duplicate <see cref="ParameterTag"/> keys are encountered.
    /// </exception>
    internal static Dictionary<ParameterTag, object> GetParameters(this IEnumerable<SqlFragment> sqlFragments)
    {
        ArgumentNullException.ThrowIfNull(sqlFragments);

        return sqlFragments
            .SelectMany(fragment => fragment.GetParameters())
            .ToDictionary(static parameter => parameter.Name, static parameter => parameter.Value ?? DBNull.Value);
    }

    /// <summary>
    /// Concatenates the SQL text representation of a sequence of fragments.
    /// </summary>
    /// <param name="sqlFragments">The sequence of fragments to render.</param>
    /// <returns>The concatenated SQL text.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlFragments"/> is <c>null</c>.
    /// </exception>
    internal static string ToSql(this IEnumerable<SqlFragment> sqlFragments)
    {
        ArgumentNullException.ThrowIfNull(sqlFragments);

        return string.Concat(sqlFragments.Select(static fragment => fragment.ToSql()));
    }

    /// <summary>
    /// Joins a sequence of SQL fragments with an optional separator.
    /// </summary>
    /// <param name="fragments">The sequence of fragments to join.</param>
    /// <param name="separator">The optional separator to insert between fragments.</param>
    /// <returns>An enumerable of <see cref="SqlFragment"/> representing the joined fragments.</returns>
    internal static IEnumerable<SqlFragment> JoinFragments(this IEnumerable<SqlFragment> fragments, SqlFragment? separator = null)
    {
        ArgumentNullException.ThrowIfNull(fragments);

        bool first = true;

        foreach (SqlFragment fragment in fragments)
        {
            if (!first && separator is not null)
            {
                yield return separator;
            }

            yield return fragment;
            first = false;
        }
    }
}