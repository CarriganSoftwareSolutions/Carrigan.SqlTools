using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlTimeAttributeTests
{
    [Fact]
    public void Constructor()
    {
        PostgreSqlTimeAttribute postgreSqlTimeAttribute = new();

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(postgreSqlTimeAttribute, "TIME WITHOUT TIME ZONE", "TIME WITHOUT TIME ZONE");
    }

    [Theory]
    [InlineData((byte)0, "TIME(0) WITHOUT TIME ZONE")]
    [InlineData((byte)6, "TIME(6) WITHOUT TIME ZONE")]
    public void Constructor_WithFractionalSecondPrecision(byte fractionalSecondPrecision, string expectedTypeDeclaration)
    {
        PostgreSqlTimeAttribute postgreSqlTimeAttribute = new(fractionalSecondPrecision);

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(
            postgreSqlTimeAttribute,
            "TIME WITHOUT TIME ZONE",
            expectedTypeDeclaration,
            expectedFractionalSecondsPrecision: fractionalSecondPrecision);
    }

    [Theory]
    [InlineData((byte)7)]
    [InlineData((byte)8)]
    public void Constructor_WithFractionalSecondPrecision_OutOfRange_Exception(byte fractionalSecondPrecision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PostgreSqlTimeAttribute(fractionalSecondPrecision));
}
