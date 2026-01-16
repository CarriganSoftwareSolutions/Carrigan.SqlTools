using Carrigan.SqlTools.OrderByItems;

namespace Carrigan.SqlTools.Tests.OrderByTests;

public class SortDirectionEnumExtensionTests
{
    [Theory]
    [InlineData(SortDirectionEnum.Ascending, "ASC")]
    [InlineData(SortDirectionEnum.Descending, "DESC")]
    public void ToSql_ShouldReturnCorrectSqlString_ForValidEnumValues(SortDirectionEnum sortDirection, string expectedSql)
    {
        // Act
        string sql = sortDirection.ToSql();

        // Assert
        Assert.Equal(expectedSql, sql);
    }

    [Fact]
    public void ToSql_ShouldThrowArgumentOutOfRangeException_ForInvalidEnumValue()
    {
        // Arrange: Cast an integer that is not defined in the enum to trigger the default case.
        SortDirectionEnum invalidSortDirection = (SortDirectionEnum)(-1);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => invalidSortDirection.ToSql());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void ToSql_InvalidValue_ThrowsArgumentOutOfRangeException(int invalidValue)
    {
        SortDirectionEnum invalid = (SortDirectionEnum)invalidValue;

        ArgumentOutOfRangeException ex =
            Assert.Throws<ArgumentOutOfRangeException>(() => invalid.ToSql());

        Assert.Equal("value", ex.ParamName);
        Assert.Equal(invalid, ex.ActualValue);
    }
}
