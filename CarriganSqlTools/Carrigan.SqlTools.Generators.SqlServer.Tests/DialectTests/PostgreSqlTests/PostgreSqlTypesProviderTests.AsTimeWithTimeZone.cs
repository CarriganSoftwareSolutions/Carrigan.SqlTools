using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;
using System;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsTimeWithTimeZone_Default_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithTimeZone();

        AssertFieldProperties(actual, "TIME WITH TIME ZONE");
    }

    [Fact]
    public void AsTimeWithTimeZone_NullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithTimeZone(true);

        AssertFieldProperties(actual, "TIME WITH TIME ZONE", isNullable: true);
    }

    [Fact]
    public void AsTimeWithTimeZone_WithFractionalSecondsPrecision_ReturnsExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithTimeZone(6);

        AssertFieldProperties(actual, "TIME WITH TIME ZONE", fractionalSecondsPrecision: 6);
    }

    [Fact]
    public void AsTimeWithTimeZone_WithFractionalSecondsPrecisionNullableTrue_ReturnsNullableExpectedType()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsTimeWithTimeZone(6, true);

        AssertFieldProperties(actual, "TIME WITH TIME ZONE", fractionalSecondsPrecision: 6, isNullable: true);
    }

    [Fact]
    public void AsTimeWithTimeZone_InvalidFractionalSecondsPrecision_Exception() => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsTimeWithTimeZone(7));
}
