using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Extension methods for <see cref="ISqlFragment"/> sequences.
/// </summary>
internal static class SqlFragmentExtensions
{
    /// <summary>
    /// Creates a parameter fragment whose name has been converted to the dialect's final positional or named form.
    /// </summary>
    /// <param name="dialect">The SQL dialect that renders final parameter names.</param>
    /// <param name="parameter">The original parameter fragment.</param>
    /// <param name="index">The 1-based parameter index used by dialects with positional parameters.</param>
    /// <returns>A parameter fragment using the final rendered parameter name.</returns>
    private static SqlFragmentParameter RenderFinalParameter(ISqlDialects dialect, SqlFragmentParameter parameter, int index)
    {
        string finalParameterName = dialect.RenderFinalParameterName(parameter.ParameterTag, index);
        return new (parameter, new ParameterTag (finalParameterName));
    }

    /// <summary>
    /// Flattens a fragment sequence and rewrites each parameter fragment into its dialect-specific final form.
    /// </summary>
    /// <param name="sqlFragments">The SQL fragments to flatten and render.</param>
    /// <param name="dialect">The SQL dialect used to render final parameter names.</param>
    /// <returns>The flattened fragments with finalized parameter names.</returns>
    private static IEnumerable<ISqlFragment> RenderFinalFragmentEnumeration(this IEnumerable<ISqlFragment> sqlFragments, ISqlDialects dialect)
    {
        List<ISqlFragment> sqlFragmentsFinalForm = [];
        IEnumerable<ISqlFragment> flattenedSqlFragments = sqlFragments.Flatten();
        int j = 1; // start at 1 because PostgreSQL and SQLite use 1-based parameter indexing.
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
    /// The parameter fragments contained by the SQL fragment tree after dialect-specific parameter names are assigned.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sqlFragments"/> is <c>null</c>.
    /// </exception>
    /// <param name="dialect">The SQL dialect used to assign final parameter names.</param>
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
    /// <param name="dialect">The SQL dialect used to render identifiers, literals, and final parameter names.</param>
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