using Carrigan.Core.Extensions;
using Carrigan.SqlTools.SqlGenerators;
using System.Data;
using System.Data.Common;

namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when an ADO.NET command execution fails.
/// </summary>
public sealed class CommandExecutionFailedException : SqlToolsSqlServerException
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

    public CommandExecutionFailedException(string operation, SqlQuery query, DbConnection connection, DbTransaction? transaction, Exception innerException)
        : base(BuildMessage(operation, query, transaction), innerException)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(innerException);

        _ = connection;

        Operation = operation;
        QueryText = query.QueryText;
        CommandType = query.CommandType;
        ParameterNames = query.Parameters.Select(parameters => parameters.Key.ToString()).Materialize(Core.Enums.NullOptionsEnum.FilteredOut);
        HasTransaction = transaction is not null;
    }

    private static string BuildMessage(string operation, SqlQuery query, DbTransaction? transaction)
    {
        string transactionDisplay = transaction is null ? "No" : "Yes";
        return $"{operation} failed. CommandType='{query.CommandType}', Parameters={query.Parameters.Count}, Transaction={transactionDisplay}.";
    }
}
