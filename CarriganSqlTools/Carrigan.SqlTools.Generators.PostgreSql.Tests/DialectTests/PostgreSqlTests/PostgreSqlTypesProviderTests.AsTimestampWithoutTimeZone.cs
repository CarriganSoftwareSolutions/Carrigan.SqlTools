using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTimestampWithoutTimeZone_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(false);

        AssertFieldProperties(actual, "TIMESTAMP WITHOUT TIME ZONE");
    }

    [Fact]
    public void AsTimestampWithoutTimeZone_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(false, true);

        AssertFieldProperties(actual, "TIMESTAMP WITHOUT TIME ZONE", isNullable: true);
    }

    [Fact]
    public void AsTimestampWithoutTimeZone_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(6, false);

        AssertFieldProperties(actual, "TIMESTAMP WITHOUT TIME ZONE", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTimestampWithoutTimeZone_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(6, false, true);

        AssertFieldProperties(actual, "TIMESTAMP WITHOUT TIME ZONE", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTimestampWithoutTimeZone_InvalidFractionalSecondsPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTimestampWithoutTimeZone(7, false));
}
