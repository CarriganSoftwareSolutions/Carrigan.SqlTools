using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Exceptions;
public class InvalidPropertyException<T> : Exception
{
    public InvalidPropertyException(params IEnumerable<PropertyName> propertyNames) :
        base(CreateMessage(propertyNames))
    {
    }
    private static string CreateMessage(IEnumerable<PropertyName> propertyNames) =>
        $"Property names for {SqlToolsReflectorCache<T>.Type.Name}, do not exist, are invalid or do qualify: " +
            propertyNames
                .Select(property => $"{property}") //TODO: simplify this?
                .JoinAnd();
}
