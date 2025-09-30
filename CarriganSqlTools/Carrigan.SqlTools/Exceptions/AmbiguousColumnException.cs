using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Exceptions;
//TODO: throw this if ambiguous columns passed into select, may apply to other query types and areas.
//TODO: Proof Read Documentation. entire class

//TODO: Create example for readme.md file.
//TODO: Unit tests?

/// <summary>
/// Thrown when ambiguous <C>COLUMN</C>s are detects in generated SQL.
/// </summary>
public class AmbiguousColumnException : Exception
{
    /// <summary>
    /// Constructor for <see cref="AmbiguousColumnException"/>.
    /// </summary>
    /// <param name="columnTags">The ambiguous columns to include in the exception</param>
    /// 
    //TODO: Or should this be for SelectTag?
    public AmbiguousColumnException(params IEnumerable<ColumnTag> columnTags) :
        base(CreateMessage(columnTags))
    {
    }

    // Builds the exception message from a collection of ColumnTag values.

    /// <summary>
    /// Creates the message to include in the exception.
    /// </summary>
    /// <param name="ambiguousColumns">The ambiguous columns to include in the exception</param>
    /// <returns></returns>
    private static string CreateMessage(IEnumerable<ColumnTag> ambiguousColumns) =>
        "Ambiguous SQL column identifier(s): " 
            + ambiguousColumns
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
