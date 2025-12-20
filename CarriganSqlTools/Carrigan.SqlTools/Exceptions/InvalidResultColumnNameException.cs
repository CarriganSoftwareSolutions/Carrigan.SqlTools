using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more <see cref="ResultColumnName"/> values returned from a SQL query
/// cannot be matched to a corresponding property on the target model type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The entity or model type expected to receive the SQL query results.
/// </typeparam>
/// <remarks>
/// This exception indicates that the SQL result set contains one or more column names that do not correspond to any
/// mapped or recognized property names on the model type <typeparamref name="T"/>. This typically occurs when the
/// query returns columns that have been aliased or otherwise renamed such that they no longer match the resolved
/// property mapping used by the SQL generator or reflector cache.
/// </remarks>
public class InvalidResultColumnNameException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResultColumnNameException{T}"/> class.
    /// </summary>
    /// <param name="resultColumnNames">
    /// The <see cref="ResultColumnName"/> values that could not be matched to properties on <typeparamref name="T"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resultColumnNames"/> is <c>null</c>.</exception>
    internal InvalidResultColumnNameException(params IEnumerable<ResultColumnName> resultColumnNames)
        : base(CreateMessage(resultColumnNames))
    {
    }

    /// <summary>
    /// Builds a formatted exception message listing the result columns that failed to map to properties on
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <param name="resultColumnNames">
    /// The <see cref="ResultColumnName"/> values that could not be matched to properties on <typeparamref name="T"/>.
    /// </param>
    /// <returns>A formatted exception message describing which result column names did not have corresponding mappings.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resultColumnNames"/> is <c>null</c>.</exception>
    internal static string CreateMessage(IEnumerable<ResultColumnName> resultColumnNames)
    {
        ArgumentNullException.ThrowIfNull(resultColumnNames, nameof(resultColumnNames));

        IReadOnlyCollection<string> invalidNames =
            [..
                resultColumnNames
                    .Select(name => name?.ToString() ?? "<null>")
                    .Distinct()
            ];

        return $"The following result column name(s) could not be mapped to properties on {typeof(T).Name}: {invalidNames.JoinAnd()}";
    }
}
