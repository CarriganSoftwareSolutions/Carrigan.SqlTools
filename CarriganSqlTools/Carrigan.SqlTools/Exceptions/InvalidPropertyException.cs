using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more properties specified for use in SQL generation or invocation
/// do not exist on the target class <typeparamref name="T"/>, or do not meet the criteria
/// for usable properties.
/// </summary>
/// <remarks>
/// <para>
/// This exception is raised when the SQL generator or reflector encounters a property
/// name that is missing, inaccessible, or unsuitable for SQL operations.
/// </para>
/// 
/// <para><b>Usable property criteria:</b></para>
/// <list type="bullet">
///   <item><description>For SQL generation: properties must be <b>public</b> and <b>readable</b>.</description></item>
///   <item><description>For invocation (reading results): properties must be <b>public</b> and <b>writable</b>.</description></item>
/// </list>
/// 
/// <para>
/// It is recommended to model all properties as public with simple getters and setters.
/// </para>
/// </remarks>
/// <typeparam name="T">
/// The entity or model type for which the invalid property names were supplied.
/// </typeparam>
public class InvalidPropertyException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPropertyException{T}"/> class.
    /// </summary>
    /// <param name="propertyNames">
    /// The collection of property names that do not exist or fail to meet
    /// the criteria for usable properties on the target class <typeparamref name="T"/>.
    /// </param>
    /// <remarks>
    /// This exception is typically thrown by the reflection cache or SQL generator
    /// when validating property names against <typeparamref name="T"/>.
    /// </remarks>
    internal InvalidPropertyException(params IEnumerable<PropertyName> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }

    /// <summary>
    /// Builds a descriptive exception message listing all invalid or ineligible property names.
    /// </summary>
    /// <param name="propertyNames">
    /// The property names that were invalid or did not meet the criteria.
    /// </param>
    /// <returns>
    /// A formatted message identifying the affected type and invalid property names.
    /// </returns>
    private static string CreateMessage(IEnumerable<PropertyName> propertyNames) =>
        $"Property names for {SqlToolsReflectorCache<T>.Type.Name}, do not exist, are invalid or do not qualify: " +
            propertyNames
                .Select(property => (string)property)
                .JoinAnd();
}
