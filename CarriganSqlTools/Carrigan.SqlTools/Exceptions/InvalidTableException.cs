using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;

public class InvalidTableException : Exception
{

    public InvalidTableException(params IEnumerable<TableTag> tableTags) :
        base(CreateMessage(tableTags))
    {
    }
    private static string CreateMessage(IEnumerable<TableTag> tableTags) =>
        $"The following identifies do not follow the SQL naming convention:" +
            tableTags
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
