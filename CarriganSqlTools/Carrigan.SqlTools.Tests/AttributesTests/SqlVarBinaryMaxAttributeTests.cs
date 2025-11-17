using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlVarBinaryMaxAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlVarBinaryMaxAttribute sqlVarBinaryMaxAttribute = new();

        Assert.NotNull(sqlVarBinaryMaxAttribute);
        Assert.NotNull(sqlVarBinaryMaxAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlVarBinaryMaxAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.VarBinary, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.True(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("VARBINARY(MAX)", sqlTypeDefinition.TypeDeclaration);
    }
}
