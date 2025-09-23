using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;
public class InvalidColumnException : Exception
{
    public InvalidColumnException(params IEnumerable<ColumnTag> columnTags) :
        base(CreateMessage(columnTags))
    {
    }

    // Builds the exception message from a collection of ColumnTag values.
    private static string CreateMessage(IEnumerable<ColumnTag> invalidColumns) =>
        "Invalid SQL identifier(s): " 
            + invalidColumns
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
