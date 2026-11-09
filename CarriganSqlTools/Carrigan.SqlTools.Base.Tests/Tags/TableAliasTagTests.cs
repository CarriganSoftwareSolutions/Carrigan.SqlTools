using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Base.Tests.Tags;

public class TableAliasTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData("DerivedTable", false)]
    public void New_NullOrEmpty_ReturnsExpectedValue(string? name, bool isNull)
    {
        TableAlias? alias = TableAlias.New(name);

        Assert.Equal(isNull, alias is null);
        if (alias is not null)
            Assert.Equal(name, alias.ToString());
    }

    [Fact]
    public void ToSql_UsesAliasIdentifier()
    {
        TableAlias alias = new("DerivedTable");

        Assert.Equal("DerivedTable", alias.ToSql(NeutralDialect.Instance));
    }

    [Fact]
    public void New_WithAlias_ReplacesTableNameAndRemovesSchema()
    {
        TableTag source = new("OriginalSchema", "OriginalTable");
        TableAlias alias = new("DerivedTable");

        TableTag actual = Assert.IsType<TableTag>(TableTag.New(source, alias));

        Assert.Null(actual.SchemaName);
        Assert.Equal(new TableName(alias), actual.TableName);
        Assert.Equal("DerivedTable", actual.ToString());
        Assert.Equal("OriginalSchema.OriginalTable", source.ToString());
    }

    [Fact]
    public void New_WithoutAlias_CopiesSourceTable()
    {
        TableTag source = new("OriginalSchema", "OriginalTable");

        TableTag actual = Assert.IsType<TableTag>(TableTag.New(source));

        Assert.NotSame(source, actual);
        Assert.Equal(source, actual);
    }

    [Fact]
    public void New_WithNoSourceOrAlias_ReturnsNull() =>
        Assert.Null(TableTag.New(null));

    [Fact]
    public void Equality_IgnoresCaseButPreservesDifferentNames()
    {
        TableAlias first = new("DerivedTable");
        TableAlias equivalent = new("derivedtable");
        TableAlias different = new("OtherTable");

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }
}
