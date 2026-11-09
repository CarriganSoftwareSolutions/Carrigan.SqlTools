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
        TableTag tableAliasTag = Assert.IsType<TableTag>(column.ColumnTag.TableTag);
        Assert.Equal(tableAlias.ToString(), tableAliasTag.TableName.ToString());
        Assert.Null(tableAliasTag.SchemaName);
        Assert.Equal("ProjectedName", column.ColumnTag.ColumnName.ToString());
        Assert.Null(selectTag.AliasTag);
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


    [Fact]
    public void FromTableAlias_ProjectsMultipleColumnsInSourceOrder()
    {
        Column firstColumn = new(new ColumnTag(new TableTag(null, "Source"), new ColumnName("First")));
        Column secondColumn = new(new ColumnTag(new TableTag(null, "Source"), new ColumnName("Second")));
        SelectTags original = new(new SelectTag(firstColumn, "Renamed"), new SelectTag(secondColumn, "Other"));

        SelectTags projected = original.FromTableAlias(new TableAlias("Derived"));

        Assert.Equal(["Derived.Renamed", "Derived.Other"], projected.Select(static tag => tag.ToString()));
        Assert.Equal(["Source.First AS Renamed", "Source.Second AS Other"], original.Select(static tag => tag.ToString()));
    }

    [Fact]
    public void FromTableAlias_AliasedExpression_UsesProjectedColumnName()
    {
        SelectTag expression = new(new Parameter(1, "Value"), "Computed");
        SelectTags original = new(expression);

        SelectTag projected = Assert.Single(original.FromTableAlias(new TableAlias("Derived")));

        Column column = Assert.IsType<Column>(projected.SqlExpression);
        Assert.Equal("Derived.Computed", column.ColumnTag.ToString());
        Assert.Null(projected.AliasTag);
    }

    private static SelectTags NewSelectTags(string? aliasName = null)
    {
        Column column = new(new ColumnTag(new TableTag(null, "SourceTable"), new ColumnName("SourceColumn")));
        return aliasName is null
            ? new SelectTag(column)
            : new SelectTag(column, aliasName);
    }
}
