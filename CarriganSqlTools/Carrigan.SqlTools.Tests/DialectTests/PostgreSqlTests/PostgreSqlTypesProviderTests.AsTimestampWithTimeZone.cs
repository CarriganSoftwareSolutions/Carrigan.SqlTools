using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTimestampWithTimeZone_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone();

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE");
    }

    [Fact]
    public void AsTimestampWithTimeZone_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone(true);

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE", isNullable: true);
    }

    [Fact]
    public void AsTimestampWithTimeZone_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone(6);

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTimestampWithTimeZone_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone(6, true);

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTimestampWithTimeZone_InvalidFractionalSecondsPrecision_Exception()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTimestampWithTimeZone(7));
    }
}
