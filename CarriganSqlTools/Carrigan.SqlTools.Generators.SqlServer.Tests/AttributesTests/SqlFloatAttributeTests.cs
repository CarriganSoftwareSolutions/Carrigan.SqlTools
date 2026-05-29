using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlFloatAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlFloatAttribute sqlFloatAttribute = new();

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlFloatAttribute, "FLOAT", "FLOAT");
    }

    [Theory]
    [InlineData((byte)1, "FLOAT(1)")]
    [InlineData((byte)24, "FLOAT(24)")]
    [InlineData((byte)53, "FLOAT(53)")]
    public void Constructor_WithPrecision(byte precision, string expectedTypeDeclaration)
    {
        SqlFloatAttribute sqlFloatAttribute = new(precision);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlFloatAttribute,
            "FLOAT",
            expectedTypeDeclaration,
            expectedPrecision: precision);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)54)]
    public void Constructor_WithPrecision_Exception(byte precision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlFloatAttribute(precision));
}
