using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.AttributesTests;

public sealed class PostgreSqlCharAttributeTests
{
    [Theory]
    [InlineData(StorageTypeEnum.Fixed, null, "CHAR", "CHAR", true)]
    [InlineData(StorageTypeEnum.Fixed, 1, "CHAR", "CHAR(1)", true)]
    [InlineData(StorageTypeEnum.Fixed, 10_485_760, "CHAR", "CHAR(10485760)", true)]
    [InlineData(StorageTypeEnum.Var, null, "VARCHAR", "VARCHAR", false)]
    [InlineData(StorageTypeEnum.Var, 1, "VARCHAR", "VARCHAR(1)", false)]
    [InlineData(StorageTypeEnum.Var, 10_485_760, "VARCHAR", "VARCHAR(10485760)", false)]
    public void Constructor_WithLength(StorageTypeEnum storageTypeEnum, int? length, string expectedProviderTypeName, string expectedTypeDeclaration, bool expectedIsFixedLength)
    {
        PostgreSqlCharAttribute postgreSqlCharAttribute = length is null ? new(storageTypeEnum) : new(storageTypeEnum, length.Value);

        PostgreSqlTypeAttributeTestHelpers.AssertFieldProperties(
            postgreSqlCharAttribute,
            expectedProviderTypeName,
            expectedTypeDeclaration,
            expectedLength: length,
            expectedIsUnicode: true,
            expectedIsFixedLength: expectedIsFixedLength);
    }

    [Theory]
    [InlineData(StorageTypeEnum.Fixed, 0)]
    [InlineData(StorageTypeEnum.Fixed, 10_485_761)]
    [InlineData(StorageTypeEnum.Var, 0)]
    [InlineData(StorageTypeEnum.Var, 10_485_761)]
    public void Constructor_WithLength_OutOfRange_Exception(StorageTypeEnum storageTypeEnum, int length) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PostgreSqlCharAttribute(storageTypeEnum, length));

    [Fact]
    public void Constructor_UnsupportedStorageType_Exception()
    {
        StorageTypeEnum unsupportedValue = (StorageTypeEnum)999;

        Assert.Throws<NotSupportedException>(() => new PostgreSqlCharAttribute(unsupportedValue, 1));
    }
}
