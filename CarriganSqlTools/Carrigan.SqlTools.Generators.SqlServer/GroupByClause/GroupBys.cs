using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.GroupByClause;

/// <summary>
/// Represents a collection of <c>GROUP BY</c> expressions built from properties on <typeparamref name="T"/>, whose properties represent SQL columns.
/// </summary>
/// <typeparam name="T">The table model whose properties identify columns to group by.</typeparam>
public class GroupBys<T> : GroupBys where T : class
{
    /// <summary>
    /// Initializes a grouping collection from validated property names on <typeparamref name="T"/>.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns to include in the <c>GROUP BY</c> clause.</param>
    public GroupBys(params IEnumerable<PropertyName> propertyNames) :
        base(propertyNames.Select(propertyName => new GroupBy<T>(propertyName)))
    { }

    /// <summary>
    /// Initializes a grouping collection from property-name strings on <typeparamref name="T"/>.
    /// </summary>
    /// <param name="propertyNames">The C# property names representing SQL columns to include in the <c>GROUP BY</c> clause.</param>
    [ExternalOnly]
    public GroupBys(params IEnumerable<string> propertyNames) : this(propertyNames.Select(propertyName => new PropertyName(propertyName)))
    { }
}
