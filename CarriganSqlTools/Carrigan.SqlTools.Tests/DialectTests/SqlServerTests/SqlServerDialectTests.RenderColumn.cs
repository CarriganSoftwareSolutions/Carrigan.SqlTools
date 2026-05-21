using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderColumn_WithTable_ReturnsTableQualifiedColumnName()
    {
        SqlServerDialect dialect = new();
        TableTag tableTag = new("dbo", "Customer");

        string actual = dialect.RenderColumn(tableTag, new ColumnName("Name"));

        Assert.Equal("[dbo].[Customer].[Name]", actual);
    }

    [Fact]
    public void RenderColumn_WithoutTable_ReturnsColumnNameOnly()
    {
        SqlServerDialect dialect = new();
        TableTag tableTag = new("dbo", "Customer");

        string actual = dialect.RenderColumn(tableTag, new ColumnName("Name"), false);

        Assert.Equal("[Name]", actual);
    }
}