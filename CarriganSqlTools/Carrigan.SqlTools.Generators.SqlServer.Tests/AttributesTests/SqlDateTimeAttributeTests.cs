using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlDateTimeAttributeTests
{
    [Theory]
    [InlineData(SizeableEnum.Regular, "DATETIME", "DATETIME")]
    [InlineData(SizeableEnum.Smaller, "SMALLDATETIME", "SMALLDATETIME")]
    public void Constructor_SizeableEnum(SizeableEnum sizeableEnum, string expectedProviderTypeName, string expectedTypeDeclaration)
    {
        SqlDateTimeAttribute sqlDateTimeAttribute = new(sizeableEnum);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(sqlDateTimeAttribute, expectedProviderTypeName, expectedTypeDeclaration);
    }

    [Fact]
    public void Constructor_SizeableEnum_Exception()
    {
        SizeableEnum unsupportedValue = (SizeableEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlDateTimeAttribute(unsupportedValue));
    }
}
