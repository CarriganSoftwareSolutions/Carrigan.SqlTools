using SqlTools.Sets;
using SqlToolsTests.TestEntities;

namespace SqlToolsTests.SetsTests;

public class SetColumnsTests
{
    [Fact]
    public void Constructor_WithValidColumn_ShouldSetColumnName()
    {
        // Arrange
        // These are valid because they match the property names of ColumnTable.
        string validColumn = "Col1";

        // Act
        SetColumns<ColumnTable> setColumns = new(validColumn);

        // Assert
        // The ColumnNames property should exactly match the provided valid columns.
        Assert.Equal(["Col1"], setColumns.ColumnNames);
    }

    [Fact]
    public void Constructor_WithValidColumns_ShouldSetColumnNames()
    {
        // Arrange
        // These are valid because they match the property names of ColumnTable.
        string[]  validColumns = ["Col1", "Col2", "Express"];

        // Act
        SetColumns<ColumnTable> setColumns = new(validColumns);

        // Assert
        // The ColumnNames property should exactly match the provided valid columns.
        Assert.Equal(validColumns, setColumns.ColumnNames);
    }

    [Fact]
    public void Constructor_WithAnInvalidColumn_ShouldThrowArgumentException()
    {
        // Arrange
        // "NotAColumn" is not a property of ColumnTable.
        string[] columns = ["Col1", "NotAColumn"];

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new SetColumns<ColumnTable>(columns));
    }

    [Fact]
    public void Constructor_WithMultipleInvalidColumns_ShouldThrowArgumentException()
    {
        // Arrange
        // Both "Invalid1" and "Invalid2" are not properties of ColumnTable.
        string[] columns = ["Invalid1", "Col2", "Invalid2"];

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new SetColumns<ColumnTable>(columns));
        Assert.Contains("Invalid1", exception.Message);
        Assert.Contains("Invalid2", exception.Message);
    }

    [Fact]
    public void Constructor_WithNoColumns_ShouldSetEmptyColumnNames()
    {
        // Arrange
        string[] columns = [];

        // Act
        SetColumns<ColumnTable> setColumns = new(columns);

        // Assert
        Assert.Empty(setColumns.ColumnNames);
    }

    [Fact]
    public void AddColumnToEmpty()
    {
        SetColumns<ColumnTable> setColumns = new();

        setColumns.AddColumn(nameof(ColumnTable.Col1));
        setColumns.AddColumn(nameof(ColumnTable.Col2));
        Assert.Equal(["Col1", "Col2"], setColumns.ColumnNames);
    }
}
