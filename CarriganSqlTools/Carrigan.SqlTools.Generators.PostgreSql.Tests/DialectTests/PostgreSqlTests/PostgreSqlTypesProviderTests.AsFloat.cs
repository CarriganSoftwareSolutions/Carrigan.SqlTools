using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsFloat_ValidPrecision_ReturnsFloat()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsFloat(24, false);

        AssertFieldProperties(actual, "FLOAT", precision: 24);
    }

    [Fact]
    public void AsFloat_NullPrecision_ReturnsFloat()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsFloat(null, false);

        AssertFieldProperties(actual, "FLOAT", precision: null);
    }

    [Fact]
    public void AsFloat_NullableTrue_ReturnsNullableFloat()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsFloat(53, false, true);

        AssertFieldProperties(actual, "FLOAT", precision: 53, isNullable: true);
    }

    [Fact]
    public void AsFloat_NullPrecision_NullableTrue_ReturnsNullableFloat()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsFloat(null, false, true);

        AssertFieldProperties(actual, "FLOAT", precision: null, isNullable: true);
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)54)]
    public void AsFloat_InvalidPrecision_Exception(byte precision) => 
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsFloat(precision, false));
}
