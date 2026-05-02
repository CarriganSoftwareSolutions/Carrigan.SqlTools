using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Extension methods for <see cref="SqlFragment"/> sequences.
/// </summary>
internal static class SqlFragmentExtensions
{
    private static Parameter RenderFinalParameter(ISqlDialects dialect, Parameter parameter, int index)
    {
        string finalParameterName = dialect.RenderFinalParameterName(parameter.Name, index);
        return new Parameter(new ParameterTag(finalParameterName, parameter.Name.SqlType), parameter.Value);
    }

    private static IEnumerable<SqlFragment> RenderFinalFragmentEnumeration(this IEnumerable<SqlFragment> sqlFragments, ISqlDialects dialect)
    {
        List<SqlFragment> sqlFragmentsFinalForm = [];
        IEnumerable<SqlFragment> flatenedSqlFragments = sqlFragments.Flaten();
        int j = 1; //start at 1 because PostgreSQL and SQL Lite use 1-based parameter indexing.
        for (int i = 0; i < flatenedSqlFragments.Count(); i++)
        {
            if (flatenedSqlFragments.ElementAt(i) is SqlFragmentParameter parameterFragment)
            {
                sqlFragmentsFinalForm.Add(new SqlFragmentParameter(RenderFinalParameter(dialect, parameterFragment.Parameter, j++)));
            }
            else
                sqlFragmentsFinalForm.Add(flatenedSqlFragments.ElementAt(i));
        }
        return sqlFragmentsFinalForm.AsEnumerable<SqlFragment>();
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
    internal static Dictionary<ParameterTag, object> GetParameters(this IEnumerable<SqlFragment> sqlFragments, ISqlDialects dialect)
    {
        ArgumentNullException.ThrowIfNull(sqlFragments);
        ArgumentNullException.ThrowIfNull(dialect);

        return sqlFragments
            .RenderFinalFragmentEnumeration(dialect)
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
    /// <param name="dialect"></param>
    internal static string ToSql(this IEnumerable<SqlFragment> sqlFragments, ISqlDialects dialect)
    {
        ArgumentNullException.ThrowIfNull(sqlFragments);
        ArgumentNullException.ThrowIfNull(dialect);     

        return string.Concat(sqlFragments.RenderFinalFragmentEnumeration(dialect).Select(fragment => fragment.ToSql()));
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

    /// <summary>
    /// Converts a sequence of SQL fragments into a complete <see cref="SqlQuery"/> object, including the concatenated SQL text and the collected parameters.
    /// </summary>
    /// <param name="fragments">The sequence of fragments to convert.</param>
    /// <param name="dialect">The SQL dialect to use for rendering the fragments.</param>
    /// <returns>A <see cref="SqlQuery"/> object containing the concatenated SQL text and the collected parameters.</returns>
    internal static SqlQuery ToSqlQuery(this IEnumerable<SqlFragment> fragments, ISqlDialects dialect) =>
        new ()
        {
            CommandType = System.Data.CommandType.Text,
            QueryText = fragments.ToSql(dialect),
            Parameters = fragments.GetParameters(dialect)
        };
    /// <summary>
    /// Flattens a sequence of SQL fragments by recursively expanding any nested sequences of fragments into a single, flat sequence.
    /// </summary>
    /// <param name="fragments">The sequence of fragments to flatten.</param>
    /// <returns>An enumerable collection of <see cref="SqlFragment"/> objects representing the flattened structure of the input sequence.</returns>
    internal static IEnumerable<SqlFragment> Flaten(this IEnumerable<SqlFragment> fragments) =>
        fragments.SelectMany(element => element.Flaten());
}