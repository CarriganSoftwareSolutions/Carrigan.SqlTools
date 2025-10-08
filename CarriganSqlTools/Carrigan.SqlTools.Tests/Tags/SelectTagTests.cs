using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.Tags;
public class SelectTagTests
{
    //IGNORE SPELLING: dbo
    private static SelectTag New(string? schemaName, string tableName, string columnName, string? aliasName) =>
        New(SchemaName.New(schemaName), new TableName(tableName), new ColumnName(columnName), AliasName.New(aliasName));
    private static SelectTag New(SchemaName? schemaName, TableName tableName, ColumnName columnName, AliasName? aliasName) =>
        new (new ColumnTag(new TableTag(schemaName, tableName), columnName), AliasTag.New(aliasName));

    [Theory]
    [InlineData(null, "SomeTable", "SomeColumn", null,
        "[SomeTable]", "[SomeTable].[SomeColumn]", null, "[SomeTable].[SomeColumn]")]
    [InlineData("dbo", "SomeTable", "SomeColumn", null,
        "[dbo].[SomeTable]", "[dbo].[SomeTable].[SomeColumn]", null, "[dbo].[SomeTable].[SomeColumn]")]
    [InlineData(null, "SomeTable", "SomeColumn", "SomeAlias",
        "[SomeTable]", "[SomeTable].[SomeColumn]", "SomeAlias", "[SomeTable].[SomeColumn] AS SomeAlias")]
    [InlineData("dbo", "SomeTable", "SomeColumn", "SomeAlias",
        "[dbo].[SomeTable]", "[dbo].[SomeTable].[SomeColumn]", "SomeAlias", "[dbo].[SomeTable].[SomeColumn] AS SomeAlias")]
    public void Constructor(string? schemaName, string tableName, string columnName, string? aliasName, 
        string expectedTable, string expectedColumn, string? expectedAlias, string expectedSelect)
    {
        SelectTag selectTag  = New(schemaName, tableName, columnName, aliasName);
        Assert.Equal(expectedColumn, selectTag.ColumnTag);
        if (expectedAlias is null)
            Assert.Null(selectTag.AliasTag);

        Assert.Equal(expectedSelect, selectTag);
        Assert.Equal(expectedTable, selectTag.ColumnTag.TableTag);
    }

    [Theory]
    [InlineData("Id", "Override", "[TableWithAliases].[Id] AS Override")]
    [InlineData("Name", "Override", "[TableWithAliases].[Name] AS Override")]
    [InlineData("Id", null, "[TableWithAliases].[Id] AS TableId")]
    [InlineData("Name", null, "[TableWithAliases].[Name]")]
    public void GetFromTableWithAliases(string property, string? alias, string expected)
    {
        SelectTag select = SelectTag.Get<TableWithAliases>(property, alias);
        Assert.Equal(expected, select);
        Assert.Equal(expected, select.ToString());
    }
}
