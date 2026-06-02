using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlFloatAttributeTests
{
    [Theory]
    [InlineData(null, "FLOAT")]
    [InlineData((byte)1, "FLOAT(1)")]
    [InlineData((byte)24, "FLOAT(24)")]
    [InlineData((byte)53, "FLOAT(53)")]
    public void Constructor_WithPrecision(byte? precision, string expectedTypeDeclaration)
    {
        PostgreSqlFloatAttribute postgreSqlFloatAttribute = precision is null ? new() : new(precision.Value);

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(
            postgreSqlFloatAttribute,
            "FLOAT",
            expectedTypeDeclaration,
            expectedPrecision: precision);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)54)]
    public void Constructor_WithPrecision_OutOfRange_Exception(byte precision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PostgreSqlFloatAttribute(precision));
}
