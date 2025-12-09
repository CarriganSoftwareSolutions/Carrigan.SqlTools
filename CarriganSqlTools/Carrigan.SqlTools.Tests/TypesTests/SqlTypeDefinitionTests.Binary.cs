using System.Data;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(4000)]
    [InlineData(8000)]
    public void AsBinary_WithValidSizes(int size)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsBinary(size);
        Assert.Equal(SqlDbType.Binary, definition.Type);
        Assert.Equal(size, definition.Size);
        Assert.Null(definition.Precision);
        Assert.Null(definition.Scale);
        Assert.False(definition.UseMax);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8001)]
    public void AsBinary_OutOfRange_Throws(int size) => 
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsBinary(size));

    [Theory]
    [InlineData(0)]
    [InlineData(4001)]
    public void AsNVarChar_OutOfRange(int size) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsNVarChar(size));

    [Theory]
    [InlineData(null, "VARBINARY(MAX)")]
    [InlineData(1, "VARBINARY(1)")]
    [InlineData(8000, "VARBINARY(8000)")]
    public void AsVarBinary(int? size, string expectedDeclaration)
    {
        SqlTypeDefinition definition = SqlTypeDefinition.AsVarBinary(size);
        Assert.Equal(SqlDbType.VarBinary, definition.Type);
        Assert.Equal(expectedDeclaration, definition.TypeDeclaration);
        Assert.Equal(size, definition.Size);
        Assert.False(definition.UseMax);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8001)]
    public void AsVarBinary_OutOfRange(int size) =>
        Assert.Throws<SqlTypeArgumentOutOfRangeException>(() => SqlTypeDefinition.AsVarBinary(size));
}
