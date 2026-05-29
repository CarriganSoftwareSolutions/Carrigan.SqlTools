using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlDecimalAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDecimalAttribute sqlDecimalAttribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlDecimalAttribute, "DECIMAL", "DECIMAL");
    }

    [Theory]
    [InlineData((byte)1, "DECIMAL(1)")]
    [InlineData((byte)38, "DECIMAL(38)")]
    public void Constructor_WithPrecision(byte precision, string expectedTypeDeclaration)
    {
        SqlDecimalAttribute sqlDecimalAttribute = new(precision);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlDecimalAttribute,
            "DECIMAL",
            expectedTypeDeclaration,
            expectedPrecision: precision);
    }

    [Theory]
    [InlineData((byte)1, (byte)0, "DECIMAL(1, 0)")]
    [InlineData((byte)18, (byte)2, "DECIMAL(18, 2)")]
    [InlineData((byte)38, (byte)38, "DECIMAL(38, 38)")]
    public void Constructor_WithPrecisionAndScale(byte precision, byte scale, string expectedTypeDeclaration)
    {
        SqlDecimalAttribute sqlDecimalAttribute = new(precision, scale);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlDecimalAttribute,
            "DECIMAL",
            expectedTypeDeclaration,
            expectedPrecision: precision,
            expectedScale: scale);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)39)]
    public void Constructor_WithPrecision_OutOfRange_Exception(byte precision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlDecimalAttribute(precision));

    [Theory]
    [InlineData((byte)0, (byte)0)]
    [InlineData((byte)39, (byte)0)]
    [InlineData((byte)5, (byte)6)]
    public void Constructor_WithPrecisionAndScale_OutOfRange_Exception(byte precision, byte scale) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlDecimalAttribute(precision, scale));
}
