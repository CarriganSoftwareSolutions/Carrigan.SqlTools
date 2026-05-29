using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlVarCharMaxAttributeTests
{
    [Theory]
    [InlineData(EncodingEnum.Ascii, "VARCHAR", "VARCHAR(MAX)", false)]
    [InlineData(EncodingEnum.Unicode, "NVARCHAR", "NVARCHAR(MAX)", true)]
    public void Constructor_EncodingEnum(EncodingEnum encoding, string expectedProviderTypeName, string expectedTypeDeclaration, bool expectedIsUnicode)
    {
        SqlVarCharMaxAttribute sqlVarCharMaxAttribute = new(encoding);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlVarCharMaxAttribute,
            expectedProviderTypeName,
            expectedTypeDeclaration,
            expectedIsMax: true,
            expectedIsUnicode: expectedIsUnicode,
            expectedIsFixedLength: false);
    }

    [Fact]
    public void Constructor_EncodingEnum_Exception()
    {
        EncodingEnum unsupportedEncoding = (EncodingEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlVarCharMaxAttribute(unsupportedEncoding));
    }
}
