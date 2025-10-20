using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more tables referenced in a query are not included
/// in the actual query definition.
/// </summary>
/// <remarks>
/// This exception typically occurs during SQL generation when the system detects
/// that a table reference is missing from the current query context (for example,
/// when attempting to generate a join or filter condition against an undefined table).
/// </remarks>
/// <example>
public class InvalidTableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidTableException"/> class.
    /// Thrown when one or more tables referenced in a query are not included
    /// in the generated SQL statement.
    /// </summary>
    /// <param name="tableTags">The <see cref="TableTag"/> instances representing the invalid tables.</param>
    internal InvalidTableException(params IEnumerable<TableTag> tableTags) :
        base(CreateMessage(tableTags))
    {
    }

    /// <summary>
    /// Builds an error message listing tables that were not included in the query.
    /// </summary>
    /// <param name="tableTags">The <see cref="TableTag"/> instances representing the invalid tables.</param>
    /// <returns>A formatted error message describing the missing or invalid tables.</returns>

    private static string CreateMessage(IEnumerable<TableTag> tableTags) =>
        $"The following tables where not included in the query and are invalid" +
            tableTags
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
