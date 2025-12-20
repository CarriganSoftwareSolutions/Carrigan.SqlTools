namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when an encrypter is required but no key version property
/// has been defined in the model class <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The model type that defines one or more encrypted properties but
/// does not specify a key version property.
/// </typeparam>
/// <remarks>
/// This exception is evaluated and may be thrown when a
/// <c>SqlGenerator&lt;T&gt;</c> is constructed.
/// It is only enforced if <typeparamref name="T"/> declares one or more
/// encrypted properties. If no properties are marked for encryption,
/// this validation is skipped.
/// </remarks>
public class NoKeyVersionException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoKeyVersionException{T}"/> class.
    /// Thrown when an encrypter is required but the target model lacks a key version property.
    /// </summary>
    public NoKeyVersionException()
        : base(CreateMessage())
    {
    }

    private static string CreateMessage() =>
        $"{typeof(T).Name} has encrypted properties but no key version property was found.";
}
