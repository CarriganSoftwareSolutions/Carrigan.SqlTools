namespace SqlTools.OrderByItems;

public enum SortDirectionEnum
{
    Ascending,
    Descending
}



public static class SortDirectionEnumExtension
{
    public static string ToSql(this SortDirectionEnum value)
    {
        return value switch
        {
            SortDirectionEnum.Descending => "DESC",
            SortDirectionEnum.Ascending => "ASC",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}