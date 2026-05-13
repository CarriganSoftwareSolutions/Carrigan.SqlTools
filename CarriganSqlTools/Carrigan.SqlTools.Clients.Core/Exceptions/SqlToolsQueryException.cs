namespace Carrigan.SqlTools.Clients.Core.Exceptions;

/// <summary>
/// Base exception type for errors thrown by Carrigan.SqlTools.SqlServer.
/// </summary>
public abstract class SqlToolsQueryException : Exception
{
    protected SqlToolsQueryException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
