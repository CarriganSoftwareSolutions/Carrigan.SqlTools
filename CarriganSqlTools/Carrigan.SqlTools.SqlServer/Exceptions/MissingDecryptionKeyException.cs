namespace Carrigan.SqlTools.SqlServer.Exceptions;

/// <summary>
/// Thrown when encrypted properties contain values but no matching decryption key can be found.
/// </summary>
/// <typeparam name="T">The model type being materialized.</typeparam>
public sealed class MissingDecryptionKeyException<T> : SqlToolsSqlServerException
{
    /// <summary>
    /// Gets the key version required to decrypt the record.
    /// </summary>
    public int? KeyVersion { get; }

    /// <summary>
    /// Gets the property name that required decryption.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingDecryptionKeyException{T}"/> class.
    /// </summary>
    /// <param name="keyVersion">The key version read from the record.</param>
    /// <param name="propertyName">The property name that required decryption.</param>
    public MissingDecryptionKeyException(int? keyVersion, string propertyName)
        : base(BuildMessage(keyVersion, propertyName), innerException: null)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        KeyVersion = keyVersion;
        PropertyName = propertyName;
    }

    private static string BuildMessage(int? keyVersion, string propertyName)
    {
        string keyDisplay = keyVersion is null ? "<null>" : keyVersion.Value.ToString();
        return $"No decryption key found for '{typeof(T).Name}.{propertyName}'. KeyVersion={keyDisplay}.";
    }
}
