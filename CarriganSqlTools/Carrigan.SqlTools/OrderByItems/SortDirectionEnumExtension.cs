namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Provides extension methods for the <see cref="SortDirectionEnum"/> enumeration.
/// </summary>
public static class SortDirectionEnumExtension
{
    /// <summary>
    /// Converts the specified <see cref="SortDirectionEnum"/> value to its SQL representation.
    /// </summary>
    /// <param name="value">The sort direction to convert.</param>
    /// <returns>
    /// A SQL string representing the sort direction:
    /// <c>"ASC"</c> for <see cref="SortDirectionEnum.Ascending"/>,
    /// or <c>"DESC"</c> for <see cref="SortDirectionEnum.Descending"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="value"/> is not a defined <see cref="SortDirectionEnum"/> value.
    /// </exception>
    public static string ToSql(this SortDirectionEnum value) => value switch
    {
        SortDirectionEnum.Descending => "DESC",
        SortDirectionEnum.Ascending => "ASC",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}