using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsVarBit_ValidLength_ReturnsVarBit()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsVarBit(8);

        AssertFieldProperties(actual, "VARBIT", length: 8, isFixedLength: false);
    }

    [Fact]
    public void AsVarBit_NullableTrue_ReturnsNullableVarBit()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsVarBit(8, true);

        AssertFieldProperties(actual, "VARBIT", length: 8, isFixedLength: false, isNullable: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AsVarBit_InvalidLength_Exception(int length) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsVarBit(length));
}
