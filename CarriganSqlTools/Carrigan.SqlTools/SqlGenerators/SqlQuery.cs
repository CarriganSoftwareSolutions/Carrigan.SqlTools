using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata;

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
    /// Gets or sets the command type for this SQL query.
    /// </summary>
    public CommandType CommandType { get; set; }

    /// <summary>
    /// Retrieves the value of a parameter by its <see cref="ParameterTag"/> (for unit testing).
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter value.</typeparam>
    /// <param name="parameter">The parameter tag to look up.</param>
    /// <returns>The parameter value cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when <paramref name="parameter"/> is not present in <see cref="Parameters"/>.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when the stored parameter value cannot be cast to <typeparamref name="T"/>.
    /// </exception>
    internal T GetParameterValue<T>(string parameterTestName) =>
        (T)Parameters.Where(param => param.Key == parameterTestName).Single().Value;

    /// <summary>
    /// Returns the total number of parameters (for unit testing).
    /// </summary>
    /// <returns>The number of items in the <see cref="Parameters"/> dictionary.</returns>
    internal int GetParameterCount() =>
        Parameters.Count;
}
