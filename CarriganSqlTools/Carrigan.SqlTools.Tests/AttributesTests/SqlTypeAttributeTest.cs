using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlTypeAttributeTest
{
    [Theory]
    [InlineData(SqlDbType.Int, "INT")]
    [InlineData(SqlDbType.BigInt, "BIGINT")]
    [InlineData(SqlDbType.SmallInt, "SMALLINT")]
    [InlineData(SqlDbType.TinyInt, "TINYINT")]
    [InlineData(SqlDbType.Bit, "BIT")]
    [InlineData(SqlDbType.Float, "FLOAT")]
    [InlineData(SqlDbType.Real, "REAL")]
    [InlineData(SqlDbType.DateTime2, "DATETIME2")]
    [InlineData(SqlDbType.UniqueIdentifier, "UNIQUEIDENTIFIER")]
    [InlineData(SqlDbType.Char, "CHAR")]
    [InlineData(SqlDbType.Time, "TIME")]
    [InlineData(SqlDbType.Date, "DATE")]
    [InlineData(SqlDbType.DateTimeOffset, "DATETIMEOFFSET")]
    public void TypeOnly(SqlDbType expectedType, string expectedKeyword)
    {
        SqlTypeAttribute attribute = new (expectedType);
        SqlTypeDefinition definition = attribute.SqlTypeDefinition;

        Assert.Equal(expectedType, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal(expectedKeyword, definition.TypeDeclaration);
    }

    [Theory]
    [InlineData(SqlDbType.Timestamp)]
    [InlineData(SqlDbType.Structured)]
    [InlineData(SqlDbType.Variant)]
    [InlineData(SqlDbType.Udt)]
    [InlineData(SqlDbType.Xml)]
    [InlineData(SqlDbType.Json)]
    public void TypeOnly_UnsupportedTypes(SqlDbType sqlDbType) => 
        Assert.Throws<SqlTypeNotSupportedException>(() => new SqlTypeAttribute(sqlDbType));

    [Theory]
    [InlineData(SqlDbType.NVarChar, 4000, "NVARCHAR(4000)")]
    [InlineData(SqlDbType.VarChar, 8000, "VARCHAR(8000)")]
    [InlineData(SqlDbType.Char, 1, "CHAR(1)")]
    [InlineData(SqlDbType.VarBinary, 8000, "VARBINARY(8000)")]
    [InlineData(SqlDbType.NChar, 4000, "NCHAR(4000)")]
    [InlineData(SqlDbType.Binary, 8000, "BINARY(8000)")]
    public void WithSize(SqlDbType type, int size, string expectedDeclaration)
    {
        SqlTypeAttribute attribute = new(type, size: size);
        SqlTypeDefinition definition = attribute.SqlTypeDefinition;

        Assert.Equal(type, definition.Type);
        Assert.Equal(size, definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Theory]
    [InlineData(SqlDbType.VarChar, 0)]
    [InlineData(SqlDbType.VarChar, 8001)]
    [InlineData(SqlDbType.NVarChar, 0)]
    [InlineData(SqlDbType.NVarChar, 4001)]
    [InlineData(SqlDbType.Char, 0)]
    [InlineData(SqlDbType.Char, 8001)]
    [InlineData(SqlDbType.NChar, 0)]
    [InlineData(SqlDbType.NChar, 4001)]
    [InlineData(SqlDbType.Binary, 0)]
    [InlineData(SqlDbType.Binary, 8001)]
    [InlineData(SqlDbType.VarBinary, 0)]
    [InlineData(SqlDbType.VarBinary, 8001)]
    public void WithSize_OutOfRange(SqlDbType type, int size) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlTypeAttribute(type, size: size));

    [Theory]
    [InlineData(SqlDbType.Int, 10)]
    [InlineData(SqlDbType.Bit, 1)]
    [InlineData(SqlDbType.Date, 5)]
    [InlineData(SqlDbType.DateTime2, 5)]
    public void WithSize_UnsupportedType(SqlDbType type, int size) =>
        Assert.Throws<SqlTypeDoesNotSupportSizeException>(() => new SqlTypeAttribute(type, size: size));

    [Theory]
    [InlineData(SqlDbType.Float, (byte)53, "FLOAT(53)")]
    [InlineData(SqlDbType.Decimal, (byte)18, "DECIMAL(18)")]
    public void WithPrecision(SqlDbType type, byte precision, string expectedDeclaration)
    {
        SqlTypeAttribute attribute = new(type, precision: precision);
        SqlTypeDefinition definition = attribute.SqlTypeDefinition;

        Assert.Equal(type, definition.Type);
        Assert.Null(definition.Size);
        Assert.Equal(precision, definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Theory]
    [InlineData(SqlDbType.Float, (byte)0)]
    [InlineData(SqlDbType.Float, (byte)54)]
    [InlineData(SqlDbType.Decimal, (byte)0)]
    [InlineData(SqlDbType.Decimal, (byte)39)]
    public void WithPrecision_OutOfRange(SqlDbType type, byte precision) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlTypeAttribute(type, precision: precision));

    [Theory]
    [InlineData(SqlDbType.Int, (byte)5)]
    [InlineData(SqlDbType.Real, (byte)10)]
    [InlineData(SqlDbType.DateTime2, (byte)10)]
    public void WithPrecision_UnsupportedType(SqlDbType type, byte precision) =>
        Assert.Throws<SqlTypeDoesNotSupportPrecisionException>(() => new SqlTypeAttribute(type, precision: precision));

    [Theory]
    [InlineData(SqlDbType.Time, (byte)7, "TIME(7)")]
    [InlineData(SqlDbType.DateTime2, (byte)7, "DATETIME2(7)")]
    [InlineData(SqlDbType.DateTimeOffset, (byte)7, "DATETIMEOFFSET(7)")]
    [InlineData(SqlDbType.Decimal, (byte)4, "DECIMAL(4)")]
    public void WithScale(SqlDbType type, byte scale, string expectedDeclaration)
    {
        SqlTypeAttribute attribute = new(type, scale: scale);
        SqlTypeDefinition definition = attribute.SqlTypeDefinition;

        Assert.Equal(type, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Equal(scale, definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Theory]
    [InlineData(SqlDbType.Time, (byte)8)]
    [InlineData(SqlDbType.DateTime2, (byte)8)]
    [InlineData(SqlDbType.DateTimeOffset, (byte)8)]
    [InlineData(SqlDbType.Decimal, (byte)39)]
    public void WithScale_OutOfRange(SqlDbType type, byte scale) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlTypeAttribute(type, scale: scale));

    [Theory]
    [InlineData(SqlDbType.Int, (byte)1)]
    [InlineData(SqlDbType.Float, (byte)2)]
    [InlineData(SqlDbType.Real, (byte)3)]
    public void WithScale_UnsupportedType(SqlDbType type, byte scale) =>
        Assert.Throws<SqlTypeDoesNotSupportScaleException>(() => new SqlTypeAttribute(type, scale: scale));

    [Fact]
    public void Decimal_PrecisionAndScale()
    {
        SqlTypeAttribute attribute = new(SqlDbType.Decimal, precision: 18, scale: 4);
        SqlTypeDefinition definition = attribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Decimal, definition.Type);
        Assert.Null(definition.Size);
        Assert.Equal((byte)18, definition.Precision);
        Assert.Equal((byte)4, definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("DECIMAL(18)(4)", definition.TypeDeclaration);
    }

    [Theory]
    [InlineData(SqlDbType.NVarChar, true, "NVARCHAR(MAX)")]
    [InlineData(SqlDbType.VarChar, true, "VARCHAR(MAX)")]
    [InlineData(SqlDbType.VarBinary, true, "VARBINARY(MAX)")]
    public void UseMax(SqlDbType type, bool useMax, string expectedDeclaration)
    {
        SqlTypeAttribute attribute = new(type, useMax);
        SqlTypeDefinition definition = attribute.SqlTypeDefinition;

        Assert.Equal(type, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.True(definition.UseMax);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
    }

    [Fact]
    public void UseMax_False()
    {
        SqlTypeAttribute attribute = new(SqlDbType.BigInt, false);
        SqlTypeDefinition definition = attribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.BigInt, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("BIGINT", definition.TypeDeclaration);
    }

    [Theory]
    [InlineData(SqlDbType.Int)]
    [InlineData(SqlDbType.Bit)]
    [InlineData(SqlDbType.Date)]
    public void UseMax_OnUnsupportedType(SqlDbType type) =>
        Assert.Throws<SqlTypeDoesNotSupportSizeException>(() => new SqlTypeAttribute(type, true));

    [Fact]
    public void Float_ScaleNotSupported() =>
        Assert.Throws<SqlTypeDoesNotSupportScaleException>(() => new SqlTypeAttribute(SqlDbType.Float, scale: 1));
}
