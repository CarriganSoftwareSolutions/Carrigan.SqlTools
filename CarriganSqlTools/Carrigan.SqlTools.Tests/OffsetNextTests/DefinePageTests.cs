using Carrigan.SqlTools.OffsetNexts;

namespace Carrigan.SqlTools.Tests.OffsetNextTests;

public class DefinePageTests
{
    [Theory]
    [InlineData(1, 10, 10, 0)]   // page 1 with pageSize 10 => Offset = 0
    [InlineData(2, 10, 10, 10)]  // page 2 with pageSize 10 => Offset = 10
    [InlineData(3, 20, 20, 40)]  // page 3 with pageSize 20 => Offset = 40
    public void Constructor_WithValidParameters_SetsLimitAndOffsetCorrectly(uint pageNumber, uint pageSize, uint expectedLimit, uint expectedOffset)
    {
        // Act
        DefinePage definePage = new(pageNumber, pageSize);

        // Assert
        Assert.Equal(expectedLimit, definePage.Next);
        Assert.Equal(expectedOffset, definePage.Offset);
    }

    [Theory]
    [InlineData(0, 10)]    // Invalid: pageNumber is 0
    [InlineData(1, 0)]     // Invalid: pageSize is 0
    [InlineData(0, 0)]     // Invalid: pageSize is 0
    public void Constructor_InvalidParameters_ThrowsArgumentOutOfRangeException(uint pageNumber, uint pageSize) =>
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new DefinePage(pageNumber, pageSize));
}
