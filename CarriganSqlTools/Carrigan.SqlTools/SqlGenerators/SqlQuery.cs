using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents a parameterized SQL command, including the command text,
/// parameters, and the ADO.NET <see cref="CommandType"/>.
/// DON'T FORGET TO PARAMETERIZE YOUR SQL TO MITIGATE SQL INJECTION
/// </summary>
/// <remarks>
/// Intentionally left public to allow manual SQL.
/// Use at your own risk, and DON'T FORGET TO PARAMETERIZE YOUR SQL TO MITIGATE SQL INJECTION.
/// </remarks>
public class SqlQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlQuery"/> class.
    /// </summary>
    internal SqlQuery()
    {
        QueryText = string.Empty;
        Parameters = [];
        CommandType = CommandType.Text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlQuery"/> class with the specified
    /// command text and parameters.
    /// DON'T FORGET TO PARAMETERIZE YOUR SQL TO MITIGATE SQL INJECTION
    /// </summary>
    /// <remarks>
    /// Intentionally left public to allow manual SQL.
    /// Use at your own risk, and DON'T FORGET TO PARAMETERIZE YOUR SQL TO MITIGATE SQL INJECTION.
    /// </remarks>
    /// <param name="query">The SQL command text.</param>
    /// <param name="parameters">
    /// The parameter dictionary for the command, keyed by <see cref="ParameterTag"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="query"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="parameters"/> is <c>null</c>.
    /// </exception>
    public SqlQuery(string query, Dictionary<ParameterTag, object> parameters)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(parameters);

        QueryText = query;
        Parameters = parameters;
    }

    /// <summary>
    /// Gets or sets the SQL command text.
    /// </summary>
    public string QueryText { get; set; }

    /// <summary>
    /// Gets or sets the parameter values for this command,
    /// keyed by <see cref="ParameterTag"/>.
    /// </summary>
    public Dictionary<ParameterTag, object> Parameters { get; set; }

    /// <summary>
    /// Gets or sets the command type for this SQL query.
    /// </summary>
    public CommandType CommandType { get; set; }

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
    internal T GetParameterValue<T>(string parameterTestName)
    {
        ArgumentNullException.ThrowIfNull(parameterTestName);

        foreach (KeyValuePair<ParameterTag, object> item in Parameters)
        {
            if (item.Key == parameterTestName)
            {
                return (T)item.Value;
            }
        }

        throw new KeyNotFoundException($"Parameter '{parameterTestName}' was not found.");
    }

    /// <summary>
    /// Returns the total number of parameters (for unit testing).
    /// </summary>
    /// <returns>The number of items in the <see cref="Parameters"/> dictionary.</returns>
    internal int GetParameterCount() =>
        Parameters.Count;
}
