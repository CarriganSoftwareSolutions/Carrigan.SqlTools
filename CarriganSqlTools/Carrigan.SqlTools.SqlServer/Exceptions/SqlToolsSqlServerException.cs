namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Base exception type for errors thrown by Carrigan.SqlTools.SqlServer.
/// </summary>
public abstract class SqlToolsSqlServerException : Exception
{
    protected SqlToolsSqlServerException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
