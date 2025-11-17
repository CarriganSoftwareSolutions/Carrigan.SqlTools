using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlDecimalAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDecimalAttribute sqlDecimalAttribute = new();

        Assert.NotNull(sqlDecimalAttribute);
        Assert.NotNull(sqlDecimalAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDecimalAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Decimal, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("DECIMAL", sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)1, "DECIMAL(1)")]
    [InlineData((byte)38, "DECIMAL(38)")]
    public void Constructor_WithPrecision(byte precision, string expectedTypeDeclaration)
    {
        SqlDecimalAttribute sqlDecimalAttribute = new(precision);

        Assert.NotNull(sqlDecimalAttribute);
        Assert.NotNull(sqlDecimalAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDecimalAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Decimal, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Equal(precision, sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)1, (byte)0, "DECIMAL(1, 0)")]
    [InlineData((byte)10, (byte)2, "DECIMAL(10, 2)")]
    [InlineData((byte)38, (byte)0, "DECIMAL(38, 0)")]
    public void Constructor_WithPrecisionAndScale(byte precision, byte scale, string expectedTypeDeclaration)
    {
        SqlDecimalAttribute sqlDecimalAttribute = new(precision, scale);

        Assert.NotNull(sqlDecimalAttribute);
        Assert.NotNull(sqlDecimalAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDecimalAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Decimal, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Equal(precision, sqlTypeDefinition.Precision);
        Assert.Equal(scale, sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)39)]
    public void Constructor_WithPrecision_Exception(byte precision)
    {
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(
            () => new SqlDecimalAttribute(precision));
    }

    [Theory]
    // Precision out of range (must be 1–38)
    [InlineData((byte)0, (byte)0)]   // invalid precision
    [InlineData((byte)39, (byte)0)]   // invalid precision
    // Scale greater than precision (invalid)
    [InlineData((byte)5, (byte)6)]
    [InlineData((byte)10, (byte)11)]
    public void Constructor_WithPrecisionAndScale_Exception(byte precision, byte scale)
    {
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(
            () => new SqlDecimalAttribute(precision, scale));
    }
}
