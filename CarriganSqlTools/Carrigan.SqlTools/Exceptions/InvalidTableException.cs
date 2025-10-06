using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation. entire class

//TODO: Create example for readme.md file.
/// <summary>
/// The InvalidTableException is thrown when a table used for query
/// is not included in the query.
/// </summary>

public class InvalidTableException : Exception
{
    /// <summary>
    /// The InvalidTableException class constructor.
    /// The InvalidTableException is thrown when a table used for query
    /// is not included in the query.
    /// </summary>
    /// <param name="tableTags">Table that are invalid</param>
    public InvalidTableException(params IEnumerable<TableTag> tableTags) :
        base(CreateMessage(tableTags))
    {
    }
    /// <summary>
    /// Creates a message for the InvalidTableException
    /// </summary>
    /// <param name="tableTags">Table that are invalid</param>
    private static string CreateMessage(IEnumerable<TableTag> tableTags) =>
        $"The following tables where not included in the query and are invalid" +
            tableTags
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
