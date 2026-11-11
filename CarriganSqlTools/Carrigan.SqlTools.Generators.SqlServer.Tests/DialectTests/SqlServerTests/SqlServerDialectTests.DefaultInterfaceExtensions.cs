using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderTableAlias_WithAlias_ReturnsQuotedAlias()
    {
        string actual = Dialect.RenderTableAlias(new TableAlias("c"));

        Assert.Equal("[c]", actual);
    }

    [Fact]
    public void RenderTableAlias_NullAlias_ReturnsEmptyString()
    {
        string actual = Dialect.RenderTableAlias(null);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void RenderColumnAlias_WithAlias_ReturnsQuotedAlias()
    {
        string actual = Dialect.RenderColumnAlias(new ColumnAlias("CustomerName"));

        Assert.Equal("[CustomerName]", actual);
    }

    [Fact]
    public void RenderColumnAlias_NullAlias_ReturnsEmptyString()
    {
        string actual = Dialect.RenderColumnAlias(null);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void RenderProcedureTag_WithSchema_ReturnsSchemaQualifiedName()
    {
        ProcedureTag procedureTag = new("dbo", "UpdateCustomer");

        string actual = Dialect.RenderProcedureTag(procedureTag);

        Assert.Equal("[dbo].[UpdateCustomer]", actual);
    }

    [Fact]
    public void RenderProcedureTag_WithoutSchema_ReturnsProcedureName()
    {
        ProcedureTag procedureTag = new(null, "UpdateCustomer");

        string actual = Dialect.RenderProcedureTag(procedureTag);

        Assert.Equal("[UpdateCustomer]", actual);
    }
}
