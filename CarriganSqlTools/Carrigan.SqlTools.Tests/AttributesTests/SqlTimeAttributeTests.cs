using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlTimeAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlTimeAttribute sqlTimeAttribute = new();

        Assert.NotNull(sqlTimeAttribute);
        Assert.NotNull(sqlTimeAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlTimeAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Time, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("TIME", sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)0, "TIME(0)")]
    [InlineData((byte)7, "TIME(7)")]
    public void Constructor_WithFractionalSecondPrecision(
        byte fractionalSecondPrecision,
        string expectedTypeDeclaration)
    {
        SqlTimeAttribute sqlTimeAttribute = new(fractionalSecondPrecision);

        Assert.NotNull(sqlTimeAttribute);
        Assert.NotNull(sqlTimeAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlTimeAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Time, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Equal(fractionalSecondPrecision, sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)8)]
    [InlineData((byte)9)]
    public void Constructor_WithFractionalSecondPrecision_Exception(byte fractionalSecondPrecision) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlTimeAttribute(fractionalSecondPrecision)); }
