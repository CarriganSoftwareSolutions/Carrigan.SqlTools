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
    public string Operation { get; }

    /// <summary>
    /// Gets the generated SQL text.
    /// </summary>
    public string QueryText { get; }

    public CommandType CommandType { get; }

    /// <summary>
    /// Gets parameter names only. Values are intentionally excluded.
    /// </summary>
    public IEnumerable<string> ParameterNames { get; }

    public bool HasTransaction { get; }

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

    private static string BuildMessage(string operation, SqlQuery query) => 
        $"{operation} failed. CommandType='{query.CommandType}', Parameters={query.Parameters.Count()}.";
}
