namespace Carrigan.SqlTools.OrderByItems;

/// <summary>
/// Provides extension methods for converting <see cref="SortDirectionEnum"/> values
/// to their SQL string equivalents.
/// </summary>
internal static class SortDirectionEnumExtension
{
    /// <summary>
    /// Converts a <see cref="SortDirectionEnum"/> value into its corresponding SQL keyword.
    /// </summary>
    /// <param name="value">The sort direction to convert.</param>
    /// <returns>
    /// A SQL string representing the sort direction:
    /// <c>"ASC"</c> for <see cref="SortDirectionEnum.Ascending"/>,
    /// or <c>"DESC"</c> for <see cref="SortDirectionEnum.Descending"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="value"/> is not a valid <see cref="SortDirectionEnum"/> value.
    /// </exception>
    internal static string ToSql(this SortDirectionEnum value) =>
        value switch
        {
            SortDirectionEnum.Ascending => "ASC",
            SortDirectionEnum.Descending => "DESC",
            _ => throw CreateInvalidValueException(value),
        };

    private static ArgumentOutOfRangeException CreateInvalidValueException(SortDirectionEnum value) =>
        new(nameof(value), value, $"Invalid {nameof(SortDirectionEnum)} value: {value}.");
}
