using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Predicates;
//TODO: Consider replacing with just ColumnTag?
/// <summary>
/// Used to represent basic column and table data in each predicate.
/// This is likely not longer needed now that <see cref="ColumnTag"/> has been expanded.
/// </summary>
public interface IColumnValue
{
    /// <summary>
    /// The Tag for the Column
    /// </summary>
    public ColumnTag ColumnTag { get; }
    /// <summary>
    /// The Tag for the Table
    /// </summary>
    public TableTag TableTag { get; }
}