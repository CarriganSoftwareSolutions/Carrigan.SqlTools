using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlTimestampAttributeTests
{
    [Fact]
    public void Constructor()
    {
        PostgreSqlTimestampAttribute postgreSqlTimestampAttribute = new();

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(postgreSqlTimestampAttribute, "TIMESTAMP WITHOUT TIME ZONE", "TIMESTAMP WITHOUT TIME ZONE");
    }

    [Theory]
    [InlineData((byte)0, "TIMESTAMP(0) WITHOUT TIME ZONE")]
    [InlineData((byte)6, "TIMESTAMP(6) WITHOUT TIME ZONE")]
    public void Constructor_WithFractionalSecondPrecision(byte fractionalSecondPrecision, string expectedTypeDeclaration)
    {
        PostgreSqlTimestampAttribute postgreSqlTimestampAttribute = new(fractionalSecondPrecision);

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(
            postgreSqlTimestampAttribute,
            "TIMESTAMP WITHOUT TIME ZONE",
            expectedTypeDeclaration,
            expectedFractionalSecondsPrecision: fractionalSecondPrecision);
    }

    [Theory]
    [InlineData((byte)7)]
    [InlineData((byte)8)]
    public void Constructor_WithFractionalSecondPrecision_OutOfRange_Exception(byte fractionalSecondPrecision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PostgreSqlTimestampAttribute(fractionalSecondPrecision));
}
