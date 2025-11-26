using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlDateAttributeTests
{
    [Fact]
    public void Constructor()
    {
        SqlDateAttribute sqlDateAttribute = new();

        Assert.NotNull(sqlDateAttribute);
        Assert.NotNull(sqlDateAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDateAttribute.SqlTypeDefinition;

        Assert.Equal(SqlDbType.Date, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal("DATE", sqlTypeDefinition.TypeDeclaration);
    }
}
