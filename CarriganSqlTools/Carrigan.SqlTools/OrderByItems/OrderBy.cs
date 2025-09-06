using Carrigan.Core.Extensions;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

public class OrderBy
{
    public static OrderBy Empty => 
        new();
    private IEnumerable<IOrderByItem> _orderByItems;
    public OrderBy(params IEnumerable<IOrderByItem> orderByItems)
    {
        _orderByItems = orderByItems;
    }

    public OrderBy Add(params IEnumerable<IOrderByItem> orderByItems)
    {
        _orderByItems = _orderByItems.Concat(orderByItems);
        return this;
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
}
