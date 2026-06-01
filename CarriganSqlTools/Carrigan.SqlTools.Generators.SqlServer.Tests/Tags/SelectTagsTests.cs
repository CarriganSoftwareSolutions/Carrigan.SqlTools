using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.Tags;
public class SelectTagsTests
{
    private static readonly SqlServerDialect Dialect = new();

    private static SelectTagBase New(string columnName, string? aliasName) =>
        new SelectTag(new PropertyName(columnName), AliasName.New(aliasName));

    private static readonly SelectTagBase a = New("SomeColumn", null);
    private static readonly SelectTagBase b = New("OtherColumn", null);
    private static readonly SelectTagBase c = New("SomeColumn", "SomeAlias");
    private static readonly SelectTagBase d = New("OtherColumn", "SomeAlias");

    private static readonly string aExpectedString = "[SomeColumn]";
    private static readonly string bExpectedString = "[OtherColumn]";
    private static readonly string cExpectedString = "[SomeColumn] AS [SomeAlias]";
    private static readonly string dExpectedString = "[OtherColumn] AS [SomeAlias]";

    [Fact]
    public void ResultColumnNames() 
    {
        ResultColumnName aName = a.ResultColumnName;
        ResultColumnName bName = b.ResultColumnName;
        ResultColumnName cName = c.ResultColumnName;
        ResultColumnName dName = d.ResultColumnName;


        string aExpectedName = "SomeColumn";
        string bExpectedName = "OtherColumn";
        string cExpectedName = "SomeAlias";
        string dExpectedName = "SomeAlias";

        Assert.Equal(aExpectedName, aName);
        Assert.Equal(bExpectedName, bName);
        Assert.Equal(cExpectedName, cName);
        Assert.Equal(dExpectedName, dName);
    }

