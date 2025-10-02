using Carrigan.Core.Extensions;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation. entire class

//TODO: Create example for readme.md file.
//TODO: Unit tests?
/// <summary>
/// Thrown when the column tag comes from a table that is not included in the clauses.
/// </summary>
public class InvalidColumnException : Exception
{
    /// <summary>
    /// Constructor for InvalidColumnException
    /// Thrown when the column tag comes from a table that is not included in the clauses.
    /// </summary>
    /// <param name="columnInfo">Invalid columns to include in exception message.</param>
    public InvalidColumnException(params IEnumerable<ColumnInfo> columnInfo) :
        base(CreateMessage(columnInfo))
    {

    }

    /// <summary>
    /// Builds the exception message from a collection of invalid ColumnTag values.
    /// </summary>
    /// <param name="invalidColumns">Invalid columns to include in exception message.</param>
    /// <returns>An exception message from a collection of invalid ColumnTag values</returns>
    private static string CreateMessage(IEnumerable<ColumnInfo> invalidColumns) =>
        $"The following columns where not included in the query and are invalid"
            + invalidColumns
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
