using Carrigan.SqlTools.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.GroupByClause;

public static class GroupBysExtensions
{

    /// <summary>
    /// Appends an group-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="groupBys"></param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>A new collection containing the additional group-by item.</returns>
    public static GroupBys Append<T>(this GroupBys groupBys, PropertyName propertyName) where T : class =>
        new (groupBys.GroupByItems.Append(new GroupBy<T>(propertyName)));

    /// <summary>
    /// Appends an group-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="groupBys"></param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <returns>A new collection containing the additional group-by item.</returns>
    public static GroupBys Append<T>(this GroupBys groupBys, string propertyName) where T : class =>
        groupBys.Append<T>(new PropertyName(propertyName));
}