    [Fact]
    public void Empty()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
    }

    [Fact]
    public void NotEmptyNew()
    {
        SelectTags selectTags = new SelectTags(d);
        Assert.False(selectTags.Empty());
        Assert.True(selectTags.Any());
        Assert.Single(selectTags.All());

        Assert.Equal(dExpectedString, selectTags.ToSql(Dialect));

        Assert.True(selectTags.GetTableTags().Single().IsEmpty());
    }

    [Fact]
    public void NotEmptyAppendProperty()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Append<Order>("Id", "Override");

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());

        Assert.Equal("[Order].[Id] AS [Override]", selectTagsAlpha.ToSql(Dialect));

        Assert.Equal("[Order]", selectTagsAlpha.GetTableTags().Single().ToSql(Dialect));
    }

    [Fact]
    public void NotEmptyAppendPropertyName()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Append<Order>(new("Id"), new("Override"));

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());

        Assert.Equal("[Order].[Id] AS [Override]", selectTagsAlpha.ToSql(Dialect));

        Assert.Equal("[Order]", selectTagsAlpha.GetTableTags().Single().ToSql(Dialect));
    }

    [Fact]
    public void NotEmptyAppend()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Append(a);

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());

        Assert.Equal(aExpectedString, selectTagsAlpha.ToSql(Dialect));

        Assert.True(selectTagsAlpha.GetTableTags().Single().IsEmpty());
    }

    [Fact]
    public void NotEmptyConcat()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat(a, b, c, d);

        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());

        Assert.False(selectTagsAlpha.Empty());
        Assert.True(selectTagsAlpha.Any());
        Assert.Equal(4, selectTagsAlpha.All().Count());

        Assert.Equal($"{aExpectedString}, {bExpectedString}, {cExpectedString}, {dExpectedString}", selectTagsAlpha.ToSql(Dialect));

        Assert.True(selectTagsAlpha.GetTableTags().Single().IsEmpty());
    }

    [Fact]
    public void NotEmptyConcatSelects()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat(a, b, c, d);
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(4, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"{aExpectedString}, {bExpectedString}, {cExpectedString}, {dExpectedString}", selectTagsBeta.ToSql(Dialect));

        Assert.True(selectTagsBeta.GetTableTags().Single().IsEmpty());
    }

    [Fact]
    public void NotEmptyConcatPropertyString()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat<Order>("Id", "CustomerId", "PaymentMethodId", "OrderDate", "Total");
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(5, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"[Order].[Id], [Order].[CustomerId], [Order].[PaymentMethodId], [Order].[OrderDate], [Order].[Total]", selectTagsBeta.ToSql(Dialect));

        Assert.Equal("[Order]", selectTagsBeta.GetTableTags().Single().ToSql(Dialect));
    }

    [Fact]
    public void NotEmptyConcatPropertiesFromGets()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat
        (
            SelectTagGenerator.Get<Order>("Id", "Override"),
            SelectTagGenerator.Get<Order>("CustomerId", "Override2"),
            SelectTagGenerator.Get<Order>("PaymentMethodId"),
            SelectTagGenerator.Get<Order>("OrderDate"),
            SelectTagGenerator.Get<Order>("Total")
        );
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(5, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"[Order].[Id] AS [Override], [Order].[CustomerId] AS [Override2], [Order].[PaymentMethodId], [Order].[OrderDate], [Order].[Total]", selectTagsBeta.ToSql(Dialect));

        Assert.Equal("[Order]", selectTagsBeta.GetTableTags().Single().ToSql(Dialect));
    }

    [Fact]
    public void NotEmptyConcatPropertiesFromGetMany()
    {
        SelectTags selectTags = new SelectTags();
        Assert.True(selectTags.Empty());
        Assert.False(selectTags.Any());
        SelectTags selectTagsAlpha = selectTags.Concat
        (
            SelectTagGenerator.GetMany<Order>("Id", "CustomerId", "PaymentMethodId", "OrderDate", "Total")
        );
        SelectTags selectTagsBeta = selectTags.Concat(selectTagsAlpha);

        Assert.True(selectTags.Empty()); //didn't modify original
        Assert.False(selectTags.Any());

        Assert.False(selectTagsBeta.Empty());
        Assert.True(selectTagsBeta.Any());

        Assert.Equal(5, selectTagsAlpha.All().Count()); //forming beta didn't modify alpha

        Assert.Equal($"[Order].[Id], [Order].[CustomerId], [Order].[PaymentMethodId], [Order].[OrderDate], [Order].[Total]", selectTagsBeta.ToSql(Dialect));

        Assert.Equal("[Order]", selectTagsBeta.GetTableTags().Single().ToSql(Dialect));
    }

    [Fact]
    public void AppendInvalidPropertyStringException() =>
        Assert.Throws<InvalidPropertyException<Order>>((Func<object?>)(() => (new SqlTools.Tags.SelectTags()).Append<Order>("InvalidColumn")));
    [Fact]
    public void AppendInvalidAliasStringException() =>
        Assert.Throws<InvalidSqlIdentifierException>((Func<object?>)(() => (new SqlTools.Tags.SelectTags()).Append<Order>("Id", "123Invalid")));

    [Fact]
    public void AppendInvalidPropertyNameException() =>
        Assert.Throws<InvalidPropertyException<Order>>((Func<object?>)(() => (new SqlTools.Tags.SelectTags()).Append<Order>(new("InvalidColumn"))));
    [Fact]
    public void AppendInvalidAliasNameException() =>
        Assert.Throws<InvalidSqlIdentifierException>((Func<object?>)(() => (new SqlTools.Tags.SelectTags()).Append<Order>(new("Id"), new("123Invalid"))));


    [Fact]
    public void ConcatInvalidPropertyStringException() =>
        Assert.Throws<InvalidPropertyException<Order>>((Func<object?>)(() => (new SqlTools.Tags.SelectTags()).Concat<Order>("InvalidColumn")));
    [Fact]
    public void ConcatInvalidPropertyNameException() =>
        Assert.Throws<InvalidPropertyException<Order>>((Func<object?>)(() => (new SqlTools.Tags.SelectTags()).Concat<Order>(new PropertyName("InvalidColumn"))));


    [Fact]
    public void GetInvalidPropertyStringException() =>
        Assert.Throws<InvalidPropertyException<Order>>(() => SelectTagGenerator.Get<Order>("InvalidColumn"));
    [Fact]
    public void GetInvalidAliasStringException() =>
        Assert.Throws<InvalidSqlIdentifierException>(() => SelectTagGenerator.Get<Order>("Id", "123Invalid"));
    [Fact]
    public void GetInvalidPropertyNameException() =>
        Assert.Throws<InvalidPropertyException<Order>>(() => SelectTagGenerator.Get<Order>(new ("InvalidColumn")));

    [Fact]
    public void GetInvalidAliasNameException() =>
        Assert.Throws<InvalidSqlIdentifierException>(() => SelectTagGenerator.Get<Order>(new("Id"), new("123Invalid")));

    [Fact]
    public void GetManyInvalidPropertyStringException() =>
        Assert.Throws<InvalidPropertyException<Order>>(() => SelectTagGenerator.GetMany<Order>("InvalidColumn"));
    [Fact]
    public void GetManyInvalidPropertyNameException() =>
        Assert.Throws<InvalidPropertyException<Order>>(() => SelectTagGenerator.GetMany<Order>(new PropertyName("InvalidColumn")));
}
