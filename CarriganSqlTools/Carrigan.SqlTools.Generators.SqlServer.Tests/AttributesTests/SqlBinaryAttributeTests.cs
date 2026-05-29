using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.AttributesTests;

public sealed class SqlBinaryAttributeTests
{
    [Theory]
    [InlineData(StorageTypeEnum.Fixed, "BINARY", "BINARY(8000)", 8000, false, true)]
    [InlineData(StorageTypeEnum.Var, "VARBINARY", "VARBINARY(MAX)", null, true, false)]
    public void Constructor_StorageType(StorageTypeEnum storageTypeEnum, string expectedProviderTypeName, string expectedTypeDeclaration, int? expectedLength, bool? expectedIsMax, bool? expectedIsFixedLength)
    {
        SqlBinaryAttribute sqlBinaryAttribute = new(storageTypeEnum);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlBinaryAttribute,
            expectedProviderTypeName,
            expectedTypeDeclaration,
            expectedLength: expectedLength,
            expectedIsMax: expectedIsMax,
            expectedIsFixedLength: expectedIsFixedLength);
    }

    [Theory]
    [InlineData(StorageTypeEnum.Fixed, 1, "BINARY", "BINARY(1)", true)]
    [InlineData(StorageTypeEnum.Fixed, 8000, "BINARY", "BINARY(8000)", true)]
    [InlineData(StorageTypeEnum.Var, 1, "VARBINARY", "VARBINARY(1)", false)]
    [InlineData(StorageTypeEnum.Var, 8000, "VARBINARY", "VARBINARY(8000)", false)]
    public void Constructor_WithSize(StorageTypeEnum storageTypeEnum, int size, string expectedProviderTypeName, string expectedTypeDeclaration, bool expectedIsFixedLength)
    {
        SqlBinaryAttribute sqlBinaryAttribute = new(storageTypeEnum, size);

        SqlTypeAttributeTestHelpers.AssertFieldProperties(
            sqlBinaryAttribute,
            expectedProviderTypeName,
            expectedTypeDeclaration,
            expectedLength: size,
            expectedIsMax: false,
            expectedIsFixedLength: expectedIsFixedLength);
    }

    [Fact]
    public void Constructor_UnsupportedStorageType_Exception()
    {
        StorageTypeEnum unsupportedValue = (StorageTypeEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlBinaryAttribute(unsupportedValue));
    }

    [Theory]
    [InlineData(StorageTypeEnum.Fixed, 0)]
    [InlineData(StorageTypeEnum.Fixed, 8001)]
    [InlineData(StorageTypeEnum.Var, 0)]
    [InlineData(StorageTypeEnum.Var, 8001)]
    public void Constructor_WithSize_OutOfRange_Exception(StorageTypeEnum storageTypeEnum, int size) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new SqlBinaryAttribute(storageTypeEnum, size));
}
