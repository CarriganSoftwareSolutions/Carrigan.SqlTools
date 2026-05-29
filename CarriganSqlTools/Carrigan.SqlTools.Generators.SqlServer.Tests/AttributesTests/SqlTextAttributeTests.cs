using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlTextAttributeTests
{
    [Theory]
    [InlineData(EncodingEnum.Ascii, "TEXT", "TEXT", false)]
    [InlineData(EncodingEnum.Unicode, "NTEXT", "NTEXT", true)]
    public void Constructor_EncodingEnum(EncodingEnum encoding, string expectedProviderTypeName, string expectedTypeDeclaration, bool expectedIsUnicode)
    {
        SqlTextAttribute sqlTextAttribute = new(encoding);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlTextAttribute,
            expectedProviderTypeName,
            expectedTypeDeclaration,
            expectedIsMax: false,
            expectedIsUnicode: expectedIsUnicode,
            expectedIsFixedLength: false);
    }

    [Fact]
    public void Constructor_EncodingEnum_Exception()
    {
        EncodingEnum unsupportedEncoding = (EncodingEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlTextAttribute(unsupportedEncoding));
    }
}
