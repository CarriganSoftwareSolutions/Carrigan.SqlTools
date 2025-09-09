using Carrigan.SqlTools.OffsetNexts;

namespace Carrigan.SqlTools.Tests.OffsetNextTests;

public class OffsetNextTests
{
    [Theory]
    // Both Offset and Next are 0 → expect an empty SQL string.
    [InlineData(0u, 0u, "")]
    // Next is 0, but Offset is nonzero → expect "OFFSET {Offset}"
    [InlineData(10u, 0u, "OFFSET 10")]
    // Both Offset is zero and Next is nonzero → expect full OFFSET-FETCH clause.
    [InlineData(0u, 15u, "OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY")]
    // Both Offset and Next are nonzero → expect full OFFSET-FETCH clause.
    [InlineData(5u, 15u, "OFFSET 5 ROWS FETCH NEXT 15 ROWS ONLY")]
    public void ToSql_ReturnsExpectedSql(uint offset, uint next, string expectedSql)
    {
        // Arrange
        OffsetNext offsetNext = new(offset, next);

        // Act
        string actualSql = offsetNext.ToSql();

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
        OffsetNext offsetNext = new DefinePage(pageNumber, pageSize);

        // Also verify the SQL string generated.
        string expectedSql = $"OFFSET {75} ROWS FETCH NEXT {25} ROWS ONLY";

        Assert.Equal(expectedSql, offsetNext.ToSql());
    }
}
