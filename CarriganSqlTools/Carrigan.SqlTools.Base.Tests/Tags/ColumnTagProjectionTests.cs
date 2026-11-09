using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Tags;

public class ColumnTagProjectionTests
{
    private static ColumnTag NewSourceColumn() =>
        new(new TableTag("SourceSchema", "SourceTable"), new ColumnName("SourceColumn"));

    [Fact]
    public void Constructor_SourceColumnWithoutAliases_RetainsQualifiedName()
    {
        ColumnTag source = NewSourceColumn();

        ColumnTag projected = new(source, null, null);

        Assert.NotSame(source, projected);
        Assert.Equal(source, projected);
        Assert.Equal("SourceSchema.SourceTable.SourceColumn", projected.ToString());
    }

    [Fact]
    public void Constructor_TableAlias_ReplacesSourceTableAndSchema()
    {
        ColumnTag source = NewSourceColumn();

        ColumnTag projected = new(source, new TableAlias("Derived"), null);

        Assert.Equal("Derived.SourceColumn", projected.ToString());
        Assert.Null(projected.TableTag?.SchemaName);
        Assert.Equal("SourceSchema.SourceTable.SourceColumn", source.ToString());
    }

    [Fact]
    public void Constructor_ColumnAlias_RenamesOnlyColumn()
    {
        ColumnTag source = NewSourceColumn();

        ColumnTag projected = new(source, null, new ColumnAlias("Projected"));

        Assert.Equal("SourceSchema.SourceTable.Projected", projected.ToString());
        Assert.Equal("SourceSchema.SourceTable.SourceColumn", source.ToString());
    }

    [Fact]
    public void Constructor_BothAliases_ProducesQualifiedProjectedColumn()
    {
        ColumnTag source = NewSourceColumn();

        ColumnTag projected = new(source, new TableAlias("Derived"), new ColumnAlias("Projected"));

        Assert.Equal("Derived.Projected", projected.ToString());
        Assert.Equal("Derived.Projected", projected.ToSql(NeutralDialect.Instance));
    }

    [Fact]
    public void Constructor_TableAndColumnAliases_BuildsResultColumn()
    {
        ColumnTag projected = new(new TableAlias("Derived"), new ColumnAlias("Projected"));

        Assert.Equal("Derived.Projected", projected.ToString());
        Assert.Equal("Derived", projected.TableTag?.TableName.ToString());
        Assert.Equal("Projected", projected.ColumnName.ToString());
    }

    [Fact]
    public void Constructor_ColumnAliasWithoutTable_ProducesUnqualifiedResultName()
    {
        ColumnTag projected = new((TableAlias?)null, new ColumnAlias("Projected"));

        Assert.Null(projected.TableTag);
        Assert.Equal("Projected", projected.ToString());
    }

    [Fact]
    public void ColumnName_ConstructorFromAlias_PreservesResultIdentifier()
    {
        ColumnAlias alias = new("Projected");

        ColumnName columnName = new(alias);

        Assert.Equal("Projected", columnName.ToString());
    }
}
