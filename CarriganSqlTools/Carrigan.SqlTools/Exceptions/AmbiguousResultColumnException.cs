using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System.Diagnostics.Metrics;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when one or more ambiguous <see cref="ResultColumnName"/> values are detected
/// in a generated SQL statement.
/// </summary>
/// <remarks>
/// This exception is raised when multiple result columns share the same resolved name,
/// causing ambiguity in the SQL result set. Such conflicts typically arise when two or
/// more columns have identical aliases or when aliasing is omitted for identically named
/// columns across joined tables.
/// </remarks>
public class AmbiguousResultColumnException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmbiguousResultColumnException"/> class
    /// with a message describing the ambiguous column names.
    /// </summary>
    /// <param name="resultColumns">The ambiguous result column names.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resultColumns"/> is <c>null</c>.</exception>
    internal AmbiguousResultColumnException(params IEnumerable<ResultColumnName> resultColumns) :
        base(CreateMessage(resultColumns))
    {
    }

    /// <summary>
    /// Builds a formatted message describing all ambiguous column identifiers.
    /// </summary>
    /// <param name="ambiguousResultColumns">The ambiguous columns to include in the message.</param>
    /// <returns>
    /// A human-readable message listing all ambiguous SQL column identifiers.
    /// </returns>
    private static string CreateMessage(IEnumerable<ResultColumnName> ambiguousResultColumns)
    {
        IReadOnlyCollection<string> ambiguousNames =
            [..
                ambiguousResultColumns
                    .Select(column => column?.ToString() ?? "<null>")
                    .Distinct()
            ];

        return $"Ambiguous SQL result column identifier(s): {ambiguousNames.JoinAnd()}";
    }

    /// <summary>
    /// Checks a given <see cref="SelectTagsBase"/> instance for duplicate
    /// <see cref="ResultColumnName"/> values and returns an exception if any are found.
    /// </summary>
    /// <param name="selects">The <see cref="SelectTagsBase"/> instance to analyze for ambiguous result columns.</param>
    /// <returns>
    /// A new <see cref="AmbiguousResultColumnException"/> if duplicate column names are detected;
    /// otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method is typically invoked by SQL generator validation routines to ensure that
    /// no two columns in a result set produce the same output name, whether by alias or by column name.
    /// </remarks>
    internal static AmbiguousResultColumnException? CheckNames(SelectTagsBase? selects)
    {
        if (selects is null || selects.Empty())
            return null;
        else
        {
            IReadOnlyCollection<ResultColumnName> names =
                [.. selects.All().Select(tag => tag.ResultColumnName)];
            Dictionary<ResultColumnName, int> namesCounter = [];

            foreach (ResultColumnName name in names)
            {
                if (namesCounter.TryGetValue(name, out int value))
                    namesCounter[name] = value + 1;
                else
                    namesCounter.Add(name, 1);
            }

            IReadOnlyCollection<ResultColumnName> invalids =
                [.. namesCounter.Where(pair => pair.Value > 1).Select(pair => pair.Key)];

            return invalids.Count == 0 ?  null : new(invalids);
        }
    }
}
