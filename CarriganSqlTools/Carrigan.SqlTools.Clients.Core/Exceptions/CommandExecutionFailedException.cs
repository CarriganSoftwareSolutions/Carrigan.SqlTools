using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.SqlGenerators;
using System.Data;

namespace Carrigan.SqlTools.Clients.Core.Exceptions;

/// <summary>
/// Thrown when an ADO.NET command execution fails.
/// </summary>
public sealed class CommandExecutionFailedException : SqlToolsQueryException
{
    /// <summary>
    /// Gets the Operation value.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets the generated SQL text.
    /// </summary>
    public string QueryText { get; }

    /// <summary>
    /// Gets the CommandType value.
    /// </summary>
    public CommandType CommandType { get; }

    /// <summary>
    /// Gets parameter names only. Values are intentionally excluded.
    /// </summary>
    public IEnumerable<string> ParameterNames { get; }

    /// <summary>
    /// Gets the HasTransaction value.
    /// </summary>
    public bool HasTransaction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandExecutionFailedException"/> class.
    /// </summary>
    /// <param name="operation">The command operation that failed.</param>
    /// <param name="query">The SQL query being executed.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public CommandExecutionFailedException(string operation, SqlQuery query, Exception innerException)
        : base(BuildMessage(operation, query), innerException)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(innerException);

        Operation = operation;
        QueryText = query.QueryText;
        CommandType = query.CommandType;
        ParameterNames = query.Parameters.Select(parameters => parameters.ToString() ?? string.Empty).Materialize(NullOptionsEnum.FilteredOut);
    }

    /// <summary>
    /// Builds the exception message based on the operation and query details.
    /// </summary>
    /// <param name="operation">
    /// The command operation that failed, such as "ExecuteNonQuery" or "ExecuteReader".
    /// </param>
    /// <param name="query">The SQL query being executed.</param>
    /// <returns>The constructed exception message.</returns>
    private static string BuildMessage(string operation, SqlQuery query) =>
        $"{operation} failed. CommandType='{query.CommandType}', Parameters={query.Parameters.Count()}.";
}
