using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
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

        Assert.Equal([new ColumnName("Col1")], setColumns.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Constructor_WithValidColumns_ShouldSetColumnNames()
    {
        string[]  validColumns = ["Col1", "Col2", "Express"];

        SetColumns<ColumnTable> setColumns = new(validColumns);

        Assert.Equal(validColumns.Select(item => new ColumnName(item)), setColumns.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Constructor_WithAnInvalidColumn_ShouldThrowArgumentException()
    {
        string[] columns = ["Col1", "NotAColumn"];

        Assert.Throws<InvalidPropertyException<ColumnTable>>(() => new SetColumns<ColumnTable>(columns));
    }

    [Fact]
    public void Constructor_WithNoColumns_ShouldSetEmptyColumnNames()
    {
        // Arrange
        string[] columns = [];

        // Act
        SetColumns<ColumnTable> setColumns = new(columns);

        // Assert
        Assert.Empty(setColumns.ColumnInfo);
    }

    [Fact]
    public void AddColumnToEmpty()
    {
        SetColumns<ColumnTable> setColumns = new(Enumerable.Empty<string>());

        setColumns.AddColumn(nameof(ColumnTable.Col1));
        setColumns.AddColumn(nameof(ColumnTable.Col2));
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2")], setColumns.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Append()
    {
        SetColumns<ColumnTable> originalSet = new(nameof(ColumnTable.Col1), nameof(ColumnTable.Col2));
        SetColumns<ColumnTable> newSet = originalSet.AppendColumn(nameof(ColumnTable.ColA));

        //Original Unchanged
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2")], originalSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
        //NewSet has new column
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2"), new ColumnName("ColA")], newSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Concat()
    {
        SetColumns<ColumnTable> originalSet = new(nameof(ColumnTable.Col1), nameof(ColumnTable.Col2));
        SetColumns<ColumnTable> newSet = originalSet.ConcatColumn(nameof(ColumnTable.ColA), nameof(ColumnTable.ColB));

        //Original Unchanged
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2")], originalSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
        //NewSet has new column
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2"), new ColumnName("ColA"), new ColumnName("ColB")], newSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }
}
