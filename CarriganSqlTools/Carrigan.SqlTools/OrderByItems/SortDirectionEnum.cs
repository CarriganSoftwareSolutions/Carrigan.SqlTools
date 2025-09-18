namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Enum options for Order By directions
/// </summary>
public enum SortDirectionEnum
{
    /// <summary>
    /// Ascending
    /// </summary>
    Ascending,
    /// <summary>
    /// Descending
    /// </summary>
    Descending
}

public static class SortDirectionEnumExtension
{
    /// <summary>
    /// Get the SQL string to represent the value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Ascending -> ASC, Descending-> DESC</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string ToSql(this SortDirectionEnum value) => value switch
    {
        SortDirectionEnum.Descending => "DESC",
        SortDirectionEnum.Ascending => "ASC",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}