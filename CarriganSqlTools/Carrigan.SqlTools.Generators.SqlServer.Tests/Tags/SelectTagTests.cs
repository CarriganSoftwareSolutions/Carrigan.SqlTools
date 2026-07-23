using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.Tags;
public class SelectTagTests
{
    private static readonly SqlServerDialect Dialect = new();

    private static SelectTag New(string columnName, string? aliasName) =>
        new (new PropertyName(columnName), AliasName.New(aliasName));

    private static readonly SelectTagBase a = New("SomeColumn", null);
    private static readonly SelectTagBase b = New("OtherColumn", null);
    private static readonly SelectTagBase c = New("SomeColumn", "SomeAlias");
    private static readonly SelectTagBase d = New("OtherColumn", "SomeAlias");

    [Theory]
    [InlineData("SomeColumn", null, "[SomeColumn]", null, "[SomeColumn]")]
    [InlineData("SomeColumn", "SomeAlias", "[SomeColumn]", "SomeAlias", "[SomeColumn] AS [SomeAlias]")]
    public void Constructor(string columnName, string? aliasName, string expectedColumn, string? expectedAlias, string expectedSelect)
    {
        SelectTagBase selectTag  = New(columnName, aliasName);
        Assert.Equal(expectedColumn, selectTag.WithNoAlias().ToSql(Dialect));
        if (expectedAlias is null)
            Assert.Null(selectTag.AliasTag);

        Assert.Equal(expectedSelect, selectTag.ToSql(Dialect));
        Assert.True(selectTag.TableTags.Single().IsEmpty());
    }

    [Theory]
    [InlineData("Id", "Override", "[TableWithAliases].[Id] AS [Override]")]
    [InlineData("Name", "Override", "[TableWithAliases].[Name] AS [Override]")]
    [InlineData("Id", null, "[TableWithAliases].[Id] AS [TableId]")]
    [InlineData("Name", null, "[TableWithAliases].[Name]")]
    public void GetFromTableWithAliases(string property, string? alias, string expected)
    {
        SelectTagBase select = SelectTagGenerator.Get<TableWithAliases>(property, alias);
        Assert.Equal(expected, select.ToSql(Dialect));
    }

    [Theory]
    [InlineData("Id", "Override", "[ColumnIdentifiers].[Id] AS [Override]")]
    [InlineData("Property", "Override", "[ColumnIdentifiers].[Property] AS [Override]")]
    [InlineData("ColumnName", "Override", "[ColumnIdentifiers].[Column] AS [Override]")]
    [InlineData("IdentifierName", "Override", "[ColumnIdentifiers].[Identifier] AS [Override]")]
    [InlineData("IdentifierOverrideName", "Override", "[ColumnIdentifiers].[IdentifierOverride] AS [Override]")]
    [InlineData("Id", null, "[ColumnIdentifiers].[Id]")]
    [InlineData("Property", null, "[ColumnIdentifiers].[Property]")]
    [InlineData("ColumnName", null, "[ColumnIdentifiers].[Column]")]
    [InlineData("IdentifierName", null, "[ColumnIdentifiers].[Identifier]")]
    [InlineData("IdentifierOverrideName", null, "[ColumnIdentifiers].[IdentifierOverride]")]
    public void GetFromColumnIdentifiers(string property, string? alias, string expected)
    {
        SelectTagBase select = SelectTagGenerator.Get<ColumnIdentifiers>(property, alias);
        Assert.Equal(expected, select.ToSql(Dialect));
    }

    [Theory]
    [InlineData("Id", "Override", "[Table].[TableNameSchemaTable].[Id] AS [Override]")]
    [InlineData("Id", null, "[Table].[TableNameSchemaTable].[Id]")]
    public void GetFromTableNameSchema(string property, string? alias, string expected)
    {
        SelectTagBase select = SelectTagGenerator.Get<TableNameSchema>(property, alias);
        Assert.Equal(expected, select.ToSql(Dialect));
    }

    [Theory]
    [InlineData("Id", "Override", "Table.TableNameSchemaTable.Id AS Override")]
    [InlineData("Id", null, "Table.TableNameSchemaTable.Id")]
    public void GetFromTableNameSchema_String(string property, string? alias, string expected)
    {
        SelectTagBase select = SelectTagGenerator.Get<TableNameSchema>(property, alias);
        Assert.Equal(expected, select);
        Assert.Equal(expected, select.ToString());
    }

    [Theory]
    [InlineData("Id", "Override", "[Identifier].[IdentifierNameSchemaTable].[Id] AS [Override]")]
    [InlineData("Id", null, "[Identifier].[IdentifierNameSchemaTable].[Id]")]
    public void GetFromIdentifierNameSchema(string property, string? alias, string expected)
    {
        SelectTagBase select = SelectTagGenerator.Get<IdentifierNameSchema>(property, alias);
        Assert.Equal(expected, select.ToSql(Dialect));
    }

