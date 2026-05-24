using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsVarChar_ValidLength_ReturnsVarChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsVarChar(50);

        AssertFieldProperties(actual, "VARCHAR", length: 50, isUnicode: true, isFixedLength: false);
    }

    [Fact]
    public void AsVarChar_NullableTrue_ReturnsNullableVarChar()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsVarChar(50, true);

        AssertFieldProperties(actual, "VARCHAR", length: 50, isUnicode: true, isFixedLength: false, isNullable: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AsVarChar_InvalidLength_Exception(int length)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsVarChar(length));
    }
}
