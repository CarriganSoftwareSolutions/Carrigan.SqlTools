using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more <see cref="ResultColumnName"/> values returned from a SQL query
/// cannot be matched to a corresponding property on the target class <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The entity or model type expected to receive the SQL query results.
/// </typeparam>
/// <remarks>
/// This exception indicates that the SQL result set contains one or more column names
/// that do not correspond to any mapped or recognized property names on the model type
/// <typeparamref name="T"/>.
///
/// <para>
/// This typically occurs when the database query returns columns that have been aliased,
/// renamed, or otherwise do not match the expected property mapping as determined by
/// the SQL generator or reflection cache.
/// </para>
/// </remarks>
public class InvalidResultColumnNameException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResultColumnNameException{T}"/> class.
    /// </summary>
    /// <param name="resultColumnNames">
    /// The <see cref="ResultColumnName"/> values that could not be matched
    /// to properties on the target class <typeparamref name="T"/>.
    /// </param>

    internal InvalidResultColumnNameException(params IEnumerable<ResultColumnName> resultColumnNames) :
        base(CreateMessage(resultColumnNames))
    {
    }

    /// <summary>
    /// Builds a formatted exception message listing the result columns that failed to map
    /// to properties on the target class <typeparamref name="T"/>.
    /// </summary>
    /// <param name="resultColumnNames">
    /// The <see cref="ResultColumnName"/> values that could not be matched
    /// to properties on the target class <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// A formatted exception message describing which result column names did not have
    /// corresponding property mappings.
    /// </returns>
    /// <remarks>
    /// This method is typically invoked when the SQL generator or reflection cache
    /// attempts to bind result columns to object properties and one or more names fail to match.
    /// </remarks>
    internal static string CreateMessage(IEnumerable<ResultColumnName> resultColumnNames) =>
        $"The ADO column name does not have corresponding property name that matches: " +
            resultColumnNames
                .Select(ResultColumnName => (string)ResultColumnName) 
                .JoinAnd();
}