    [Theory]
    [InlineData("Id", "Override", "Identifier.IdentifierNameSchemaTable.Id AS Override")]
    [InlineData("Id", null, "Identifier.IdentifierNameSchemaTable.Id")]
    public void GetFromIdentifierNameSchema_String(string property, string? alias, string expected)
    {
        SelectTagBase select = SelectTagGenerator.Get<IdentifierNameSchema>(property, alias);
        Assert.Equal(expected, select);
        Assert.Equal(expected, select.ToString());
    }

    [Fact]
    public void GetManyFromTableWithAliases()
    {
        IEnumerable<string> expected = ["[TableWithAliases].[Id] AS [TableId]", "[TableWithAliases].[Name]"];
        IEnumerable<string> properties = ["Id", "Name"];
        IEnumerable<string> selects = SelectTagGenerator.GetMany<TableWithAliases>(properties).Select(select => select.ToSql(Dialect));
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetManyFromTableWithAliases_String()
    {
        IEnumerable<string> expected = ["TableWithAliases.Id AS TableId", "TableWithAliases.Name"];
        IEnumerable<string> properties = ["Id", "Name"];
        IEnumerable<string> selects = SelectTagGenerator.GetMany<TableWithAliases>(properties).Select(select => select.ToString());
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
        IEnumerable<string> selects = SelectTagGenerator.GetMany<ColumnIdentifiers>(properties).Select(select => select.ToSql(Dialect));
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetManyFromColumnIdentifiers_String()
    {
        IEnumerable<string> expected = 
        [
            "ColumnIdentifiers.Id", 
            "ColumnIdentifiers.Property", 
            "ColumnIdentifiers.Column", 
            "ColumnIdentifiers.Identifier", 
            "ColumnIdentifiers.IdentifierOverride"
        ];
        IEnumerable<string> properties = ["Id", "Property", "ColumnName", "IdentifierName", "IdentifierOverrideName"];
        IEnumerable<string> selects = SelectTagGenerator.GetMany<ColumnIdentifiers>(properties).Select(select => select.ToString());
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
        IEnumerable<string> selects = SelectTagGenerator.GetMany<TableNameSchema>(properties).Select(select => select.ToSql(Dialect));
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetManyFromTableNameSchema_String()
    {
        IEnumerable<string> expected =
        [
            "Table.TableNameSchemaTable.Id",
            "Table.TableNameSchemaTable.Text"
        ];
        IEnumerable<string> properties = ["Id", "Text"];
        IEnumerable<string> selects = SelectTagGenerator.GetMany<TableNameSchema>(properties).Select(select => select.ToString());
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
        IEnumerable<string> selects = SelectTagGenerator.GetMany<IdentifierNameSchema>(properties).Select(select => select.ToSql(Dialect));
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetManyFromIdentifierNameSchema_String()
    {
        IEnumerable<string> expected =
        [
            "Identifier.IdentifierNameSchemaTable.Id",
            "Identifier.IdentifierNameSchemaTable.Text"
        ];
        IEnumerable<string> properties = ["Id", "Text"];
        IEnumerable<string> selects = SelectTagGenerator.GetMany<IdentifierNameSchema>(properties).Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetAllFromTableWithAliases()
    {
        IEnumerable<string> expected = ["[TableWithAliases].[Id] AS [TableId]", "[TableWithAliases].[Name]"];
        IEnumerable<string> selects = SelectTagGenerator.GetAll<TableWithAliases>().Select(select => select.ToSql(Dialect));
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetAllFromTableWithAliases_String()
    {
        IEnumerable<string> expected = ["TableWithAliases.Id AS TableId", "TableWithAliases.Name"];
        IEnumerable<string> selects = SelectTagGenerator.GetAll<TableWithAliases>().Select(select => select.ToString());
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetAllFromSelectTagAttributes_UsesAttributeTables()
    {
        SelectTags selects = SelectTagGenerator.GetAll<SelectProjection>();

        IEnumerable<string> expectedSql =
        [
            "[SelectLeft].[Id] AS [LeftId]",
            "[SelectRight].[Name] AS [RightName]"
        ];

        IEnumerable<string> expectedTables = ["SelectLeft", "SelectRight"];

        Assert.Equal(expectedSql, selects.Select(select => select.ToSql(Dialect)));
        Assert.Equal(expectedTables, selects.GetTableTags().Select(static table => table.ToString()));
        Assert.DoesNotContain("SelectProjection", selects.GetTableTags().Select(static table => table.ToString()));
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
        IEnumerable<string> selects = SelectTagGenerator.GetAll<ColumnIdentifiers>().Select(select => select.ToString());
    }

    [Fact]
    public void GetAllFromTableNameSchema()
    {
        IEnumerable<string> expected =
        [
            "[Table].[TableNameSchemaTable].[Id]",
            "[Table].[TableNameSchemaTable].[Text]"
        ];
        IEnumerable<string> selects = SelectTagGenerator.GetAll<TableNameSchema>().Select(select => select.ToSql(Dialect));
        Assert.Equal(expected, selects);
    }

    [Fact]
    public void GetAllFromTableNameSchema_String()
    {
        IEnumerable<string> expected =
        [
            "Table.TableNameSchemaTable.Id",
            "Table.TableNameSchemaTable.Text"
        ];
        IEnumerable<string> selects = SelectTagGenerator.GetAll<TableNameSchema>().Select(select => select.ToString());
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
        IEnumerable<string> selects = SelectTagGenerator.GetAll<IdentifierNameSchema>().Select(select => select.ToSql(Dialect));
        Assert.Equal(expected, selects);
    }

    [Theory]
    [InlineData("SomeColumn", null, "[SomeColumn]")]
    [InlineData("SomeColumn", "SomeAlias", "[SomeColumn] AS [SomeAlias]")]
    public void Equality(string columnName, string? aliasName, string expectedSelect)
    {
        SelectTagBase select = New(columnName, aliasName);
        SelectTagBase selectAlt = New(columnName, aliasName);

        Assert.Equal(expectedSelect, select.ToSql(Dialect));
        Assert.Equal(expectedSelect, selectAlt.ToSql(Dialect));
        Assert.Equal(expectedSelect, select.ToSql(Dialect));
        Assert.Equal(expectedSelect, selectAlt.ToSql(Dialect));
        Assert.Equal(0, select.CompareTo(selectAlt));
        Assert.Equal(select, selectAlt);
        Assert.Equal(selectAlt, selectAlt);
        Assert.True(select == selectAlt);
        Assert.True(expectedSelect == selectAlt.ToSql(Dialect));
        Assert.Equal(expectedSelect, select.ToSql(Dialect));
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
        SelectTagBase aAlt = New("SomeColumn", null);
        SelectTagBase bAlt = New("OtherColumn", null);
        SelectTagBase cAlt = New("SomeColumn", "SomeAlias");
        SelectTagBase dAlt = New("OtherColumn", "SomeAlias");
        Dictionary<SelectTagBase, int> dictionary = [];

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
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTagGenerator.Get<TableNameSchema>("NotAProperty", "ValidAlias"));

    [Fact]
    public void InvalidGet_PropertyExceptionPropertyName() => 
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTagGenerator.Get<TableNameSchema>(new("NotAProperty"), new("ValidAlias")));

    [Fact]
    public void InvalidGet_SqlIdentifierExceptionAliasString() =>
        Assert.Throws<InvalidSqlIdentifierException>(() => SelectTagGenerator.Get<TableNameSchema>("Id", "123"));

    [Fact]
    public void InvalidGet_PropertyExceptionAliasName() =>
        Assert.Throws<InvalidSqlIdentifierException>(() => SelectTagGenerator.Get<TableNameSchema>(new("Id"), new("123")));

    [Fact]
    public void ValidGet_FromString() =>
        _ = SelectTagGenerator.Get<TableNameSchema>("Id", "ValidAlias");

    [Fact]
    public void ValidGet_FromName() =>
        _ = SelectTagGenerator.Get<TableNameSchema>(new("Id"), new("ValidAlias"));


    [Fact]
    public void InvalidGetMany_PropertyExceptionPropertyString() =>
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTagGenerator.Get<TableNameSchema>("NotAProperty"));

    [Fact]
    public void InvalidGetMany_PropertyExceptionPropertyName() =>
        Assert.Throws<InvalidPropertyException<TableNameSchema>>(() => SelectTagGenerator.Get<TableNameSchema>(new("NotAProperty")));
    [Fact]
    public void ValidGetMany_FromString() =>
        _ = SelectTagGenerator.Get<TableNameSchema>("Id", "Text");

    [Fact]
    public void SelectTagWithCast()
    {
        SelectTag selectTag = SelectTagGenerator.Get<SelectLeft>("Id", "Id", SqlServerTypesProvider.AsVarCharMax());
        SelectBuilder<SelectLeft> selectBuilder = new()
        {
            Selects = selectTag
        };

        SqlGenerator<SelectLeft> sqlGenerator = new();
        SqlQuery query = sqlGenerator.Select(selectBuilder);

        string expectedQuery = "SELECT CAST([SelectLeft].[Id] AS VARCHAR(MAX)) AS [Id] FROM [SelectLeft]";

        Assert.Equal(expectedQuery, query.QueryText);
    }

    private sealed class SelectLeft
    {
        public int Id { get; set; }
    }

    private sealed class SelectRight
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class SelectProjection
    {
        [SelectTag<SelectLeft>(nameof(SelectLeft.Id), nameof(LeftId))]
        public int? LeftId { get; set; }

        [SelectTag<SelectRight>(nameof(SelectRight.Name), nameof(RightName))]
        public string? RightName { get; set; }
    }
}
