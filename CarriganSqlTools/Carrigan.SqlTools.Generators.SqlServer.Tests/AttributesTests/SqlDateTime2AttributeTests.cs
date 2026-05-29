using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlDateTime2AttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDateTime2Attribute sqlDateTime2Attribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlDateTime2Attribute, "DATETIME2", "DATETIME2");
    }

    [Theory]
    [InlineData((byte)0, "DATETIME2(0)")]
    [InlineData((byte)7, "DATETIME2(7)")]
    public void Constructor_WithFractionalSecondPrecision(byte fractionalSecondPrecision, string expectedTypeDeclaration)
    {
        SqlDateTime2Attribute sqlDateTime2Attribute = new(fractionalSecondPrecision);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlDateTime2Attribute,
            "DATETIME2",
            expectedTypeDeclaration,
            expectedFractionalSecondsPrecision: fractionalSecondPrecision);
    }

    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)9)]
    public void Constructor_WithFractionalSecondPrecision_Exception(byte fractionalSecondPrecision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlDateTime2Attribute(fractionalSecondPrecision));
}
