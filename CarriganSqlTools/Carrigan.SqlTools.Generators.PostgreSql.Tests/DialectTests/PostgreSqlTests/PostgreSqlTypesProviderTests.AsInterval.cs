using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsInterval_ReturnsInterval()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsInterval();

        AssertFieldProperties(actual, "INTERVAL");
    }

    [Fact]
    public void AsInterval_NullableTrue_ReturnsNullableInterval()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsInterval(true);

        AssertFieldProperties(actual, "INTERVAL", isNullable: true);
    }
}
