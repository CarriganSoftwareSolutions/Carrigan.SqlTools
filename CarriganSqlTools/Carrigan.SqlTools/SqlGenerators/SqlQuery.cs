using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents a fully rendered SQL query, including the final command text with parameter placeholders,
/// and the associated parameter values.
/// </summary>
public abstract class SqlQuery
{
    /// <summary>
    /// Gets or sets the SQL command text.
    /// </summary>
    public string QueryText { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parameter values for this command,
    /// keyed by <see cref="ParameterTag"/>.
    /// </summary>
    public Dictionary<ParameterTag, object?> ParametersAsDictionary { get; protected set; }

    public IEnumerable<SqlFragmentParameter> Parameters { get; protected set; } = [];

    /// <summary>
    /// Gets or sets the command type for this SQL query.
    /// </summary>
    public CommandType CommandType { get; protected init; }

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
