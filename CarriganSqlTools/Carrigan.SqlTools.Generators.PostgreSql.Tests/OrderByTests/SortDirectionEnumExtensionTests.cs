using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.OrderByClause;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.OrderByTests;

public class SortDirectionEnumExtensionTests
{
    private readonly static ISqlDialects Dialect = new PostgreSqlDialect();
    [Theory]
    [InlineData(SortDirectionEnum.Ascending, "ASC")]
    [InlineData(SortDirectionEnum.Descending, "DESC")]
    public void ToSql_ReturnsExpectedSql_ForValidValue(SortDirectionEnum sortDirection, string expectedSql) =>
        Assert.Equal(expectedSql, sortDirection.ToSqlFragment().ToSql(Dialect));

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void ToSql_InvalidValue_ThrowsArgumentOutOfRangeException(int invalidValue)
    {
        SortDirectionEnum invalidSortDirection = (SortDirectionEnum)invalidValue;

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() => invalidSortDirection.ToSqlFragment());
        Assert.Equal("value", exception.ParamName);
        Assert.Equal(invalidSortDirection, exception.ActualValue);
    }
}
