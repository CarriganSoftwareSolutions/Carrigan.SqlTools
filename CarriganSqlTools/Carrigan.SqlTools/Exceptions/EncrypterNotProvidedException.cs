namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a <c>SqlGenerator&lt;T&gt;</c> is constructed for a model type
/// that contains encrypted properties, but no encryption service is provided.
/// </summary>
/// <typeparam name="T">
/// The model type that defines one or more encrypted properties.
/// </typeparam>
/// <remarks>
/// This exception is evaluated during construction of <c>SqlGenerator&lt;T&gt;</c> when the target model type
/// declares one or more encrypted properties (for example, via <c>[Encrypted]</c>) but the generator is
/// initialized without an encryption service (for example, without providing <c>IEncryption</c>).
/// <para>
/// Depending on what other validation failures are detected, this exception may be thrown directly or wrapped
/// as an inner exception of an <see cref="AggregateException"/>.
/// </para>
/// </remarks>
public sealed class EncrypterNotProvidedException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncrypterNotProvidedException{T}"/> class.
    /// </summary>
    internal EncrypterNotProvidedException()
        : base($"No encrypter was provided to SqlGenerator<{typeof(T).Name}>, but {typeof(T).Name} contains encrypted properties.")
    {
    }
}
