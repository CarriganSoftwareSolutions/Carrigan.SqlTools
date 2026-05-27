using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTimestamp_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestamp();

        AssertFieldProperties(actual, "TIMESTAMP");
    }

    [Fact]
    public void AsTimestamp_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestamp(true);

        AssertFieldProperties(actual, "TIMESTAMP", isNullable: true);
    }

    [Fact]
    public void AsTimestamp_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestamp(6);

        AssertFieldProperties(actual, "TIMESTAMP", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTimestamp_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestamp(6, true);

        AssertFieldProperties(actual, "TIMESTAMP", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTimestamp_InvalidFractionalSecondsPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTimestamp(7));
}
