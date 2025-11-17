using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlTextAttributeTests
{
    [Theory]
    [InlineData(EncodingEnum.Ascii, SqlDbType.Text, "TEXT")]
    [InlineData(EncodingEnum.Unicode, SqlDbType.NText, "NTEXT")]
    public void Constructor_EncodingEnum(EncodingEnum encoding,SqlDbType expectedSqlDbType, string expectedTypeDeclaration)
    {
        SqlTextAttribute sqlTextAttribute = new(encoding);

        Assert.NotNull(sqlTextAttribute);
        Assert.NotNull(sqlTextAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlTextAttribute.SqlTypeDefinition;

        Assert.Equal(expectedSqlDbType, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Fact]
    public void Constructor_EncodingEnum_Exception()
    {
        EncodingEnum unsupportedEncoding = (EncodingEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlTextAttribute(unsupportedEncoding));
    }
}
