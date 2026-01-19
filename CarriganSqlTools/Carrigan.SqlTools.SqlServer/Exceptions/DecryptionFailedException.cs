namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when a decryption operation fails.
/// </summary>
/// <typeparam name="T">The model type being materialized.</typeparam>
public sealed class DecryptionFailedException<T> : SqlToolsSqlServerException
{
    /// <summary>
    /// Gets the key version used for decryption.
    /// </summary>
    public int KeyVersion { get; }

    /// <summary>
    /// Gets the property name that failed to decrypt.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DecryptionFailedException{T}"/> class.
    /// </summary>
    /// <param name="keyVersion">The key version used.</param>
    /// <param name="propertyName">The property name being decrypted.</param>
    /// <param name="innerException">The underlying exception.</param>
    public DecryptionFailedException(int keyVersion, string propertyName, Exception innerException)
        : base(BuildMessage(keyVersion, propertyName), innerException)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(innerException);

        KeyVersion = keyVersion;
        PropertyName = propertyName;
    }

    private static string BuildMessage(int keyVersion, string propertyName) =>
        $"Decryption failed for '{typeof(T).Name}.{propertyName}'. KeyVersion={keyVersion}.";
}
