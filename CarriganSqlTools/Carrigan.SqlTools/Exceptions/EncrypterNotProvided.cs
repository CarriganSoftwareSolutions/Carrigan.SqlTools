namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a <c>SqlGenerator&lt;T&gt;</c> is constructed for a model type
/// that contains encrypted properties, but no encrypter instance is provided.
/// </summary>
/// <remarks>
/// This exception is triggered during construction of <c>SqlGenerator&lt;T&gt;</c>
/// (or any similar generator class) when the target model type <typeparamref name="T"/>
/// declares one or more properties marked for encryption, but the generator
/// is initialized without a valid encrypter.
///
/// <para><b>Example scenario:</b></para>
/// If a model class includes properties decorated with an <c>[Encrypted]</c> attribute
/// (or equivalent flag), the SQL generator requires an encrypter to handle those
/// values during query generation. Failing to supply one at construction time
/// results in this exception.
/// </remarks>
public class EncrypterNotProvided<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncrypterNotProvided{T}"/> class.
    /// </summary>
    /// <remarks>
    /// This exception is raised when <c>SqlGenerator&lt;T&gt;</c> is instantiated
    /// without providing an encrypter, and <typeparamref name="T"/> defines one or
    /// more encrypted properties.
    /// </remarks>
    internal EncrypterNotProvided() :
        base($"No encrypter provided for Sql Generator<{nameof(T)}>, and {nameof(T)} has encrypted properties.")   
    {
    }
}
