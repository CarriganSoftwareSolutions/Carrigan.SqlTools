using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Paging;

namespace Carrigan.SqlTools.Tests.PagingTests;

public class OffsetNextTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Theory]
    // Both Offset and Next are 0 → expect an empty SQL string.
    [InlineData(0u, 0u, "")]
    // Next is 0, but Offset is nonzero → expect "OFFSET {Offset}"
    [InlineData(10u, 0u, "OFFSET 10 ROWS")]
    // OFFSET ROW Singular
    [InlineData(1u, 0u, "OFFSET 1 ROW")]
    // OFFSET FETCH NEXT ROW Singular
    [InlineData(1u, 1u, "OFFSET 1 ROW FETCH NEXT 1 ROW ONLY")]
    // OFFSET ROWS FETCH NEXT ROW Singular
    [InlineData(5u, 1u, "OFFSET 5 ROWS FETCH NEXT 1 ROW ONLY")]
    // OFFSET ROW Singular FETCH NEXT ROWs 
    [InlineData(1u, 15u, "OFFSET 1 ROW FETCH NEXT 15 ROWS ONLY")]
    // Both Offset is zero and Next is nonzero → expect full OFFSET-FETCH clause.
    [InlineData(0u, 15u, "OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY")]
    // Both Offset and Next are nonzero → expect full OFFSET-FETCH clause.
    [InlineData(5u, 15u, "OFFSET 5 ROWS FETCH NEXT 15 ROWS ONLY")]
    public void ToSql_ReturnsExpectedSql(uint offset, uint next, string expectedSql)
    {
        // Arrange
        OffsetFetchNext offsetNext = new(offset, next);

        // Act
        string actualSql = (Dialect).RenderPaging(offsetNext).ToSql();

        // Assert
        Assert.Equal(expectedSql, actualSql);
    }

    [Fact]
    public void ConversionConstructor_FromDefinePage_SetsPropertiesAndSqlCorrectly()
    {
        // Assume the DefinePage class calculates:
        //   Offset = (pageNumber - 1) * pageSize
        //   Next = pageSize
        // For pageNumber = 4 and pageSize = 25, we expect:
        //   Offset = (4 - 1) * 25 = 75, and Next = 25.
        uint pageNumber = 4;
        uint pageSize = 25;
        PagingBase offsetNext = new DefinePage(pageNumber, pageSize);

        // Also verify the SQL string generated.
        string expectedSql = $"OFFSET {75} ROWS FETCH NEXT {25} ROWS ONLY";

        Assert.Equal(expectedSql, (Dialect).RenderPaging(offsetNext).ToSql());
    }
}
