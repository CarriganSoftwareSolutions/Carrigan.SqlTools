using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

//TODO: Move postge tests to the new postgre unit tests project.
namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTimeWithoutTimeZone_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone();

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE");
    }

    [Fact]
    public void AsTimeWithoutTimeZone_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone(true);

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE", isNullable: true);
    }

    [Fact]
    public void AsTimeWithoutTimeZone_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone(6);

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTimeWithoutTimeZone_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithoutTimeZone(6, true);

        AssertFieldProperties(actual, "TIME WITHOUT TIME ZONE", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTimeWithoutTimeZone_InvalidFractionalSecondsPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTimeWithoutTimeZone(7));
}
