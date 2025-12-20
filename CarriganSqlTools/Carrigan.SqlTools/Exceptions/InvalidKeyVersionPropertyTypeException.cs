using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a model type <typeparamref name="T"/> includes encrypted properties
/// but defines one or more key version properties that are not of type <see cref="int"/>.
/// </summary>
/// <remarks>
/// This exception is typically raised during initialization of a SQL generator or model reflector when encryption
/// support is enabled, and the property designated to store the encryption key version uses an invalid CLR type.
/// <para>
/// If a property is intended to hold the key version used for encryption rotation, it must be declared as an
/// <see cref="int"/>. If the property type differs (for example, <see cref="string"/> or <see cref="short"/>),
/// this exception is thrown.
/// </para>
/// </remarks>
//TODO: create an analyzer to enforce the int requirement
public sealed class InvalidKeyVersionPropertyTypeException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidKeyVersionPropertyTypeException{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">
    /// The property or properties that were marked as key version fields but are not of type <see cref="int"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyNames"/> is <c>null</c>.</exception>
    internal InvalidKeyVersionPropertyTypeException(params IEnumerable<PropertyName> propertyNames)
        : base(CreateMessage(propertyNames))
    {
    }

    /// <summary>
    /// Builds a descriptive exception message listing all invalid key version properties.
    /// </summary>
    /// <param name="invalidPropertyNames">The invalid property names that were identified during model reflection.</param>
    /// <returns>A message describing which key version properties were not of type <see cref="int"/>.</returns>
    private static string CreateMessage(IEnumerable<PropertyName> invalidPropertyNames)
    {
        ArgumentNullException.ThrowIfNull(invalidPropertyNames, nameof(invalidPropertyNames));

        IReadOnlyCollection<string> invalidNames =
            [..
                invalidPropertyNames
                    .Select(property => property?.ToString() ?? "<null>")
                    .Distinct()
            ];

        if (invalidNames.Count == 0)
            return $"{typeof(T).Name} has encrypted columns and one or more key version properties are not of type int.";

        string propertyLabel = invalidNames.Count == 1 ? "property" : "properties";

        return $"{typeof(T).Name} has encrypted columns and the key version {propertyLabel} "
            + $"{invalidNames.JoinAnd()} {(invalidNames.Count == 1 ? "is" : "are")} not of type int.";
    }
}
