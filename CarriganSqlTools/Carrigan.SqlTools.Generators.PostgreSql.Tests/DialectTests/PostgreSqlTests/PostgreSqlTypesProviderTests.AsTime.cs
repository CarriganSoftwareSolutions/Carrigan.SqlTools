using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTime_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTime(false);

        AssertFieldProperties(actual, "TIME");
    }

    [Fact]
    public void AsTime_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTime(false, true);

        AssertFieldProperties(actual, "TIME", isNullable: true);
    }

    [Fact]
    public void AsTime_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTime(6, false);

        AssertFieldProperties(actual, "TIME", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTime_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTime(6, false, true);

        AssertFieldProperties(actual, "TIME", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTime_InvalidFractionalSecondsPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTime(7, false));
}
