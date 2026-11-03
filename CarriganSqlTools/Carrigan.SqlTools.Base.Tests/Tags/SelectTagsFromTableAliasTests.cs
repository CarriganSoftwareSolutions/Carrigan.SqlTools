using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Tags;

public class SelectTagsFromTableAliasTests
{
    [Fact]
    public void FromTableAlias_Null_ReturnsCurrentInstance()
    {
        SelectTags selectTags = NewSelectTags();

        SelectTags actual = selectTags.FromTableAlias(null);

        Assert.Same(selectTags, actual);
    }

    [Fact]
    public void FromTableAlias_ProjectsResultColumnThroughAlias()
    {
        SelectTags selectTags = NewSelectTags("ProjectedName");
        TableAlias tableAlias = new ("DerivedTable");

        SelectTags actual = selectTags.FromTableAlias(tableAlias);

        SelectTag selectTag = Assert.Single(actual);
        Column column = Assert.IsType<Column>(selectTag.SqlExpression);
        TableAliasTag tableAliasTag = Assert.IsType<TableAliasTag>(column.ColumnTag.TableTag);
        Assert.Same(tableAlias, tableAliasTag.TableAlias);
        Assert.Equal("ProjectedName", column.ColumnTag.ColumnName.ToString());
        Assert.Equal("DerivedTable.ProjectedName", selectTag.ToSql(NeutralDialect.Instance));
    }

    [Fact]
    public void FromTableAlias_WithoutSelectAlias_UsesColumnResultName()
    {
        SelectTags selectTags = NewSelectTags();

        SelectTag actual = Assert.Single(selectTags.FromTableAlias(new TableAlias("DerivedTable")));

        Column column = Assert.IsType<Column>(actual.SqlExpression);
        Assert.Equal("SourceColumn", column.ColumnTag.ColumnName.ToString());
    }

    private static SelectTags NewSelectTags(string? aliasName = null)
    {
        Column column = new(new ColumnTag(new TableTag(null, "SourceTable"), new ColumnName("SourceColumn")));
        return aliasName is null
            ? new SelectTag(column)
            : new SelectTag(column, aliasName);
    }
}
