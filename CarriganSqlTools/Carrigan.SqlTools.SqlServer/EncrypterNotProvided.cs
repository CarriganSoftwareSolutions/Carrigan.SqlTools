namespace Carrigan.SqlTools.SqlServer;

/// <summary>
/// Thrown when a <c>SqlGenerator&lt;T&gt;</c> is constructed for a model type
/// that contains Decrypted properties, but no Decrypter instance is provided.
/// </summary>
/// <remarks>
/// This exception is triggered during construction of <c>SqlGenerator&lt;T&gt;</c>
/// (or any similar generator class) when the target model type <typeparamref name="T"/>
/// declares one or more properties marked for Decryption, but the generator
/// is initialized without a valid Decrypter.
///
/// <para><b>Example scenario:</b></para>
/// If a model class includes properties decorated with an <c>[Decrypted]</c> attribute
/// (or equivalent flag), the SQL generator requires an Decrypter to handle those
/// values during query generation. Failing to supply one at construction time
/// results in this exception.
/// </remarks>
public class DecrypterNotProvided<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecrypterNotProvided{T}"/> class.
    /// </summary>
    /// <remarks>
    /// This exception is raised when <c>SqlGenerator&lt;T&gt;</c> is instantiated
    /// without providing an Decrypter, and <typeparamref name="T"/> defines one or
    /// more Decrypted properties.
    /// </remarks>
    internal DecrypterNotProvided() :
        base($"No Decrypter provided for Sql Generator<{nameof(T)}>, and {nameof(T)} has Decrypted properties.")   
    {
    }
}
