using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;
public class InvalidPropertyException<T> : Exception
{
    public InvalidPropertyException(params IEnumerable<string> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }
    private static string CreateMessage(IEnumerable<string> propertyNames) =>
        $"Property names for {SqlToolsReflectorCache<T>.Type.Name}, do not exist, are invalid or do qualify: " +
            propertyNames
                .Select(column => $"{column?.ToString() ?? "<null>"}")
                .JoinAnd();
}
