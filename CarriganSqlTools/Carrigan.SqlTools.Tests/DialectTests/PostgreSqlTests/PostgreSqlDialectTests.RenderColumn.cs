using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderColumn_IncludeTable_ReturnsTableAndQuotedColumn()
    {
        string actual = Dialect.RenderColumn(new TableTag(null, "Customer"), new ColumnName("Name"));
        Assert.Equal("\"Customer\".\"Name\"", actual);
    }

    [Fact]
    public void RenderColumn_DoNotIncludeTable_ReturnsQuotedColumn()
    {
        string actual = Dialect.RenderColumn(new TableTag(null, "Customer"), new ColumnName("Name"), false);
        Assert.Equal("\"Name\"", actual);
    }
}
