using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;
public class InvalidSqlIdentifierException : Exception
{
    public InvalidSqlIdentifierException(params IEnumerable<string?> identifiers) :
        base(CreateMessage(identifiers))
    {
    }
    private static string CreateMessage(IEnumerable<string?> identifiers) =>
        $"The following identifies do not follow the SQL naming convention:" +
            identifiers
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
