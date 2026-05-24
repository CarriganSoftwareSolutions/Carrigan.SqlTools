using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderTable_WithSchema_ReturnsSchemaQualifiedTableName()
    {
        SqlServerDialect dialect = new();

        string actual = dialect.RenderTable(new SchemaName("dbo"), new TableName("Customer"));

        Assert.Equal("[dbo].[Customer]", actual);
    }

    [Fact]
    public void RenderTable_WithoutSchema_ReturnsTableNameOnly()
    {
        SqlServerDialect dialect = new();

        string actual = dialect.RenderTable(null, new TableName("Customer"));

        Assert.Equal("[Customer]", actual);
    }
}