using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
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
        string expected = "VARCHAR(100)";

        string actual = Dialect.RenderCastType(PostgreSqlTypesProvider.AsVarChar(100, false, true));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderCastType_RendersPrecisionAndScaleWithoutNullability()
    {
        string expected = "NUMERIC(18, 2)";

        string actual = Dialect.RenderCastType(PostgreSqlTypesProvider.AsNumeric(18, 2, false, true));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RenderCastType_RendersTemporalArrayWithoutNullability()
    {
        string expected = "TIMESTAMP(6) WITH TIME ZONE[]";

        string actual = Dialect.RenderCastType(PostgreSqlTypesProvider.AsTimestampWithTimeZone(6, true, true));

        Assert.Equal(expected, actual);
    }
}
