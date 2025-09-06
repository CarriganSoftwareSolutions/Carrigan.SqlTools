using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.OrderByItems;

public interface IOrderByItem : IEquatable<IOrderByItem>
{
    public ColumnTag ColumnTag { get; }
    public TableTag TableTag { get; }
    public string ColumnName { get; }
    public SortDirectionEnum SortDirection { get; }
    public string ToSql();
}
