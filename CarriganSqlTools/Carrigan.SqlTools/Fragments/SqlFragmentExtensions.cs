using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Extension methods for <see cref="ISqlFragment"/> sequences.
/// </summary>
internal static class SqlFragmentExtensions
{
    private static SqlFragmentParameter RenderFinalParameter(ISqlDialects dialect, SqlFragmentParameter parameter, int index)
    {
        string finalParameterName = dialect.RenderFinalParameterName(parameter.ParameterTag, index);
        return new (parameter, new ParameterTag (finalParameterName));
    }

    private static IEnumerable<ISqlFragment> RenderFinalFragmentEnumeration(this IEnumerable<ISqlFragment> sqlFragments, ISqlDialects dialect)
    {
        List<ISqlFragment> sqlFragmentsFinalForm = [];
        IEnumerable<ISqlFragment> flattenedSqlFragments = sqlFragments.Flatten();
        int j = 1; //start at 1 because PostgreSQL and SQL Lite use 1-based parameter indexing.
        for (int i = 0; i < flattenedSqlFragments.Count(); i++)
        {
            if (flattenedSqlFragments.ElementAt(i) is SqlFragmentParameter parameterFragment)
            {
                sqlFragmentsFinalForm.Add(RenderFinalParameter(dialect, parameterFragment, j++));
            }
            else
                sqlFragmentsFinalForm.Add(flattenedSqlFragments.ElementAt(i));
        }
        return sqlFragmentsFinalForm.AsEnumerable<ISqlFragment>();
    }

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
    /// <param name="dialect"></param>
    internal static IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(this IEnumerable<ISqlFragment> sqlFragments, ISqlDialects dialect)
    {
        ArgumentNullException.ThrowIfNull(sqlFragments);
        ArgumentNullException.ThrowIfNull(dialect);

        return sqlFragments
            .RenderFinalFragmentEnumeration(dialect)
            .SelectMany(fragment => fragment.GetSqlFragmentParameters());
    }

    /// <summary>
    /// Concatenates the SQL text representation of a sequence of fragments.
    /// </summary>
    /// <param name="sqlFragments">The sequence of fragments to render.</param>
    /// <returns>The concatenated SQL text.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlFragments"/> is <c>null</c>.
    /// </exception>
    /// <param name="dialect"></param>
    internal static string ToSql(this IEnumerable<ISqlFragment> sqlFragments, ISqlDialects dialect)
    {
        ArgumentNullException.ThrowIfNull(sqlFragments);
        ArgumentNullException.ThrowIfNull(dialect);     

        return string.Concat(sqlFragments.RenderFinalFragmentEnumeration(dialect).Select(fragment => fragment.ToSql(dialect)));
    }

    /// <summary>
    /// Joins a sequence of SQL fragments with an optional separator.
    /// </summary>
    /// <param name="fragments">The sequence of fragments to join.</param>
    /// <param name="separator">The optional separator to insert between fragments.</param>
    /// <returns>An enumerable of <see cref="ISqlFragment"/> representing the joined fragments.</returns>
    internal static IEnumerable<ISqlFragment> JoinFragments(this IEnumerable<ISqlFragment> fragments, ISqlFragment? separator = null)
    {
        ArgumentNullException.ThrowIfNull(fragments);

        bool first = true;

        foreach (ISqlFragment fragment in fragments)
        {
            if (!first && separator is not null)
            {
                yield return separator;
            }

            yield return fragment;
            first = false;
        }
    }
    /// <summary>
    /// Joins a sequence of SQL fragments with an optional separator.
    /// </summary>
    /// <param name="fragments">The sequence of fragments to join.</param>
    /// <param name="separator">The optional separator to insert between fragments.</param>
    /// <returns>An enumerable of <see cref="ISqlFragment"/> representing the joined fragments.</returns>
    internal static IEnumerable<ISqlFragment> JoinFragments(this IEnumerable<ISqlFragment> fragments, string separator) =>
        separator.IsNullOrEmpty()
            ? fragments.JoinFragments()
            : fragments.JoinFragments(new SqlFragmentText(separator));
            
    /// <summary>
    /// Flattens a sequence of SQL fragments by recursively expanding any nested sequences of fragments into a single, flat sequence.
    /// </summary>
    /// <param name="fragments">The sequence of fragments to flatten.</param>
    /// <returns>An enumerable collection of <see cref="ISqlFragment"/> objects representing the flattened structure of the input sequence.</returns>
    internal static IEnumerable<ISqlFragment> Flatten(this IEnumerable<ISqlFragment> fragments) =>
        fragments.SelectMany(element => element.Flatten());


    /// <summary>
    /// Returns an enumerable sequence containing the specified SQL fragments in order.
    /// </summary>
    /// <param name="fragment1">The first SQL fragment to include in the sequence.</param>
    /// <param name="fragment2">The second SQL fragment to include in the sequence.</param>
    /// <returns>An enumerable sequence of SQL fragments, with the first fragment followed by the second fragment.</returns>
    internal static IEnumerable<ISqlFragment> Append(this ISqlFragment fragment1, ISqlFragment fragment2)
    {
        yield return fragment1;
        yield return fragment2;
    }

    /// <summary>
    /// Returns a sequence that contains the specified initial fragment followed by the elements of the provided
    /// fragment collection.
    /// </summary>
    /// <param name="fragment1">The first fragment to include in the resulting sequence.</param>
    /// <param name="fragments">A collection of fragments to append after the initial fragment. Cannot be null.</param>
    /// <returns>An <see cref="IEnumerable{ISqlFragment}"/> containing the initial fragment followed by the elements of <paramref name="fragments"/>.</returns>
    internal static IEnumerable<ISqlFragment> Concat(this ISqlFragment fragment1, IEnumerable<ISqlFragment> fragments)
    {
        yield return fragment1;

        foreach (ISqlFragment fragment in fragments)
            yield return fragment;
    }

    /// <summary>
    /// Returns an enumerable sequence containing only this SQL fragment.
    /// </summary>
    /// <param name="fragment">The SQL fragment to include in the sequence.</param>
    /// <returns>An <see cref="IEnumerable{ISqlFragment}"/> containing only this fragment.</returns>
    internal static IEnumerable<ISqlFragment> AsEnumerable(this ISqlFragment fragment)
    {
        yield return fragment;
    }
}