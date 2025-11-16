using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlDateTime2AttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDateTime2Attribute sqlDateTime2Attribute = new();

        Assert.NotNull(sqlDateTime2Attribute);
        Assert.NotNull(sqlDateTime2Attribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDateTime2Attribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.DateTime2, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("DATETIME2", sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)0, "DATETIME2(0)")]
    [InlineData((byte)7, "DATETIME2(7)")]
    public void Constructor_WithValue(byte fractionalSecondPrecision,string expectedTypeDeclaration)
    {
        SqlDateTime2Attribute sqlDateTime2Attribute = new(fractionalSecondPrecision);

        Assert.NotNull(sqlDateTime2Attribute);
        Assert.NotNull(sqlDateTime2Attribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDateTime2Attribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.DateTime2, sqlTypeDefinition.Type);
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
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlDateTime2Attribute(fractionalSecondPrecision));
}
