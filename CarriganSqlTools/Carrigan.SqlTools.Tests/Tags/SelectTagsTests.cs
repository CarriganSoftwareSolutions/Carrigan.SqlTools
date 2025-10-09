using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Tests.TestEntities;
using Carrigan.SqlTools.Tests.TestEntities.Attributes;

namespace Carrigan.SqlTools.Tests.Tags;
public class SelectTagsTests
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

    private static string aExpectedString = "[SomeTable].[SomeColumn]";
    private static string bExpectedString = "[dbo].[SomeTable].[SomeColumn]";
    private static string cExpectedString = "[SomeTable].[SomeColumn] AS SomeAlias";
    private static string dExpectedString = "[dbo].[SomeTable].[SomeColumn] AS SomeAlias";

    [Fact]
    public void Empty()
    {
        SelectTags selectTags = new ();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
    }

    [Fact]
    public void NotEmptyNew()
    {
        SelectTags selectTags = new(d);
        Assert.False(selectTags.Empty());
        Assert.True(selectTags.Any());
        Assert.Single(selectTags.All());

        Assert.Equal(dExpectedString, selectTags.GetSelects());

        Assert.Equal("[dbo].[SomeTable]", selectTags.GetTableTags().Single());
    }

    [Fact]
    public void NotEmptyAppendProperty()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Append<Order>("Id", "Override");

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());

        Assert.Equal("[Order].[Id] AS Override", selectTagsAlpha.GetSelects());

        Assert.Equal("[Order]", selectTagsAlpha.GetTableTags().Single());
    }

    [Fact]
    public void NotEmptyAppendPropertyName()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Append<Order>(new("Id"), new("Override"));

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());

        Assert.Equal("[Order].[Id] AS Override", selectTagsAlpha.GetSelects());

        Assert.Equal("[Order]", selectTagsAlpha.GetTableTags().Single());
    }

    [Fact]
    public void NotEmptyAppend()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Append(a);

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());

        Assert.Equal(aExpectedString, selectTagsAlpha.GetSelects());

        Assert.Equal("[SomeTable]", selectTagsAlpha.GetTableTags().Single());;
    }

    [Fact]
    public void NotEmptyConcat()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat(a, b, c, d);

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());
        Assert.Equal(4, selectTagsAlpha.All().Count());

        Assert.Equal($"{aExpectedString}, {bExpectedString}, {cExpectedString}, {dExpectedString}", selectTagsAlpha.GetSelects());

        Assert.Equal("[SomeTable]", selectTagsAlpha.GetTableTags().ElementAt(0));
        Assert.Equal("[dbo].[SomeTable]", selectTagsAlpha.GetTableTags().ElementAt(1));
    }

    [Fact]
    public void NotEmptyConcatSelects()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat(a, b, c, d);
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(4, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"{aExpectedString}, {bExpectedString}, {cExpectedString}, {dExpectedString}", selectTagsBeta.GetSelects());

        Assert.Equal("[SomeTable]", selectTagsBeta.GetTableTags().ElementAt(0));
        Assert.Equal("[dbo].[SomeTable]", selectTagsBeta.GetTableTags().ElementAt(1));
    }

    [Fact]
    public void NotEmptyConcatPropertyString()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat<Order>("Id", "CustomerId", "PaymentMethodId", "OrderDate", "Total");
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(5, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"[Order].[Id], [Order].[CustomerId], [Order].[PaymentMethodId], [Order].[OrderDate], [Order].[Total]", selectTagsBeta.GetSelects());

        Assert.Equal("[Order]", selectTagsBeta.GetTableTags().Single());
    }

    [Fact]
    public void NotEmptyConcatPropertiesFromGets()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat
        (
            SelectTags.Get<Order>("Id", "Override"),
            SelectTags.Get<Order>("CustomerId", "Override2"),
            SelectTags.Get<Order>("PaymentMethodId"),
            SelectTags.Get<Order>("OrderDate"),
            SelectTags.Get<Order>("Total")
        );
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(5, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"[Order].[Id] AS Override, [Order].[CustomerId] AS Override2, [Order].[PaymentMethodId], [Order].[OrderDate], [Order].[Total]", selectTagsBeta.GetSelects());

        Assert.Equal("[Order]", selectTagsBeta.GetTableTags().Single());
    }

    [Fact]
    public void NotEmptyConcatPropertiesFromGetMany()
    {
        SelectTags selectTags = new();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat
        (
            SelectTags.GetMany<Order>("Id", "CustomerId", "PaymentMethodId", "OrderDate", "Total")
        );
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(5, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"[Order].[Id], [Order].[CustomerId], [Order].[PaymentMethodId], [Order].[OrderDate], [Order].[Total]", selectTagsBeta.GetSelects());

        Assert.Equal("[Order]", selectTagsBeta.GetTableTags().Single());
    }
}
