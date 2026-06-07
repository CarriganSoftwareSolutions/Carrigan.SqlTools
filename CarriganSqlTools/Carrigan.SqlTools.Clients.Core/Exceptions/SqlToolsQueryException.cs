namespace Carrigan.SqlTools.Clients.Core.Exceptions;

/// <summary>
/// Base exception type for errors thrown by Carrigan.SqlTools client packages.
/// </summary>
public abstract class SqlToolsQueryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlToolsQueryException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    protected SqlToolsQueryException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
