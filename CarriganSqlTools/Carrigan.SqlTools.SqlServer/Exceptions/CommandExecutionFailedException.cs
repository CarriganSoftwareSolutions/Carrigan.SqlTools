using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.Core.Extensions;
using System.Data;
using System.Data.Common;

namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when an ADO.NET command execution fails.
/// </summary>
public sealed class CommandExecutionFailedException : SqlToolsSqlServerException
{
    public string Operation { get; }
    public string QueryText { get; }
    public CommandType CommandType { get; }
    public IEnumerable<string> ParameterNames { get; }
    public string ConnectionType { get; }
    public string? Database { get; }
    public ConnectionState ConnectionState { get; }
    public bool HasTransaction { get; }

    public CommandExecutionFailedException(string operation, SqlQuery query, DbConnection connection, DbTransaction? transaction, Exception innerException)
        : base(BuildMessage(operation, query, connection, transaction), innerException)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(innerException);

        Operation = operation;
        QueryText = query.QueryText;
        CommandType = query.CommandType;
        ParameterNames = query.Parameters.Select(parameter => parameter.Key.ToString()).Materialize(Core.Enums.NullOptionsEnum.FilteredOut);
        ConnectionType = connection.GetType().Name;
        Database = SafeGetDatabase(connection);
        ConnectionState = connection.State;
        HasTransaction = transaction is not null;
    }

    private static string? SafeGetDatabase(DbConnection connection)
    {
        try
        {
            return connection.Database;
        }
        catch
        {
            return null;
        }
    }

    private static string BuildMessage(string operation, SqlQuery query, DbConnection connection, DbTransaction? transaction)
    {
        string database = SafeGetDatabase(connection) ?? "";
        string databaseDisplay = string.IsNullOrWhiteSpace(database) ? "" : $", Database='{database}'";

        int parameterCount = query.Parameters.Count;
        string transactionDisplay = transaction is null ? "No" : "Yes";

        return $"{operation} failed. CommandType='{query.CommandType}', Parameters={parameterCount}, Transaction={transactionDisplay}, ConnectionType='{connection.GetType().Name}', ConnectionState='{connection.State}'{databaseDisplay}.";
    }
}
