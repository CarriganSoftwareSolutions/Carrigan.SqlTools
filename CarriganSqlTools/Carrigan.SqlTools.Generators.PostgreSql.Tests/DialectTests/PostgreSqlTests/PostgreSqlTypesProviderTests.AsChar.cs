using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsChar_ValidLength_ReturnsChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsChar(10);

        AssertFieldProperties(actual, "CHAR", length: 10, isUnicode: true, isFixedLength: true);
    }

    [Fact]
    public void AsChar_NullableTrue_ReturnsNullableChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsChar(10, true);

        AssertFieldProperties(actual, "CHAR", length: 10, isUnicode: true, isFixedLength: true, isNullable: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AsChar_InvalidLength_Exception(int length) => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsChar(length));
}
