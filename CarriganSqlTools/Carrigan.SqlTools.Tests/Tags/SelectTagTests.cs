using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;
using System.Collections.Generic;

namespace Carrigan.SqlTools.Tests.Tags;
public class SelectTagTests
{
    //IGNORE SPELLING: dbo
    private static SelectTag New(string? schemaName, string tableName, string columnName, string? aliasName) =>
        New(SchemaName.New(schemaName), new TableName(tableName), new ColumnName(columnName), AliasName.New(aliasName));
    private static SelectTag New(SchemaName? schemaName, TableName tableName, ColumnName columnName, AliasName? aliasName) =>
        new (new ColumnTag(new TableTag(schemaName, tableName), columnName), AliasTag.New(aliasName));

    private static readonly SelectTag a = New(null, "SomeTable", "SomeColumn", null);
    private static readonly SelectTag b = New("dbo", "SomeTable", "SomeColumn", null);
    private static readonly SelectTag c = New(null, "SomeTable", "SomeColumn", "SomeAlias");
    private static readonly SelectTag d = New("dbo", "SomeTable", "SomeColumn", "SomeAlias");

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

    [Theory]
    [InlineData("Id", "Override", "[ColumnIdentifiers].[Id] AS Override")]
    [InlineData("Property", "Override", "[ColumnIdentifiers].[Property] AS Override")]
    [InlineData("ColumnName", "Override", "[ColumnIdentifiers].[Column] AS Override")]
    [InlineData("IdentifierName", "Override", "[ColumnIdentifiers].[Identifier] AS Override")]
    [InlineData("IdentifierOverrideName", "Override", "[ColumnIdentifiers].[IdentifierOverride] AS Override")]
    [InlineData("Id", null, "[ColumnIdentifiers].[Id]")]
    [InlineData("Property", null, "[ColumnIdentifiers].[Property]")]
    [InlineData("ColumnName", null, "[ColumnIdentifiers].[Column]")]
    [InlineData("IdentifierName", null, "[ColumnIdentifiers].[Identifier]")]
    [InlineData("IdentifierOverrideName", null, "[ColumnIdentifiers].[IdentifierOverride]")]
    public void GetFromColumnIdentifiers(string property, string? alias, string expected)
    {
        SelectTag select = SelectTag.Get<ColumnIdentifiers>(property, alias);
        Assert.Equal(expected, select);
        Assert.Equal(expected, select.ToString());
    }

    [Theory]
    [InlineData("Id", "Override", "[Table].[TableNameSchemaTable].[Id] AS Override")]
    [InlineData("Id", null, "[Table].[TableNameSchemaTable].[Id]")]
    public void GetFromTableNameSchema(string property, string? alias, string expected)
    {
        SelectTag select = SelectTag.Get<TableNameSchema>(property, alias);
        Assert.Equal(expected, select);
        Assert.Equal(expected, select.ToString());
    }

    [Theory]
    [InlineData("Id", "Override", "[Identifier].[IdentifierNameSchemaTable].[Id] AS Override")]
    [InlineData("Id", null, "[Identifier].[IdentifierNameSchemaTable].[Id]")]
    public void GetFromIdentifierNameSchema(string property, string? alias, string expected)
    {
        SelectTag select = SelectTag.Get<IdentifierNameSchema>(property, alias);
        Assert.Equal(expected, select);
        Assert.Equal(expected, select.ToString());
    }

