using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.ExtensionTests;

public class ColumnCollectionExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_NullColumnCollection_ReturnsTrue()
    {
        // Arrange
        ColumnCollection<ColumnTable>? columnCollection = null;

        // Act
        bool result = columnCollection.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_EmptyColumnNames_ReturnsTrue()
    {
        // Arrange: pass an empty IEnumerable<string>.
        ColumnCollection<ColumnTable> columnCollection = new(Enumerable.Empty<string>());

        // Act
        bool result = columnCollection.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_NonEmptyColumnNames_ReturnsFalse()
    {
        // Arrange: pass a non-empty IEnumerable<string> containing property names.
        List<string> propertyNames =
        [
            "Col1", "Col2", "ColA", "ColB", "Pizza", "D000destruct0", "Express"
        ];
        ColumnCollection<ColumnTable> columnCollection = new(propertyNames);

        // Act
        bool result = columnCollection.IsNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNotNullOrEmpty_NullColumnCollection_ReturnsFalse()
    {
        // Arrange
        ColumnCollection<ColumnTable>? columnCollection = null;

        // Act
        bool result = columnCollection.IsNotNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNotNullOrEmpty_EmptyColumnNames_ReturnsFalse()
    {
        // Arrange: pass an empty IEnumerable<string>.
        ColumnCollection<ColumnTable> columnCollection = new(Enumerable.Empty<string>());

        // Act
        bool result = columnCollection.IsNotNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNotNullOrEmpty_NonEmptyColumnNames_ReturnsTrue()
    {
        // Arrange: pass a non-empty IEnumerable<string> containing property names.
        List<string> propertyNames =
        [
            "Col1", "Col2", "ColA", "ColB", "Pizza", "D000destruct0", "Express"
        ];
        ColumnCollection<ColumnTable> columnCollection = new(propertyNames);

        // Act
        bool result = columnCollection.IsNotNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNotNullOrEmpty_WithReflectionPropertyNames_ReturnsTrue()
    {
        // Arrange: obtain property names from ColumnTable using reflection.
        IEnumerable<string> propertyNames = typeof(ColumnTable)
            .GetProperties()
            .Select(p => p.Name);
        ColumnCollection<ColumnTable> columnCollection = new(propertyNames);

        // Act
        bool result = columnCollection.IsNotNullOrEmpty();

        // Assert
        Assert.True(result);
    }
}
