namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Thrown when one or more encrypted properties exist on <typeparamref name="T"/>, but no decrypter provider is supplied.
/// </summary>
/// <remarks>
/// This exception is typically raised when materializing query results into <typeparamref name="T"/>
/// (for example, via <c>Commands.ExecuteReader&lt;T&gt;</c> or <c>CommandsAsync.ExecuteReaderAsync&lt;T&gt;</c>)
/// and the model declares one or more properties marked as encrypted (for example, with <c>[Encrypted]</c>),
/// but the caller does not provide an <see cref="Carrigan.Core.Interfaces.IDecrypters"/> instance.
///
/// <para><b>Example scenario:</b></para>
/// A model type includes encrypted properties and you call <c>ExecuteReader&lt;T&gt;</c> without providing
/// a decrypter provider. Because encrypted properties require decryption, this exception is thrown.
/// </remarks>
public class DecrypterNotProvided<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecrypterNotProvided{T}"/> class.
    /// </summary>
    /// <remarks>
    /// This exception is raised when encrypted properties exist on <typeparamref name="T"/>, but no decrypter provider is supplied.
    /// </remarks>
    internal DecrypterNotProvided() :
        base($"No Decrypter provided for Sql Generator<{nameof(T)}>, and {nameof(T)} has Decrypted properties.")
    {
    }
}
