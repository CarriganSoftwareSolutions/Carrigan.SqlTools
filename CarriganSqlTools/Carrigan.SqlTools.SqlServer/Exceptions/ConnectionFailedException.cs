namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when a SQL Server connection cannot be established.
/// </summary>
public sealed class ConnectionFailedException : SqlToolsSqlServerException
{
    public string FriendlyName { get; }

    public ConnectionFailedException(string friendlyName, Exception innerException)
        : base($"{friendlyName}: Connection could not be established.", innerException)
    {
        ArgumentNullException.ThrowIfNull(friendlyName);
        ArgumentNullException.ThrowIfNull(innerException);

        FriendlyName = friendlyName;
    }
}
