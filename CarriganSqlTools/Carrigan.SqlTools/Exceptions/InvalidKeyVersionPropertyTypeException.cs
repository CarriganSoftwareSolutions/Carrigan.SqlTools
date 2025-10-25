using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a model type <typeparamref name="T"/> includes encrypted properties
/// but defines one or more key version properties that are not of type <see cref="int"/>.
/// </summary>
/// <remarks>
/// This exception is typically raised during initialization of a SQL generator or
/// model reflector when encryption support is enabled, and the property designated
/// to store the encryption key version uses an invalid data type.
/// 
/// <para><b>Example:</b></para>
/// If a property is intended to hold the key version used for encryption rotation,
/// it must be declared as an <see cref="int"/>. If the property type differs
/// (for example, <see cref="string"/> or <see cref="short"/>), this exception is thrown.
/// </remarks>
public class InvalidKeyVersionPropertyTypeException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidKeyVersionPropertyTypeException{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">
    /// The property or properties that were marked as key version fields but are not of type <see cref="int"/>.
    /// </param>
    /// <remarks>
    /// This exception is thrown when the SQL generator detects one or more properties
    /// associated with encryption key versioning that do not use the required <see cref="int"/> type.
    /// </remarks>
    internal InvalidKeyVersionPropertyTypeException(params IEnumerable<PropertyName> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }

    /// <summary>
    /// Builds a descriptive exception message listing all invalid key version properties.
    /// </summary>
    /// <param name="invalidPropertyNames">
    /// The invalid property names that were identified during model reflection.
    /// </param>
    /// <returns>
    /// A human-readable message describing which key version properties were not of type <see cref="int"/>.
    /// </returns>
    private static string CreateMessage(IEnumerable<PropertyName> invalidPropertyNames) =>
        $"{nameof(T)} has encrypted columns and the key version property, "
            + invalidPropertyNames
                .Select(property => $"{property.ToString() ?? "<null>"}")
                .JoinAnd()
            + ", is not of type int.";
}
