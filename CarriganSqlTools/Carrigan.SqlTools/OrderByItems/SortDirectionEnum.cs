namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Specifies the sort direction for an <c>ORDER BY</c> clause.
/// </summary>
/// <remarks>
/// This enum maps to SQL keywords via <see cref="SortDirectionEnumExtension.ToSql(SortDirectionEnum)"/>:
/// <c>ASC</c> and <c>DESC</c>.
/// </remarks>
public enum SortDirectionEnum
{
    /// <summary>
    /// Sorts the results in ascending order (<c>ASC</c>).
    /// </summary>
    Ascending = 0,

    /// <summary>
    /// Sorts the results in descending order (<c>DESC</c>).
    /// </summary>
    Descending = 1,
}
