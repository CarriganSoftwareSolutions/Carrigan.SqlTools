using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTimeWithoutTimeZone_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone(false);

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE");
    }

    [Fact]
    public void AsTimeWithoutTimeZone_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone(false, true);

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE", isNullable: true);
    }

    [Fact]
    public void AsTimeWithoutTimeZone_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone(6, false);

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTimeWithoutTimeZone_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone(6, false, true);

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTimeWithoutTimeZone_InvalidFractionalSecondsPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTimeWithoutTimeZone(7, false));
}
