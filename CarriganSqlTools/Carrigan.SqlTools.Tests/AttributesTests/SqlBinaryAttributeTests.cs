using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlBinaryAttributeTests
{
    [Theory]
    [InlineData(StorageTypeEnum.Fixed, SqlDbType.Binary, "BINARY")]
    [InlineData(StorageTypeEnum.Var, SqlDbType.VarBinary, "VARBINARY")]
    public void Constructor_StorageType(StorageTypeEnum storageTypeEnum, SqlDbType expectedSqlDbType, string expectedTypeDeclaration)
    {
        SqlBinaryAttribute sqlBinaryAttribute = new(storageTypeEnum);

        Assert.NotNull(sqlBinaryAttribute);
        Assert.NotNull(sqlBinaryAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlBinaryAttribute.SqlTypeDefinition;

        Assert.Equal(expectedSqlDbType, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData(StorageTypeEnum.Fixed, 1, SqlDbType.Binary, "BINARY(1)")]
    [InlineData(StorageTypeEnum.Fixed, 8000, SqlDbType.Binary, "BINARY(8000)")]
    [InlineData(StorageTypeEnum.Var, 1, SqlDbType.VarBinary, "VARBINARY(1)")]
    [InlineData(StorageTypeEnum.Var, 8000, SqlDbType.VarBinary, "VARBINARY(8000)")]
    public void Constructor_WithSize(StorageTypeEnum storageTypeEnum, int size, SqlDbType expectedSqlDbType, string expectedTypeDeclaration)
    {
        SqlBinaryAttribute sqlBinaryAttribute = new(storageTypeEnum, size);

        Assert.NotNull(sqlBinaryAttribute);
        Assert.NotNull(sqlBinaryAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlBinaryAttribute.SqlTypeDefinition;

        Assert.Equal(expectedSqlDbType, sqlTypeDefinition.Type);
        Assert.Equal(size, sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Fact]
    public void Constructor_UnsupportedStorageType_Exception()
    {
        StorageTypeEnum unsupportedValue = (StorageTypeEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlBinaryAttribute(unsupportedValue));
    }
}
