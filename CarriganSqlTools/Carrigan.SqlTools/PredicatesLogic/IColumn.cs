using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;
/// <summary>
/// Used to represent basic column and table data in each predicate.
/// This is likely not longer needed now that <see cref="ColumnTag"/> has been expanded.
/// </summary>
/// <remarks>
/// I forget exactly why I made this interface, however, it appears to allow some shenanigans
/// to take place to allow me to more generically use Columns T. For now it is not worth the 
/// time and effort to attempt to replace it, it may not even be possible.
/// </remarks>
internal interface IColumn
{
    /// <summary>
    /// The Tag for the Column
    /// </summary>
    internal ColumnInfo ColumnInfo { get; }
    /// <summary>
    /// The Tag for the Table
    /// </summary>
    internal TableTag TableTag { get; } //TODO: Can we get rid of this now?
}