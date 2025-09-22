using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents a parameterized SQL command, including the command text,
/// parameters, and the ADO.NET <see cref="CommandType"/>.
/// </summary>
public class SqlQuery
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlQuery"/> class.
    /// </summary>
    internal SqlQuery()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlQuery"/> class with the specified
    /// command text and parameters.
    /// </summary>
    /// <param name="query">The SQL command text.</param>
    /// <param name="parameters">
    /// The parameter dictionary for the command, keyed by <see cref="ParameterTag"/>.
    /// </param>
    public SqlQuery(string query, Dictionary<ParameterTag, object> parameters)
    {
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
    /// Gets or sets the parameter values for this command,
    /// keyed by <see cref="ParameterTag"/>.
    /// </summary>
    public CommandType CommandType { get; set; }

    /// <summary>
    /// Retrieves the value of a parameter by name (for unit testing).
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter value.</typeparam>
    /// <param name="parameterTestName">The parameter name to look up.</param>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This method is intended for unit testing scenarios and assumes that
    /// parameter names are comparable to <see cref="ParameterTag"/> via equality.
    /// </remarks>
    internal T GetParameterValue<T>(string parameterTestName) =>
        (T)Parameters.Where(param => param.Key == parameterTestName).Single().Value;

    /// <summary>
    /// Returns the total number of parameters (for unit testing).
    /// </summary>
    /// <returns>The number of items in the <see cref="Parameters"/> dictionary.</returns>
    internal int GetParameterCount() =>
        Parameters.Count;
}
