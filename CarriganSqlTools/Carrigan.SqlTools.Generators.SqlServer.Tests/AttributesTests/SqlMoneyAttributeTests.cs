using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlMoneyAttributeTests
{
    [Theory]
    [InlineData(SizeableEnum.Regular, "MONEY", "MONEY")]
    [InlineData(SizeableEnum.Smaller, "SMALLMONEY", "SMALLMONEY")]
    public void Constructor_SizeableEnum(SizeableEnum moneySize, string expectedProviderTypeName, string expectedTypeDeclaration)
    {
        SqlMoneyAttribute sqlMoneyAttribute = new(moneySize);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlMoneyAttribute, expectedProviderTypeName, expectedTypeDeclaration);
    }

    [Fact]
    public void Constructor_SizeableEnum_Exception()
    {
        SizeableEnum unsupportedValue = (SizeableEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlMoneyAttribute(unsupportedValue));
    }
}
