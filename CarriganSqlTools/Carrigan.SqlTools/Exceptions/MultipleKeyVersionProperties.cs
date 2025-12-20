using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when multiple key version properties are detected in a model class,
/// which is not permitted. Each class may define at most one key version property
/// for encryption key management.
/// </summary>
/// <typeparam name="T">
/// The model type in which multiple key version properties were found.
/// </typeparam>
/// <remarks>
/// This exception is evaluated and may be thrown when a <c>SqlGenerator&lt;T&gt;</c> is constructed.
/// The check is enforced only if <typeparamref name="T"/> declares one or more properties
/// flagged for encryption. If the model has no encrypted properties, this constraint
/// is not enforced and the exception will not be thrown.
/// </remarks>
public class MultipleKeyVersionProperties<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleKeyVersionProperties{T}"/> class.
    /// Raised during <c>SqlGenerator&lt;T&gt;</c> construction when more than one key version
    /// property is detected and the model contains encrypted properties.
    /// </summary>
    /// <param name="propertyNames">
    /// The <see cref="PropertyName"/> instances representing the conflicting key version properties.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyNames"/> is <c>null</c>.</exception>
    internal MultipleKeyVersionProperties(params IEnumerable<PropertyName> propertyNames)
        : base(CreateMessage(propertyNames))
    {
    }

    /// <summary>
    /// Builds a formatted exception message listing the duplicate key version properties.
    /// </summary>
    /// <param name="invalidPropertyNames">
    /// The <see cref="PropertyName"/> instances representing the duplicate key version properties.
    /// </param>
    /// <returns>
    /// A formatted exception message describing the duplicate key version properties.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="invalidPropertyNames"/> is <c>null</c>.</exception>
    private static string CreateMessage(IEnumerable<PropertyName> invalidPropertyNames)
    {
        ArgumentNullException.ThrowIfNull(invalidPropertyNames, nameof(invalidPropertyNames));

        IReadOnlyCollection<string> names =
            [..
                invalidPropertyNames
                    .Select(property => property?.ToString() ?? "<null>")
                    .Distinct()
            ];

        return $"{typeof(T).Name} has multiple key version properties, which is not allowed: {names.JoinAnd()}.";
    }
}
