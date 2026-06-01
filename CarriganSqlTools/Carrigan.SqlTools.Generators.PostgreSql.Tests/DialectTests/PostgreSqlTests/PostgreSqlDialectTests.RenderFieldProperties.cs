using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Fact]
    public void RenderFieldProperties_EmptyProviderTypeName_ReturnsEmptyString()
    {
        FieldProperties fieldProperties = new();
        string actual = Dialect.RenderFieldProperties(fieldProperties);
        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void RenderFieldProperties_IntegerNotNullable_ReturnsIntegerNotNull()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsInteger());
        Assert.Equal("INTEGER NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_IntegerNullable_ReturnsIntegerNull()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsInteger(true));
        Assert.Equal("INTEGER NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_VarCharWithLength_ReturnsVarCharLengthNotNull()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsVarChar(50));
        Assert.Equal("VARCHAR(50) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_TextMax_DoesNotRenderMaxLength()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsText());
        Assert.Equal("TEXT NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_NumericWithPrecision_ReturnsNumericPrecisionNotNull()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsNumeric(18));
        Assert.Equal("NUMERIC(18) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_NumericWithPrecisionAndScale_ReturnsNumericPrecisionScaleNotNull()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsNumeric(18, 2));
        Assert.Equal("NUMERIC(18, 2) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_TimeWithPrecision_ReturnsPostgreSqlTemporalPrecisionFormat()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsTimeWithoutTimeZone(6));
        Assert.Equal("TIME(6) WITHOUT TIME ZONE NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_TimestampWithPrecision_ReturnsPostgreSqlTemporalPrecisionFormat()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsTimestampWithTimeZone(6));
        Assert.Equal("TIMESTAMP(6) WITH TIME ZONE NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_VectorWithDimensions_ReturnsVectorDimensionsNotNull()
    {
        string actual = Dialect.RenderFieldProperties(PostgreSqlTypesProvider.AsVector(1536));
        Assert.Equal("VECTOR(1536) NOT NULL", actual);
    }

    [Fact]
    public void RenderFieldProperties_NullFieldProperties_Exception() =>
        Assert.Throws<ArgumentNullException>(() => Dialect.RenderFieldProperties(null!));
}
