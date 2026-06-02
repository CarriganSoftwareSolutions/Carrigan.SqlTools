using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTimestampWithTimeZone_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone(false);

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE");
    }

    [Fact]
    public void AsTimestampWithTimeZone_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone(false, true);

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE", isNullable: true);
    }

    [Fact]
    public void AsTimestampWithTimeZone_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone(6, false);

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTimestampWithTimeZone_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimestampWithTimeZone(6, false, true);

        AssertFieldProperties(actual, "TIMESTAMP WITH TIME ZONE", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTimestampWithTimeZone_InvalidFractionalSecondsPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTimestampWithTimeZone(7, false));
}
