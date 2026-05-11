using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsDoublePrecision_Default_ReturnsDoublePrecision()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsDoublePrecision();

        AssertFieldProperties(actual, "DOUBLE PRECISION");
    }

    [Fact]
    public void AsDoublePrecision_NullableTrue_ReturnsNullableDoublePrecision()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsDoublePrecision(true);

        AssertFieldProperties(actual, "DOUBLE PRECISION", isNullable: true);
    }
}
