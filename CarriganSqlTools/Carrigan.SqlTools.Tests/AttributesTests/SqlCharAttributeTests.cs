using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlCharAttributeTests
{

    [Theory]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, SqlDbType.Char, "CHAR(8000)")]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, SqlDbType.VarChar, "VARCHAR(MAX)")]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, SqlDbType.NChar, "NCHAR(4000)")]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, SqlDbType.NVarChar, "NVARCHAR(MAX)")]
    public void Constructor_EncodingAndStorageType(
        EncodingEnum encoding,
        StorageTypeEnum storageTypeEnum,
        SqlDbType expectedSqlDbType,
        string expectedTypeDeclaration)
    {
        SqlCharAttribute sqlCharAttribute = new(encoding, storageTypeEnum);

        Assert.NotNull(sqlCharAttribute);
        Assert.NotNull(sqlCharAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlCharAttribute.SqlTypeDefinition;

        Assert.Equal(expectedSqlDbType, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 1, SqlDbType.Char, "CHAR(1)")]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 8000, SqlDbType.Char, "CHAR(8000)")]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 1, SqlDbType.VarChar, "VARCHAR(1)")]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 8000, SqlDbType.VarChar, "VARCHAR(8000)")]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 1, SqlDbType.NChar, "NCHAR(1)")]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 4000, SqlDbType.NChar, "NCHAR(4000)")]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 1, SqlDbType.NVarChar, "NVARCHAR(1)")]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 4000, SqlDbType.NVarChar, "NVARCHAR(4000)")]
    public void Constructor_WithSize(
        EncodingEnum encoding,
        StorageTypeEnum storageTypeEnum,
        int size,
        SqlDbType expectedSqlDbType,
        string expectedTypeDeclaration)
    {
        SqlCharAttribute sqlCharAttribute = new(encoding, storageTypeEnum, size);

        Assert.NotNull(sqlCharAttribute);
        Assert.NotNull(sqlCharAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlCharAttribute.SqlTypeDefinition;

        Assert.Equal(expectedSqlDbType, sqlTypeDefinition.Type);
        Assert.Equal(size, sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    // ASCII, fixed-length CHAR: valid 1–8000
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 0)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 8001)]
    // ASCII, variable-length VARCHAR: valid 1–8000
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 0)]
    [InlineData(EncodingEnum.Ascii, StorageTypeEnum.Var, 8001)]
    // Unicode, fixed-length NCHAR: valid 1–4000
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 0)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 4001)]
    // Unicode, variable-length NVARCHAR: valid 1–4000
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 0)]
    [InlineData(EncodingEnum.Unicode, StorageTypeEnum.Var, 4001)]
    public void Constructor_WithSize_OutOfRange_Exception(EncodingEnum encoding, StorageTypeEnum storageTypeEnum, int size) => 
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlCharAttribute(encoding, storageTypeEnum, size));

    [Fact]
    public void Constructor_UnsupportedStorageType()
    {
        EncodingEnum encoding = (EncodingEnum)999;
        StorageTypeEnum storageTypeEnum = StorageTypeEnum.Fixed;

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
