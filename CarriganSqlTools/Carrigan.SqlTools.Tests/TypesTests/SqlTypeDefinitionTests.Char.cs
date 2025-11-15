using System.Data;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;
using Xunit;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(4000)]
    [InlineData(8000)]
    public void AsChar_WithValidSizes(int size)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsChar(size);
        Assert.Equal(SqlDbType.Char, definition.Type);
        Assert.Equal(size, definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2000)]
    [InlineData(4000)]
    public void AsNChar_WithValidSizes(int size)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsNChar(size);
        Assert.Equal(SqlDbType.NChar, definition.Type);
        Assert.Equal(size, definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8001)]
    public void AsChar_OutOfRange_Throws(int size) => 
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsChar(size));

    [Theory]
    [InlineData(0)]
    [InlineData(4001)]
    public void AsNChar_OutOfRange_Throws(int size) => 
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsNChar(size));

    [Theory]
    [InlineData(null, "VARCHAR")]
    [InlineData(1, "VARCHAR(1)")]
    [InlineData(8000, "VARCHAR(8000)")]
    public void AsVarChar(int? size, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsVarChar(size);
        Assert.Equal(SqlDbType.VarChar, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(size, definition.Size);
        Assert.False(definition.UseMax);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8001)]
    public void AsVarChar_OutOfRange(int size) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsVarChar(size));

    [Theory]
    [InlineData(null, "NVARCHAR")]
    [InlineData(1, "NVARCHAR(1)")]
    [InlineData(4000, "NVARCHAR(4000)")]
    public void AsNVarChar(int? size, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsNVarChar(size);
        Assert.Equal(SqlDbType.NVarChar, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(size, definition.Size);
        Assert.False(definition.UseMax);
    }

}
