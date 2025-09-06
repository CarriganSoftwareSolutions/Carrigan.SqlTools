using SqlTools.Tags;

namespace SqlTools.Predicates;

public interface IColumnValue
{
    public ColumnTag ColumnTag { get; }
    public TableTag TableTag { get; }
}