using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;
//TODO: throw this if ambiguous columns passed into select, may apply to other query types and areas.
//TODO: Documentation
public class AmbiguousColumnException : Exception
{
    public AmbiguousColumnException(params IEnumerable<ColumnTag> columnTags) :
        base(CreateMessage(columnTags))
    {
    }

    // Builds the exception message from a collection of ColumnTag values.
    private static string CreateMessage(IEnumerable<ColumnTag> ambiguousColumns) =>
        "Ambiguous SQL column identifier(s): " 
            + ambiguousColumns
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
