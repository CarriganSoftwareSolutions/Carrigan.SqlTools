using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlNumericAttributeTests
{
    [Fact]
    public void Constructor()
    {
        PostgreSqlNumericAttribute postgreSqlNumericAttribute = new();

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(postgreSqlNumericAttribute, "NUMERIC", "NUMERIC");
    }

    [Theory]
    [InlineData(null, "NUMERIC")]
    [InlineData((byte)1, "NUMERIC(1)")]
    [InlineData((byte)255, "NUMERIC(255)")]
    public void Constructor_WithPrecision(byte? precision, string expectedTypeDeclaration)
    {
        PostgreSqlNumericAttribute postgreSqlNumericAttribute = precision is null ? new() : new(precision.Value);

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(
            postgreSqlNumericAttribute,
            "NUMERIC",
            expectedTypeDeclaration,
            expectedPrecision: precision);
    }

    [Theory]
    [InlineData((byte)1, (byte)0, "NUMERIC(1, 0)")]
    [InlineData((byte)18, (byte)2, "NUMERIC(18, 2)")]
    [InlineData((byte)255, (byte)255, "NUMERIC(255, 255)")]
    public void Constructor_WithPrecisionAndScale(byte precision, byte scale, string expectedTypeDeclaration)
    {
        PostgreSqlNumericAttribute postgreSqlNumericAttribute = new(precision, scale);

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(
            postgreSqlNumericAttribute,
            "NUMERIC",
            expectedTypeDeclaration,
            expectedPrecision: precision,
            expectedScale: scale);
    }

    [Fact]
    public void Constructor_WithPrecision_OutOfRange_Exception() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PostgreSqlNumericAttribute(0));

    [Theory]
    [InlineData((byte)0, (byte)0)]
    [InlineData((byte)5, (byte)6)]
    public void Constructor_WithPrecisionAndScale_OutOfRange_Exception(byte precision, byte scale) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PostgreSqlNumericAttribute(precision, scale));
}
