using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Predicates;

public interface IColumnValue
{
    public ColumnTag ColumnTag { get; }
    public TableTag TableTag { get; }
}