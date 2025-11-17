using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlVarCharMaxAttributeTests
{
    [Theory]
    [InlineData(EncodingEnum.Ascii, SqlDbType.VarChar, "VARCHAR(MAX)")]
    [InlineData(EncodingEnum.Unicode, SqlDbType.NVarChar, "NVARCHAR(MAX)")]
    public void Constructor_EncodingEnum(
        EncodingEnum encoding,
        SqlDbType expectedSqlDbType,
        string expectedTypeDeclaration)
    {
        SqlVarCharMaxAttribute sqlVarCharMaxAttribute = new(encoding);

        Assert.NotNull(sqlVarCharMaxAttribute);
        Assert.NotNull(sqlVarCharMaxAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlVarCharMaxAttribute.SqlTypeDefinition;

        Assert.Equal(expectedSqlDbType, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.True(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Fact]
    public void Constructor_EncodingEnum_Exception()
    {
        EncodingEnum unsupportedEncoding = (EncodingEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlVarCharMaxAttribute(unsupportedEncoding));
    }
}
