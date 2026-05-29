using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlCharAttributeTests
{
    [Theory]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, "CHAR", "CHAR(8000)", 8000, false, false, true)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, "VARCHAR", "VARCHAR(MAX)", null, true, false, false)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, "NCHAR", "NCHAR(4000)", 4000, false, true, true)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, "NVARCHAR", "NVARCHAR(MAX)", null, true, true, false)]
    public void Constructor_EncodingAndStorageType(
        EncodingEnum encoding,
        StorageTypeEnum storageTypeEnum,
        string expectedProviderTypeName,
        string expectedTypeDeclaration,
        int? expectedLength,
        bool? expectedIsMax,
        bool? expectedIsUnicode,
        bool? expectedIsFixedLength)
    {
        SqlCharAttribute sqlCharAttribute = new(encoding, storageTypeEnum);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlCharAttribute,
            expectedProviderTypeName,
            expectedTypeDeclaration,
            expectedLength: expectedLength,
            expectedIsMax: expectedIsMax,
            expectedIsUnicode: expectedIsUnicode,
            expectedIsFixedLength: expectedIsFixedLength);
    }

    [Theory]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 1, "CHAR", "CHAR(1)", false, true)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 8000, "CHAR", "CHAR(8000)", false, true)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 1, "VARCHAR", "VARCHAR(1)", false, false)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 8000, "VARCHAR", "VARCHAR(8000)", false, false)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 1, "NCHAR", "NCHAR(1)", true, true)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 4000, "NCHAR", "NCHAR(4000)", true, true)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 1, "NVARCHAR", "NVARCHAR(1)", true, false)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 4000, "NVARCHAR", "NVARCHAR(4000)", true, false)]
    public void Constructor_WithSize(
        EncodingEnum encoding,
        StorageTypeEnum storageTypeEnum,
        int size,
        string expectedProviderTypeName,
        string expectedTypeDeclaration,
        bool expectedIsUnicode,
        bool expectedIsFixedLength)
    {
        SqlCharAttribute sqlCharAttribute = new(encoding, storageTypeEnum, size);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlCharAttribute,
            expectedProviderTypeName,
            expectedTypeDeclaration,
            expectedLength: size,
            expectedIsMax: false,
            expectedIsUnicode: expectedIsUnicode,
            expectedIsFixedLength: expectedIsFixedLength);
    }

    [Theory]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 0)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 8001)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 0)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 8001)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 0)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 4001)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 0)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 4001)]
    public void Constructor_WithSize_OutOfRange_Exception(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int size) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlCharAttribute(encoding, storageTypeEnum, size));

    [Fact]
    public void Constructor_UnsupportedStorageType()
    {
        EncodingEnum encoding = EncodingEnum.Ascii;
        StorageTypeEnum storageTypeEnum = (StorageTypeEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlCharAttribute(encoding, storageTypeEnum));
    }

    [Fact]
    public void Constructor_UnsupportedEncoding()
    {
        EncodingEnum encoding = (EncodingEnum)999;
        StorageTypeEnum storageTypeEnum = StorageTypeEnum.Fixed;

        Assert.Throws<NotSupportedException>(() => new SqlCharAttribute(encoding, storageTypeEnum));
    }
}
