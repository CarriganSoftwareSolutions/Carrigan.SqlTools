using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlImageAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlImageAttribute sqlImageAttribute = new();

        Assert.NotNull(sqlImageAttribute);
        Assert.NotNull(sqlImageAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlImageAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Image, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("IMAGE", sqlTypeDefinition.TypeDeclaration);
    }
}
