using Carrigan.SqlTools.Extensions;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.ExtensionTests;

public class SetColumnsExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_NullSetColumns_ReturnsTrue()
    {
        // Arrange
        SetColumns<ColumnTable> setColumns = null;

        // Act
        bool result = setColumns.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_EmptyColumnNames_ReturnsTrue()
    {
        // Arrange: pass an empty IEnumerable<string>.
        SetColumns<ColumnTable> setColumns = new([]);

        // Act
        bool result = setColumns.IsNullOrEmpty();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_NonEmptyColumnNames_ReturnsFalse()
    {
        // Arrange: pass a non-empty IEnumerable<string> containing property names.
        List<string> propertyNames =
        [
            "Col1", "Col2", "ColA", "ColB", "Pizza", "D000descruct0", "Express"
        ];
        SetColumns<ColumnTable> setColumns = new(propertyNames);

        // Act
        bool result = setColumns.IsNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNotNullOrEmpty_NullSetColumns_ReturnsFalse()
    {
        // Arrange
        SetColumns<ColumnTable> setColumns = null;

        // Act
        bool result = setColumns.IsNotNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNotNullOrEmpty_EmptyColumnNames_ReturnsFalse()
    {
        // Arrange: pass an empty IEnumerable<string>.
        SetColumns<ColumnTable> setColumns = new([]);

        // Act
        bool result = setColumns.IsNotNullOrEmpty();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNotNullOrEmpty_NonEmptyColumnNames_ReturnsTrue()
    {
        // Arrange: pass a non-empty IEnumerable<string> containing property names.
        List<string> propertyNames =
        [
            "Col1", "Col2", "ColA", "ColB", "Pizza", "D000descruct0", "Express"
        ];
        SetColumns<ColumnTable> setColumns = new(propertyNames);

        // Act
        bool result = setColumns.IsNotNullOrEmpty();

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
        SetColumns<ColumnTable> setColumns = new(propertyNames);

        // Act
        bool result = setColumns.IsNotNullOrEmpty();

        // Assert
        Assert.True(result);
    }
}
