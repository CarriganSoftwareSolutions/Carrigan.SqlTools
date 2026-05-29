using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderColumn_WithTable_ReturnsTableQualifiedColumnName()
    {
        TableTag tableTag = new("dbo", "Customer");

        string actual = Dialect.RenderColumn(tableTag, new ColumnName("Name"));

        Assert.Equal("[dbo].[Customer].[Name]", actual);
    }

    [Fact]
    public void RenderColumn_WithoutTable_ReturnsColumnNameOnly()
    {
        TableTag tableTag = new("dbo", "Customer");

        string actual = Dialect.RenderColumn(tableTag, new ColumnName("Name"), false);

        Assert.Equal("[Name]", actual);
    }
}