using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsJson_Default_ReturnsJson()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsJson(false);

        AssertFieldProperties(actual, "JSON");
    }

    [Fact]
    public void AsJson_NullableTrue_ReturnsNullableJson()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsJson(false, true);

        AssertFieldProperties(actual, "JSON", isNullable: true);
    }
}
