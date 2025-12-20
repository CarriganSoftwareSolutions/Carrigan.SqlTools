using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more properties specified for use in SQL generation or invocation
/// do not exist on the target model type <typeparamref name="T"/>, or do not meet the criteria
/// for usable properties.
/// </summary>
/// <remarks>
/// <para>
/// This exception is raised when the SQL generator or reflection cache encounters a property
/// name that is missing, inaccessible, or unsuitable for SQL operations.
/// </para>
/// <para>
/// Usable property criteria:
/// </para>
/// <list type="bullet">
///   <item>
///     <description>
///     For SQL generation: the property must be publicly readable.
///     </description>
///   </item>
///   <item>
///     <description>
///     For invocation (materializing results): the property must be publicly writable.
///     </description>
///   </item>
/// </list>
/// </remarks>
/// <typeparam name="T">
/// The entity or model type for which the invalid property names were supplied.
/// </typeparam>
public sealed class InvalidPropertyException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPropertyException{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">
    /// The collection of property names that do not exist or fail to meet the criteria for usable properties on
    /// <typeparamref name="T"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyNames"/> is <c>null</c>.</exception>
    internal InvalidPropertyException(params IEnumerable<PropertyName> propertyNames)
        : base(CreateMessage(propertyNames))
    {
    }

    private static string CreateMessage(IEnumerable<PropertyName> propertyNames)
    {
        ArgumentNullException.ThrowIfNull(propertyNames, nameof(propertyNames));

        IReadOnlyCollection<string> invalidNames =
            [..
                propertyNames
                    .Select(property => property?.ToString() ?? "<null>")
                    .Distinct()
            ];

        if (invalidNames.Count == 0)
            return $"One or more properties on {typeof(T).Name} are invalid or do not meet the required criteria.";

        return $"The following properties on {typeof(T).Name} are invalid or do not meet the required criteria: {invalidNames.JoinAnd()}";
    }
}
