using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderTable_WithSchema_ReturnsSchemaQualifiedTableName()
    {
        string actual = Dialect.RenderTable(new SchemaName("dbo"), new TableName("Customer"));

        Assert.Equal("[dbo].[Customer]", actual);
    }

    [Fact]
    public void RenderTable_WithoutSchema_ReturnsTableNameOnly()
    {
        string actual = Dialect.RenderTable(null, new TableName("Customer"));

        Assert.Equal("[Customer]", actual);
    }
}