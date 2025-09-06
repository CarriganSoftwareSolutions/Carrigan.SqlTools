using SqlTools.Tags;

namespace SqlTools.OrderByItems;

public interface IOrderByItem : IEquatable<IOrderByItem>
{
    public ColumnTag ColumnTag { get; }
    public TableTag TableTag { get; }
    public string ColumnName { get; }
    public SortDirectionEnum SortDirection { get; }
    public string ToSql();
}
