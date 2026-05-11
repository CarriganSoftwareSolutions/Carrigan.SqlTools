using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderTable_WithoutSchema_ReturnsQuotedTable()
    {
        string actual = Dialect.RenderTable(null, new TableName("Customer"));
        Assert.Equal("\"Customer\"", actual);
    }

    [Fact]
    public void RenderTable_WithSchema_ReturnsQuotedSchemaAndTable()
    {
        string actual = Dialect.RenderTable(new SchemaName("public"), new TableName("Customer"));
        Assert.Equal("\"public\".\"Customer\"", actual);
    }
}
