using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsVector_ValidDimensions_ReturnsVector()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsVector(1536);

        AssertFieldProperties(actual, "VECTOR", length: 1536);
    }

    [Fact]
    public void AsVector_NullableTrue_ReturnsNullableVector()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsVector(1536, true);

        AssertFieldProperties(actual, "VECTOR", length: 1536, isNullable: true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AsVector_InvalidDimensions_Exception(int dimensions) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PostgreSqlTypesProvider.AsVector(dimensions));
}
