using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlTimestampUtcAttributeTests
{
    [Fact]
    public void Constructor()
    {
        PostgreSqlTimestampUtcAttribute postgreSqlTimestampUtcAttribute = new();

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(postgreSqlTimestampUtcAttribute, "TIMESTAMP WITH TIME ZONE", "TIMESTAMP WITH TIME ZONE");
    }

    [Theory]
    [InlineData((byte)0, "TIMESTAMP(0) WITH TIME ZONE")]
    [InlineData((byte)6, "TIMESTAMP(6) WITH TIME ZONE")]
    public void Constructor_WithFractionalSecondPrecision(byte fractionalSecondPrecision, string expectedTypeDeclaration)
    {
        PostgreSqlTimestampUtcAttribute postgreSqlTimestampUtcAttribute = new(fractionalSecondPrecision);

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(
            postgreSqlTimestampUtcAttribute,
            "TIMESTAMP WITH TIME ZONE",
            expectedTypeDeclaration,
            expectedFractionalSecondsPrecision: fractionalSecondPrecision);
    }

    [Theory]
    [InlineData((byte)7)]
    [InlineData((byte)8)]
    public void Constructor_WithFractionalSecondPrecision_OutOfRange_Exception(byte fractionalSecondPrecision) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PostgreSqlTimestampUtcAttribute(fractionalSecondPrecision));
}
