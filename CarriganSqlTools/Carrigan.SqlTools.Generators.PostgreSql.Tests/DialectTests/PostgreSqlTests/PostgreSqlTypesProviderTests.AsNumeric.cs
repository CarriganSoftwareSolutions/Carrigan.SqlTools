using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsNumeric_Default_ReturnsNumeric()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsNumeric(false);

        AssertFieldProperties(actual, "NUMERIC");
    }

    [Fact]
    public void AsNumeric_NullableTrue_ReturnsNullableNumeric()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsNumeric(false, true);

        AssertFieldProperties(actual, "NUMERIC", isNullable: true);
    }

    [Fact]
    public void AsNumeric_WithPrecision_ReturnsNumeric()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsNumeric(18, false);

        AssertFieldProperties(actual, "NUMERIC", precision: 18);
    }

    [Fact]
    public void AsNumeric_WithPrecisionNullableTrue_ReturnsNullableNumeric()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsNumeric(18, false, true);

        AssertFieldProperties(actual, "NUMERIC", precision: 18, isNullable: true);
    }

    [Fact]
    public void AsNumeric_WithPrecisionAndScale_ReturnsNumeric()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsNumeric(18, 2, false);

        AssertFieldProperties(actual, "NUMERIC", precision: 18, scale: 2);
    }

    [Fact]
    public void AsNumeric_WithPrecisionAndScaleNullableTrue_ReturnsNullableNumeric()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsNumeric(18, 2, false, true);

        AssertFieldProperties(actual, "NUMERIC", precision: 18, scale: 2, isNullable: true);
    }

    [Fact]
    public void AsNumeric_ZeroPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsNumeric(0, false));

    [Fact]
    public void AsNumeric_ScaleGreaterThanPrecision_Exception() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsNumeric(2, 3, false));
}
