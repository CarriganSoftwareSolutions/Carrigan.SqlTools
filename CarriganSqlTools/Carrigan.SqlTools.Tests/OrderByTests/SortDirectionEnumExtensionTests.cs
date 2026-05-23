using Carrigan.SqlTools.OrderByItems;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class SortDirectionEnumExtensionTests
{
    [Theory]
    [InlineData(SortDirectionEnum.Ascending, "ASC")]
    [InlineData(SortDirectionEnum.Descending, "DESC")]
    public void ToSql_ReturnsExpectedSql_ForValidValue(SortDirectionEnum sortDirection, string expectedSql) =>
        Assert.Equal(expectedSql, sortDirection.ToSql());

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void ToSql_InvalidValue_ThrowsArgumentOutOfRangeException(int invalidValue)
    {
        SortDirectionEnum invalidSortDirection = (SortDirectionEnum)invalidValue;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => invalidSortDirection.ToSql());
        Assert.Equal("value", exception.ParamName);
        Assert.Equal(invalidSortDirection, exception.ActualValue);
    }
}
