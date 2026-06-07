namespace Carrigan.SqlTools.Clients.Core.Exceptions;

/// <summary>
/// Thrown when a database connection cannot be established by a Carrigan.SqlTools client package.
/// </summary>
public sealed class ConnectionFailedException : SqlToolsQueryException
{
    /// <summary>
    /// Gets the friendly name of the database connection that failed.
    /// </summary>
    public string FriendlyName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionFailedException"/> class.
    /// </summary>
    /// <param name="friendlyName">The friendly connection name.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public ConnectionFailedException(string friendlyName, Exception innerException)
        : base($"{friendlyName}: Connection could not be established.", innerException)
    {
        ArgumentNullException.ThrowIfNull(friendlyName);
        ArgumentNullException.ThrowIfNull(innerException);

        FriendlyName = friendlyName;
    }
}
