using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;


public sealed class SqlFloatAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlFloatAttribute sqlFloatAttribute = new();

        Assert.NotNull(sqlFloatAttribute);
        Assert.NotNull(sqlFloatAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlFloatAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Float, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("FLOAT", sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)1, "FLOAT(1)")]
    [InlineData((byte)24, "FLOAT(24)")]
    [InlineData((byte)53, "FLOAT(53)")]
    public void Constructor_WithPrecision(byte precision, string expectedTypeDeclaration)
    {
        SqlFloatAttribute sqlFloatAttribute = new(precision);

        Assert.NotNull(sqlFloatAttribute);
        Assert.NotNull(sqlFloatAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlFloatAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Float, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Equal(precision, sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)54)]
    [InlineData((byte)100)]
    public void Constructor_WithPrecision_Exception(byte precision) => 
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => new SqlFloatAttribute(precision));
}
