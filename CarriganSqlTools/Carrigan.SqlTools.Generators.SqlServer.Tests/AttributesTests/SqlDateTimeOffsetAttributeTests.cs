using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlDateTimeOffsetAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDateTimeOffsetAttribute sqlDateTimeOffsetAttribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlDateTimeOffsetAttribute, "DATETIMEOFFSET", "DATETIMEOFFSET");
    }

    [Theory]
    [InlineData((byte)0, "DATETIMEOFFSET(0)")]
    [InlineData((byte)7, "DATETIMEOFFSET(7)")]
    public void Constructor_WithFractionalSecondPrecision(byte fractionalSecondPrecision, string expectedTypeDeclaration)
    {
        SqlDateTimeOffsetAttribute sqlDateTimeOffsetAttribute = new(fractionalSecondPrecision);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlDateTimeOffsetAttribute,
            "DATETIMEOFFSET",
            expectedTypeDeclaration,
            expectedFractionalSecondsPrecision: fractionalSecondPrecision);
    }

    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)9)]
    public void Constructor_WithFractionalSecondPrecision_Exception(byte fractionalSecondPrecision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlDateTimeOffsetAttribute(fractionalSecondPrecision));
}
