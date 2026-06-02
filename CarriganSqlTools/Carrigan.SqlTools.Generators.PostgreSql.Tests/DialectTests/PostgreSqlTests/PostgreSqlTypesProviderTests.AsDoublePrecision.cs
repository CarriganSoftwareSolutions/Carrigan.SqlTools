using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsDoublePrecision_Default_ReturnsDoublePrecision()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsDoublePrecision(false);

        AssertFieldProperties(actual, "DOUBLE PRECISION");
    }

    [Fact]
    public void AsDoublePrecision_NullableTrue_ReturnsNullableDoublePrecision()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsDoublePrecision(false, true);

        AssertFieldProperties(actual, "DOUBLE PRECISION", isNullable: true);
    }
}
