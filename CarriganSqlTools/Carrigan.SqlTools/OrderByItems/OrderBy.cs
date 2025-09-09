using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;
using System.Linq;

namespace Carrigan.SqlTools.OrderByItems;

public class OrderBy: IOrderByClause
{
    public static OrderBy Empty => 
        new();
    private IEnumerable<IOrderByItem> _orderByItems;
    public OrderBy(params IEnumerable<IOrderByItem> orderByItems)
    {
        _orderByItems = orderByItems;
    }

    public bool Contains(IOrderByItem orderByItem) =>
        _orderByItems.Contains(orderByItem);


    public virtual IEnumerable<TableTag>TableTags => _orderByItems.Select(item => item.TableTag);


    public virtual string ToSql() =>
        IsEmpty() 
            ? string.Empty
            : $"ORDER BY {string.Join(", ", _orderByItems.Select(item => item.ToSql()))}";

    public virtual IEnumerable<IOrderByItem> OrderByItemsAsEnumerable() =>
        _orderByItems;

    public virtual bool IsEmpty() =>
        OrderByItemsAsEnumerable().IsNullOrEmpty();

    public OrderBy WithAppend(IOrderByItem orderByItem) =>
        new (_orderByItems.Append(orderByItem));

    public OrderBy WithConcat(params IEnumerable<IOrderByItem> orderByItems) =>
        new (_orderByItems.Concat(orderByItems));

    public OrderBy AsOrderBy() => this;
}
