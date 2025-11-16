using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlDateTimeOffsetAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDateTimeOffsetAttribute sqlDateTimeOffsetAttribute = new();

        Assert.NotNull(sqlDateTimeOffsetAttribute);
        Assert.NotNull(sqlDateTimeOffsetAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDateTimeOffsetAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.DateTimeOffset, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("DATETIMEOFFSET", sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)0, "DATETIMEOFFSET(0)")]
    [InlineData((byte)7, "DATETIMEOFFSET(7)")]
    public void Constructor_WithValue(byte fractionalSecondPrecision,string expectedTypeDeclaration)
    {
        SqlDateTimeOffsetAttribute sqlDateTimeOffsetAttribute = new(fractionalSecondPrecision);

        Assert.NotNull(sqlDateTimeOffsetAttribute);
        Assert.NotNull(sqlDateTimeOffsetAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDateTimeOffsetAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.DateTimeOffset, sqlTypeDefinition.Type);
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
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlDateTimeOffsetAttribute(fractionalSecondPrecision));
}
