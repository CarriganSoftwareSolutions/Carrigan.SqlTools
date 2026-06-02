using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsUuid_Default_ReturnsUuid()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsUuid(false);

        AssertFieldProperties(actual, "UUID");
    }

    [Fact]
    public void AsUuid_NullableTrue_ReturnsNullableUuid()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsUuid(false, true);

        AssertFieldProperties(actual, "UUID", isNullable: true);
    }
}
