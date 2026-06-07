using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents a fully rendered SQL query, including the final command text with parameter placeholders,
/// and the associated parameter values.
/// </summary>
public class SqlQuery
{
    /// <summary>
    /// Gets the SQL fragments used to render the command text and collect parameters.
    /// </summary>
    public IEnumerable<ISqlFragment> SqlFragments { get; set; }
    /// <summary>
    /// Gets the SQL dialect used to render fragments and parameter placeholders.
    /// </summary>
    internal readonly ISqlDialects Dialect;
    /// <summary>
    /// Gets or sets the SQL command text.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when an argument is invalid for the requested SQL operation.
    /// </exception>
    public string QueryText =>
        Validate()
            ? CommandType == CommandType.StoredProcedure
                ? SqlFragments.Where(fragment => fragment is not SqlFragmentParameter).ToSql(Dialect)
                : SqlFragments.ToSql(Dialect)
            : throw new ArgumentException(null, nameof(SqlFragments));

    /// <summary>
    /// Gets or sets the parameter values for this command,
    /// keyed by <see cref="ParameterTag"/>.
    /// </summary>
    /// <remarks>
    /// This is now reserved for internal testing methods.
    /// </remarks>
    [ExternalOnly]
    internal Dictionary<ParameterTag, object?> ParametersAsDictionary => new
    (
        Parameters
            .Select(parameter => new KeyValuePair<ParameterTag, object?>(parameter.ParameterTag, parameter.Value))
    );


    /// <summary>
    /// Gets the rendered SQL parameters keyed by their final dialect-specific names.
    /// </summary>
    public IEnumerable<SqlFragmentParameter> Parameters =>
        SqlFragments.GetSqlFragmentParameters(Dialect);

    /// <summary>
    /// Gets or sets the command type for this SQL query.
    /// </summary>
    public CommandType CommandType { get; protected init; }

    /// <summary>
    /// Validates the SQL query structure.
    /// </summary>
    /// <returns>The current query after validation succeeds.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when an argument is invalid for the requested SQL operation.
    /// </exception>
    public bool Validate()
    {
        ArgumentNullException.ThrowIfNull(SqlFragments);
        if (SqlFragments.IsNullOrEmpty())
            throw new ArgumentException(null, nameof(SqlFragments));
        return true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlQuery"/> class with the specified
    /// </summary>
    /// <param name="dialect">The SQL dialect to use for rendering the fragments.</param>
    /// <param name="commandType"></param>
    /// <param name="fragments">The sequence of SQL fragments to render.</param>
    internal SqlQuery(ISqlDialects dialect, CommandType commandType, IEnumerable<ISqlFragment> fragments)
    {
        ArgumentNullException.ThrowIfNull(fragments);
        ArgumentNullException.ThrowIfNull(dialect);
        SqlFragments = fragments;
        Dialect = dialect;
        CommandType = commandType;
    }

    /// <summary>
    /// Retrieves the value of a parameter by its name (for unit testing).
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter value.</typeparam>
    /// <param name="parameterTestName">The parameter name to look up.</param>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="parameterTestName"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when <paramref name="parameterTestName"/> is not present in <see cref="Parameters"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when the stored parameter value cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when the stored parameter value is <c>null</c> and cannot be unboxed to <typeparamref name="T"/>.
    /// </exception>
    internal object? GetParameterValue(string parameterTestName)
    {
        ArgumentNullException.ThrowIfNull(parameterTestName);
        ParameterTag tag = new (parameterTestName);

        if (ParametersAsDictionary.ContainsKey(new ParameterTag(tag)))
            return ParametersAsDictionary[tag];
        else
            throw new KeyNotFoundException($"Parameter '{parameterTestName}' was not found.");
    }

    /// <summary>
    /// Returns the total number of parameters (for unit testing).
    /// </summary>
    /// <returns>The number of items in the <see cref="Parameters"/> dictionary.</returns>
    internal int GetParameterCount() =>
        Parameters.Count();
}
