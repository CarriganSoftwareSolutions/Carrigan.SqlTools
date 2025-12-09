// File: SqlTypeDefinitionTests.SimpleFactories.cs
using System.Data;
using Carrigan.SqlTools.Types;
using Xunit;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    #region UniqueIdentifier
    [Fact]
    public void AsUniqueIdentifier()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsUniqueIdentifier();
        Assert.Equal(SqlDbType.UniqueIdentifier, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("UNIQUEIDENTIFIER", definition.TypeDeclaration);
    }
    #endregion

    #region char

    [Fact]
    public void AsChar_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsChar();
        Assert.Equal(SqlDbType.Char, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("CHAR(8000)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsNChar_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsNChar();
        Assert.Equal(SqlDbType.NChar, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("NCHAR(4000)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsVarChar_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsVarChar();
        Assert.Equal(SqlDbType.VarChar, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("VARCHAR(MAX)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsNVarChar_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsNVarChar();
        Assert.Equal(SqlDbType.NVarChar, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("NVARCHAR(MAX)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsVarCharMax()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsVarCharMax();
        Assert.Equal(SqlDbType.VarChar, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.True(definition.UseMax);
        Assert.Equal("VARCHAR(MAX)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsNVarCharMax()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsNVarCharMax();
        Assert.Equal(SqlDbType.NVarChar, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.True(definition.UseMax);
        Assert.Equal("NVARCHAR(MAX)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsText()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsText();
        Assert.Equal(SqlDbType.Text, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("TEXT", definition.TypeDeclaration);
    }

    [Fact]
    public void AsNText()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsNText();
        Assert.Equal(SqlDbType.NText, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("NTEXT", definition.TypeDeclaration);
    }

    #endregion

    #region Binary

    [Fact]
    public void AsBinary_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsBinary();
        Assert.Equal(SqlDbType.Binary, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("BINARY(8000)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsVarBinary_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsVarBinary();
        Assert.Equal(SqlDbType.VarBinary, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("VARBINARY(MAX)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsVarBinaryMax()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsVarBinaryMax();
        Assert.Equal(SqlDbType.VarBinary, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.True(definition.UseMax);
        Assert.Equal("VARBINARY(MAX)", definition.TypeDeclaration);
    }

    [Fact]
    public void AsImage()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsImage();
        Assert.Equal(SqlDbType.Image, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("IMAGE", definition.TypeDeclaration);
    }

    #endregion

    #region Bit

    [Fact]
    public void AsBit()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsBit();
        Assert.Equal(SqlDbType.Bit, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("BIT", definition.TypeDeclaration);
    }

    #endregion

    #region Integer

    [Fact]
    public void AsTinyInt()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsTinyInt();
        Assert.Equal(SqlDbType.TinyInt, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("TINYINT", definition.TypeDeclaration);
    }

    [Fact]
    public void AsSmallInt()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsSmallInt();
        Assert.Equal(SqlDbType.SmallInt, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("SMALLINT", definition.TypeDeclaration);
    }

    [Fact]
    public void AsInt()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsInt();
        Assert.Equal(SqlDbType.Int, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("INT", definition.TypeDeclaration);
    }

    [Fact]
    public void AsBigInt()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsBigInt();
        Assert.Equal(SqlDbType.BigInt, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("BIGINT", definition.TypeDeclaration);
    }

    #endregion
    
    #region Floating Point

    [Fact]
    public void AsReal()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsReal();
        Assert.Equal(SqlDbType.Real, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("REAL", definition.TypeDeclaration);
    }

    #endregion

    #region  Decimal

    [Fact]
    public void AsDecimal_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDecimal();
        Assert.Equal(SqlDbType.Decimal, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("DECIMAL", definition.TypeDeclaration);
    }

    [Fact]
    public void AsMoney()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsMoney();
        Assert.Equal(SqlDbType.Money, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("MONEY", definition.TypeDeclaration);
    }

    [Fact]
    public void AsSmallMoney()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsSmallMoney();
        Assert.Equal(SqlDbType.SmallMoney, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("SMALLMONEY", definition.TypeDeclaration);
    }

    #endregion

    #region Date/Time

    [Fact]
    public void AsDateTime2_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDateTime2();
        Assert.Equal(SqlDbType.DateTime2, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale); // no fractional precision supplied
        Assert.False(definition.UseMax);
        Assert.Equal("DATETIME2", definition.TypeDeclaration);
    }

    [Fact]
    public void AsDateTimeOffset_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDateTimeOffset();
        Assert.Equal(SqlDbType.DateTimeOffset, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale); // no fractional precision supplied
        Assert.False(definition.UseMax);
        Assert.Equal("DATETIMEOFFSET", definition.TypeDeclaration);
    }

    [Fact]
    public void AsDateTime()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDateTime();
        Assert.Equal(SqlDbType.DateTime, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("DATETIME", definition.TypeDeclaration);
    }

    [Fact]
    public void AsSmallDateTime()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsSmallDateTime();
        Assert.Equal(SqlDbType.SmallDateTime, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("SMALLDATETIME", definition.TypeDeclaration);
    }

    [Fact]
    public void AsDate()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsDate();
        Assert.Equal(SqlDbType.Date, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
        Assert.Equal("DATE", definition.TypeDeclaration);
    }

    [Fact]
    public void AsTime_NoArgs()
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsTime();
        Assert.Equal(SqlDbType.Time, definition.Type);
        Assert.Null(definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale); // no fractional precision supplied
        Assert.False(definition.UseMax);
        Assert.Equal("TIME", definition.TypeDeclaration);
    }

    #endregion
}
