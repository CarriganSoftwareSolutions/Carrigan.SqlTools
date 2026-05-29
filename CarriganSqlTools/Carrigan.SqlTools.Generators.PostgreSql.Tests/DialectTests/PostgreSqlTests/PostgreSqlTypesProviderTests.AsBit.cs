using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsBit_ValidLength_ReturnsBit()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBit(8);

        AssertFieldProperties(actual, "BIT", length: 8, isFixedLength: true);
    }

    [Fact]
    public void AsBit_NullableTrue_ReturnsNullableBit()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBit(8, true);

        AssertFieldProperties(actual, "BIT", length: 8, isFixedLength: true, isNullable: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AsBit_InvalidLength_Exception(int length) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsBit(length));
}
