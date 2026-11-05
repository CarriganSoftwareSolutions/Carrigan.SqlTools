using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Expressions;

public class ColumnTagConstructorTests
{
    private static readonly SchemaName? SchemaName = SchemaName.New("TestSchema");
    private static readonly TableName TableName = new("TestTable");
    private static readonly ColumnName ColumnName = new("TestColumn");

    private static ColumnNameTag NewColumnTag() =>
        new(new TableTag(SchemaName, TableName), ColumnName);

    [Fact]
    public void Constructor_ColumnTag_SetsColumnTag()
    {
        ColumnNameTag columnTag = NewColumnTag();

        Column column = new(columnTag);

        Assert.Same(columnTag, column.ColumnTag);
    }

    [Fact]
    public void Constructor_ColumnTag_SetsColumnParameterTagFromColumnName()
    {
        ColumnNameTag columnTag = NewColumnTag();

        Column column = new(columnTag);

        Assert.Equal(ColumnName.ToString(), column.ColumnParameterTag.ToString());
    }

    [Fact]
    public void Constructor_ColumnTag_SetsLeafTableFromColumnTag()
    {
        ColumnNameTag columnTag = NewColumnTag();

        Column column = new(columnTag);

        TableTagBase actual = Assert.Single(column.LeafTables);
        Assert.Same(columnTag.TableTag, actual);
    }
}
