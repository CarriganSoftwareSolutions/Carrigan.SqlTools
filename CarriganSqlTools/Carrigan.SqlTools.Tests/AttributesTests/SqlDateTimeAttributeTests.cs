using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;


public sealed class SqlDateTimeAttributeTests
{
    [Theory]
    [InlineData(SizeableEnum.Regular, SqlDbType.DateTime, "DATETIME")]
    [InlineData(SizeableEnum.Smaller, SqlDbType.SmallDateTime, "SMALLDATETIME")]
    public void Constructor_SizeableEnum(
        SizeableEnum dateTimeSize,
        SqlDbType expectedSqlDbType,
        string expectedTypeDeclaration)
    {
        SqlDateTimeAttribute sqlDateTimeAttribute = new(dateTimeSize);

        Assert.NotNull(sqlDateTimeAttribute);
        Assert.NotNull(sqlDateTimeAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlDateTimeAttribute.SqlTypeDefinition;

        Assert.Equal(expectedSqlDbType, sqlTypeDefinition.Type);
        Assert.Null(sqlTypeDefinition.Size);
        Assert.False(sqlTypeDefinition.UseMax);
        Assert.Null(sqlTypeDefinition.Precision);
        Assert.Null(sqlTypeDefinition.Scale);
        Assert.Equal(expectedTypeDeclaration, sqlTypeDefinition.TypeDeclaration);
    }

    [Fact]
    public void Constructor_SizeableEnum_Exception()
    {
        SizeableEnum unsupportedValue = (SizeableEnum)999;

        Assert.Throws<NotSupportedException>(() => new SqlDateTimeAttribute(unsupportedValue));
    }
}
