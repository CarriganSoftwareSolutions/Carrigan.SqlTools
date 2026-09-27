using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.GroupByClause;

public class GroupBys<T> : GroupBys where T: class
{
    public GroupBys(params IEnumerable<PropertyName> propertyNames) :
        base (propertyNames.Select(propertyName => new GroupBy<T>(propertyName)))
    { }

    [ExternalOnly]
    public GroupBys(params IEnumerable<string> propertyNames) : this(propertyNames.Select(propertyName => new PropertyName(propertyName)))
    { }
}
