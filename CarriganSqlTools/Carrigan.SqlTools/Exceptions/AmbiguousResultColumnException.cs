using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Exceptions;
//TODO: throw this if ambiguous columns passed into select, may apply to other query types and areas.
//TODO: Proof Read Documentation. entire class

/// <summary>
/// Thrown when ambiguous Result Column Name's are detects in generated SQL.
/// </summary>
public class AmbiguousResultColumnException : Exception
{
    /// <summary>
    /// Constructor for <see cref="AmbiguousResultColumnException"/>.
    /// </summary>
    /// <param name="resultColumns">The ambiguous result columns to include in the exception</param>
    /// 
    internal AmbiguousResultColumnException(params IEnumerable<ResultColumnName> resultColumns) :
        base(CreateMessage(resultColumns))
    {
    }

    /// <summary>
    /// Creates the message to include in the exception.
    /// Builds the exception message from a collection of SelectTag values.
    /// </summary>
    /// <param name="ambiguousResultColumns">The ambiguous columns to include in the exception</param>
    /// <returns></returns>
    private static string CreateMessage(IEnumerable<ResultColumnName> ambiguousResultColumns) =>
        "Ambiguous SQL result column identifier(s): " 
            + ambiguousResultColumns
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();


    internal static AmbiguousResultColumnException? CheckNames(SelectTagsBase? selects)
    {
        if (selects is null || selects.Empty())
            return null;
        else
        {
            IEnumerable<ResultColumnName> names = selects.All().Select(tag => tag.ResultColumnName);
            IEnumerable<ResultColumnName> invalids = 
                selects.All()
                    .Select(tag => tag.ResultColumnName)
                    .Where(outerName => names.Count(innerTag => innerTag == outerName) > 1);
            return invalids.Any() ? new (invalids) : null;
        }
    }
}
