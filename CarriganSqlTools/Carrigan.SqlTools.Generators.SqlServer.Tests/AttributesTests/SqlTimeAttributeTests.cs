using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlTimeAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlTimeAttribute sqlTimeAttribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlTimeAttribute, "TIME", "TIME");
    }

    [Theory]
    [InlineData((byte)0, "TIME(0)")]
    [InlineData((byte)7, "TIME(7)")]
    public void Constructor_WithFractionalSecondPrecision(byte fractionalSecondPrecision, string expectedTypeDeclaration)
    {
        SqlTimeAttribute sqlTimeAttribute = new(fractionalSecondPrecision);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlTimeAttribute,
            "TIME",
            expectedTypeDeclaration,
            expectedFractionalSecondsPrecision: fractionalSecondPrecision);
    }

    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)9)]
    public void Constructor_WithFractionalSecondPrecision_Exception(byte fractionalSecondPrecision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlTimeAttribute(fractionalSecondPrecision));
}
