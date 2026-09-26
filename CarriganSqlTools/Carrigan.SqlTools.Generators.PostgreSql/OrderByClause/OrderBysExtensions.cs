using Carrigan.SqlTools.IdentifierTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.OrderByClause;

public static class OrderBysExtensions
{

    /// <summary>
    /// Appends an order-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="orderBys">The existing order-by collection.</param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="sortDirection">The SQL sort direction.</param>
    /// <returns>A new collection containing the additional order-by item.</returns>
    public static OrderBys Append<T>(this OrderBys orderBys, PropertyName propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        where T : class =>
        orderBys.Append(new OrderBy<T>(propertyName, sortDirection));

    /// <summary>
    /// Appends an order-by item for a property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="orderBys">The existing order-by collection.</param>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="sortDirection">The SQL sort direction.</param>
    /// <returns>A new collection containing the additional order-by item.</returns>
    public static OrderBys Append<T>(this OrderBys orderBys, string propertyName, SortDirectionEnum sortDirection = SortDirectionEnum.Ascending)
        where T : class =>
        orderBys.Append<T>(new PropertyName(propertyName), sortDirection);
}
