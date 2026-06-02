using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsJsonB_Default_ReturnsJsonB()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsJsonB(false);

        AssertFieldProperties(actual, "JSONB");
    }

    [Fact]
    public void AsJsonB_NullableTrue_ReturnsNullableJsonB()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsJsonB(false, true);

        AssertFieldProperties(actual, "JSONB", isNullable: true);
    }
}
