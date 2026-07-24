using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.SqlServerTests;

public partial class SqlServerDialectTests
{
    [Fact]
    public void RenderCastType_ReturnsEmptyString_WhenProviderTypeNameIsMissing()
    {
        string expected = string.Empty;

        string actual = Dialect.RenderCastType(new FieldProperties());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderCastType_RendersLengthWithoutNullability()
    {
        string expected = "NVARCHAR(100)";

        string actual = Dialect.RenderCastType(SqlServerTypesProvider.AsNVarChar(100, true));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderCastType_RendersMaxWithoutNullability()
    {
        string expected = "NVARCHAR(MAX)";

        string actual = Dialect.RenderCastType(SqlServerTypesProvider.AsNVarCharMax(true));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderCastType_RendersPrecisionAndScaleWithoutNullability()
    {
        string expected = "DECIMAL(18, 2)";

        string actual = Dialect.RenderCastType(SqlServerTypesProvider.AsDecimal(18, 2, true));

        Assert.Equal(expected, actual);
    }
    [Fact]
    public void RenderCastType_Throws_WhenProviderTypeNameContainsSqlSyntax() =>
        Assert.Throws<ArgumentException>(() => Dialect.RenderCastType(new FieldProperties
        {
            ProviderTypeName = "INT); DROP TABLE AuditLog; --"
        }));

    [Fact]
    public void RenderCastType_Throws_WhenVectorBaseTypeContainsSqlSyntax() =>
        Assert.Throws<ArgumentException>(() => Dialect.RenderCastType(new FieldProperties
        {
            ProviderTypeName = "VECTOR",
            Length = 3,
            BaseType = "FLOAT32); DROP TABLE AuditLog; --"
        }));

}
