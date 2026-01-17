using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Sets;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.SetsTests;

public class ColumnCollectionTests
{
    [Fact]
    public void Constructor_WithValidColumn_ShouldSetColumnName()
    {
        string propertyName = "Col1";

        ColumnCollection<ColumnTable> columnCollection = new(propertyName);

        Assert.Equal([new ColumnName("Col1")], columnCollection.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Constructor_WithValidColumns_ShouldSetColumnNames()
    {
        string[]  validColumns = ["Col1", "Col2", "Express"];

        ColumnCollection<ColumnTable> columnCollection = new(validColumns);

        Assert.Equal(validColumns.Select(item => new ColumnName(item)), columnCollection.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Constructor_WithAnInvalidColumn_ShouldThrowArgumentException()
    {
        string[] columns = ["Col1", "NotAColumn"];

        Assert.Throws<InvalidPropertyException<ColumnTable>>(() => new ColumnCollection<ColumnTable>(columns));
    }

    [Fact]
    public void Constructor_WithNoColumns_ShouldSetEmptyColumnNames()
    {
        // Arrange
        string[] columns = [];

        // Act
        ColumnCollection<ColumnTable> columnCollection = new(columns);

        // Assert
        Assert.Empty(columnCollection.ColumnInfo);
    }

    [Fact]
    public void AddColumnToEmpty()
    {
        ColumnCollection<ColumnTable> columnCollection = new(Enumerable.Empty<string>());

        columnCollection.AddColumn(nameof(ColumnTable.Col1));
        columnCollection.AddColumn(nameof(ColumnTable.Col2));
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2")], columnCollection.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Append()
    {
        ColumnCollection<ColumnTable> originalSet = new(nameof(ColumnTable.Col1), nameof(ColumnTable.Col2));
        ColumnCollection<ColumnTable> newSet = originalSet.AppendColumn(nameof(ColumnTable.ColA));

        //Original Unchanged
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2")], originalSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
        //NewSet has new column
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2"), new ColumnName("ColA")], newSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }

    [Fact]
    public void Concat()
    {
        ColumnCollection<ColumnTable> originalSet = new(nameof(ColumnTable.Col1), nameof(ColumnTable.Col2));
        ColumnCollection<ColumnTable> newSet = originalSet.ConcatColumn(nameof(ColumnTable.ColA), nameof(ColumnTable.ColB));

        //Original Unchanged
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2")], originalSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
        //NewSet has new column
        Assert.Equal([new ColumnName("Col1"), new ColumnName("Col2"), new ColumnName("ColA"), new ColumnName("ColB")], newSet.ColumnInfo.Select(columnTag => columnTag.ColumnName));
    }
    [Fact]
    public void Constructor_WithDuplicateColumns_ShouldNotDuplicate()
    {
        string[] columns = ["Col1", "Col1", "Col2"];

        ColumnCollection<ColumnTable> columnCollection = new(columns);

        Assert.Equal(
            [new ColumnName("Col1"), new ColumnName("Col2")],
            columnCollection.ColumnInfo.Select(column => column.ColumnName));
    }

    [Fact]
    public void AddColumn_WhenColumnAlreadyExists_ShouldNotDuplicate()
    {
        ColumnCollection<ColumnTable> columnCollection = new(nameof(ColumnTable.Col1));

        columnCollection.AddColumn(nameof(ColumnTable.Col1));

        Assert.Equal(
            [new ColumnName("Col1")],
            columnCollection.ColumnInfo.Select(column => column.ColumnName));
    }

    [Fact]
    public void ColumnInfo_ShouldBeMaterialized()
    {
        ColumnCollection<ColumnTable> columnCollection = new(nameof(ColumnTable.Col1));

        Assert.True(columnCollection.ColumnInfo is IReadOnlyCollection<ColumnInfo>);
    }

    [Fact]
    public void AppendColumn_ShouldReturnMaterializedCollection()
    {
        ColumnCollection<ColumnTable> originalSet = new(nameof(ColumnTable.Col1));
        ColumnCollection<ColumnTable> newSet = originalSet.AppendColumn(nameof(ColumnTable.Col2));

        Assert.True(newSet.ColumnInfo is IReadOnlyCollection<ColumnInfo>);
    }

    [Fact]
    public void ConcatColumn_ShouldReturnMaterializedCollection()
    {
        ColumnCollection<ColumnTable> originalSet = new(nameof(ColumnTable.Col1));
        ColumnCollection<ColumnTable> newSet = originalSet.ConcatColumn(nameof(ColumnTable.Col2), nameof(ColumnTable.ColA));

        Assert.True(newSet.ColumnInfo is IReadOnlyCollection<ColumnInfo>);
    }
}
