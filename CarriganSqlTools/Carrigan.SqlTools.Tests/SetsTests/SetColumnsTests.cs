using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.SetsTests;

public class SetColumnsTests
{
    [Fact]
    public void Constructor_WithValidColumn_ShouldSetColumnName()
    {
        string propertyName = "Col1";

        SetColumns<ColumnTable> setColumns = new(propertyName);

        Assert.Equal(["Col1"], setColumns.ColumnTags.Select(columnTag => columnTag._columnName));
    }

    [Fact]
    public void Constructor_WithValidColumns_ShouldSetColumnNames()
    {
        string[]  validColumns = ["Col1", "Col2", "Express"];

        SetColumns<ColumnTable> setColumns = new(validColumns);

        Assert.Equal(validColumns, setColumns.ColumnTags.Select(columnTag => columnTag._columnName));
    }

    [Fact]
    public void Constructor_WithAnInvalidColumn_ShouldThrowArgumentException()
    {
        string[] columns = ["Col1", "NotAColumn"];

        ArgumentException exception = Assert.Throws<ArgumentException>(() => new SetColumns<ColumnTable>(columns));
    }

    [Fact]
    public void Constructor_WithMultipleInvalidColumns_ShouldThrowArgumentException()
    {
        string[] columns = ["Invalid1", "Col2", "Invalid2"];

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
        Assert.Empty(setColumns.ColumnTags);
    }

    [Fact]
    public void AddColumnToEmpty()
    {
        SetColumns<ColumnTable> setColumns = new();

        setColumns.AddColumn(nameof(ColumnTable.Col1));
        setColumns.AddColumn(nameof(ColumnTable.Col2));
        Assert.Equal(["Col1", "Col2"], setColumns.ColumnTags.Select(columnTag => columnTag._columnName));
    }
}