    [Fact]
    public void GetManyFromTableWithAliases()
    {
        IEnumerable<string> expected = ["[TableWithAliases].[Id] AS TableId", "[TableWithAliases].[Name]"];
        IEnumerable<string> properties = ["Id", "Name"];
        IEnumerable<string> selects = SelectTag.GetMany<TableWithAliases>(properties).Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetManyFromColumnIdentifiers()
    {
        IEnumerable<string> expected = 
        [
            "[ColumnIdentifiers].[Id]", 
            "[ColumnIdentifiers].[Property]", 
            "[ColumnIdentifiers].[Column]", 
            "[ColumnIdentifiers].[Identifier]", 
            "[ColumnIdentifiers].[IdentifierOverride]"
        ];
        IEnumerable<string> properties = ["Id", "Property", "ColumnName", "IdentifierName", "IdentifierOverrideName"];
        IEnumerable<string> selects = SelectTag.GetMany<ColumnIdentifiers>(properties).Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetManyFromTableNameSchema()
    {
        IEnumerable<string> expected =
        [
            "[Table].[TableNameSchemaTable].[Id]",
            "[Table].[TableNameSchemaTable].[Text]"
        ];
        IEnumerable<string> properties = ["Id", "Text"];
        IEnumerable<string> selects = SelectTag.GetMany<TableNameSchema>(properties).Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetManyFromIdentifierNameSchema()
    {
        IEnumerable<string> expected =
        [
            "[Identifier].[IdentifierNameSchemaTable].[Id]",
            "[Identifier].[IdentifierNameSchemaTable].[Text]"
        ];
        IEnumerable<string> properties = ["Id", "Text"];
        IEnumerable<string> selects = SelectTag.GetMany<IdentifierNameSchema>(properties).Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetAllFromTableWithAliases()
    {
        IEnumerable<string> expected = ["[TableWithAliases].[Id] AS TableId", "[TableWithAliases].[Name]"];
        IEnumerable<string> selects = SelectTag.GetAll<TableWithAliases>().Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }
    [Fact]
    public void GetAllFromColumnIdentifiers()
    {
        IEnumerable<string> expected =
        [
            "[ColumnIdentifiers].[Id]",
            "[ColumnIdentifiers].[Property]",
            "[ColumnIdentifiers].[Column]",
            "[ColumnIdentifiers].[Identifier]",
            "[ColumnIdentifiers].[IdentifierOverride]"
        ];
        IEnumerable<string> selects = SelectTag.GetAll<ColumnIdentifiers>().Select(select => select.ToString());
    }

    [Fact]
    public void GetAllFromTableNameSchema()
    {
        IEnumerable<string> expected =
        [
            "[Table].[TableNameSchemaTable].[Id]",
            "[Table].[TableNameSchemaTable].[Text]"
        ];
        IEnumerable<string> selects = SelectTag.GetAll<TableNameSchema>().Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetAllFromIdentifierNameSchema()
    {
        IEnumerable<string> expected =
        [
            "[Identifier].[IdentifierNameSchemaTable].[Id]",
            "[Identifier].[IdentifierNameSchemaTable].[Text]"
        ];
        IEnumerable<string> selects = SelectTag.GetAll<IdentifierNameSchema>().Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Theory]
    [InlineData(null, "SomeTable", "SomeColumn", null, "[SomeTable].[SomeColumn]")]
    [InlineData("dbo", "SomeTable", "SomeColumn", null, "[dbo].[SomeTable].[SomeColumn]")]
    [InlineData(null, "SomeTable", "SomeColumn", "SomeAlias","[SomeTable].[SomeColumn] AS SomeAlias")]
    [InlineData("dbo", "SomeTable", "SomeColumn", "SomeAlias","[dbo].[SomeTable].[SomeColumn] AS SomeAlias")]
    public void Equality(string? schemaName, string tableName, string columnName, string? aliasName, string expectedSelect)
    {
        SelectTag select = New(schemaName, tableName, columnName, aliasName);
        SelectTag selectAlt = New(schemaName, tableName, columnName, aliasName);

        Assert.Equal(expectedSelect, select);
        Assert.Equal(expectedSelect, selectAlt);
        Assert.Equal(expectedSelect, select.ToString());
        Assert.Equal(expectedSelect, selectAlt.ToString());
        Assert.Equal(0, select.CompareTo(selectAlt));
        Assert.Equal(select, selectAlt);
        Assert.Equal(selectAlt, selectAlt);
        Assert.Equal(expectedSelect.GetHashCode(), select.GetHashCode());
        Assert.True(select == selectAlt);
        Assert.True(expectedSelect == selectAlt);
        Assert.Equal(expectedSelect, select.ToSql());
        Assert.True(select.Any());
        Assert.False(select.Empty());
    }

    [Fact]
    public void NotEqual()
    {
        Assert.NotEqual(a, b);
        Assert.NotEqual(a, c);
        Assert.NotEqual(a, d);
        Assert.NotEqual(b, c);
        Assert.NotEqual(b, d);
        Assert.NotEqual(c, d);

        Assert.NotEqual(0, a.CompareTo(b));
        Assert.NotEqual(0, a.CompareTo(c));
        Assert.NotEqual(0, a.CompareTo(d));
        Assert.NotEqual(0, b.CompareTo(c));
        Assert.NotEqual(0, c.CompareTo(d));

        Assert.True(a != b);
        Assert.True(a != c);
        Assert.True(a != d);
        Assert.True(b != c);
        Assert.True(b != d);
        Assert.True(c != d);
    }

    [Fact]
    public void Dictionary()
    {
        SelectTag aAlt = New(null, "SomeTable", "SomeColumn", null);
        SelectTag bAlt = New("dbo", "SomeTable", "SomeColumn", null);
        SelectTag cAlt = New(null, "SomeTable", "SomeColumn", "SomeAlias");
        SelectTag dAlt = New("dbo", "SomeTable", "SomeColumn", "SomeAlias");
        Dictionary<SelectTag, int> dictionary = [];

        dictionary[a] = 1;
        dictionary[b] = 2;
        dictionary[c] = 3;
        dictionary[d] = 4;

        Assert.Equal(1, dictionary[aAlt]);
        Assert.Equal(2, dictionary[bAlt]);
        Assert.Equal(3, dictionary[cAlt]);
        Assert.Equal(4, dictionary[dAlt]);
    }

    [Fact]
    public void InvalidGet_PropertyExceptionPropertyString() => 
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTag.Get<TableNameSchema>("NotAProperty", "ValidAlias"));

    [Fact]
    public void InvalidGet_PropertyExceptionPropertyName() => 
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTag.Get<TableNameSchema>(new("NotAProperty"), new("ValidAlias")));

    [Fact]
    public void InvalidGet_SqlIdentifierExceptionAliasString() =>
        Assert.Throws<InvalidSqlIdentifierException>(() => SelectTag.Get<TableNameSchema>("Id", "123"));

    [Fact]
    public void InvalidGet_PropertyExceptionAliasName() =>
        Assert.Throws<InvalidSqlIdentifierException>(() => SelectTag.Get<TableNameSchema>(new("Id"), new("123")));

    [Fact]
    public void ValidGet_FromString() =>
        _ = SelectTag.Get<TableNameSchema>("Id", "ValidAlias");

    [Fact]
    public void ValidGet_FromName() =>
        _ = SelectTag.Get<TableNameSchema>(new("Id"), new("ValidAlias"));


    [Fact]
    public void InvalidGetMany_PropertyExceptionPropertyString() =>
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTag.Get<TableNameSchema>("NotAProperty"));

    [Fact]
    public void InvalidGetMany_PropertyExceptionPropertyName() =>
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTag.Get<TableNameSchema>(new("NotAProperty")));
    [Fact]
    public void ValidGetMany_FromString() =>
        _ = SelectTag.Get<TableNameSchema>("Id", "Text");

    [Fact]
    public void All()
    {
        SelectTag a = New(null, "SomeTable", "SomeColumn", null);
        Assert.Equal(a, a.All().Single());
    }

    [Fact]
    public void GetTableTags()
    {
        Assert.Equal("[SomeTable]", a.GetTableTags().Single());
        Assert.Equal("[dbo].[SomeTable]", b.GetTableTags().Single());
        Assert.Equal("[SomeTable]", c.GetTableTags().Single());
        Assert.Equal("[dbo].[SomeTable]", d.GetTableTags().Single());
    }
}
