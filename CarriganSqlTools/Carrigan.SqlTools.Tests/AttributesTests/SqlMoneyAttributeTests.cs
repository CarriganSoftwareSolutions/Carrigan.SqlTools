using System.Data;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.AttributesTests;

public sealed class SqlMoneyAttributeTests
{
    [Theory]
    [InlineData(SizeableEnum.Regular, SqlDbType.Money, "MONEY")]
    [InlineData(SizeableEnum.Smaller, SqlDbType.SmallMoney, "SMALLMONEY")]
    public void Constructor_SizeableEnum(
        SizeableEnum moneySize,
        SqlDbType expectedSqlDbType,
        string expectedTypeDeclaration)
    {
        SqlMoneyAttribute sqlMoneyAttribute = new(moneySize);

        Assert.NotNull(sqlMoneyAttribute);
        Assert.NotNull(sqlMoneyAttribute.SqlTypeDefinition);

        SqlTypeDefinition sqlTypeDefinition = sqlMoneyAttribute.SqlTypeDefinition;

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

        Assert.Throws<NotSupportedException>(() => new SqlMoneyAttribute(unsupportedValue));
    }
}
