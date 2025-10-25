using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

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
public class NoKeyVersionPropertyException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoKeyVersionPropertyException{T}"/> class.
    /// Thrown when an encrypter is required but the target model lacks a
    /// key version property.
    /// </summary>
    public NoKeyVersionPropertyException() :
        base($"No Key Version property has been provided for the{nameof(T)} class, and {nameof(T)} has encrypted properties.")   
    {
    }
}
