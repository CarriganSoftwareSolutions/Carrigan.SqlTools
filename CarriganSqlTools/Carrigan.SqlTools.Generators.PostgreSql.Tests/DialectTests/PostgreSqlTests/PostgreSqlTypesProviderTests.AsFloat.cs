using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsFloat_ValidPrecision_ReturnsFloat()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsFloat(24);

        AssertFieldProperties(actual, "FLOAT", precision: 24);
    }

    [Fact]
    public void AsFloat_NullableTrue_ReturnsNullableFloat()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsFloat(53, true);

        AssertFieldProperties(actual, "FLOAT", precision: 53, isNullable: true);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)54)]
    public void AsFloat_InvalidPrecision_Exception(byte precision) => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsFloat(precision));
}
