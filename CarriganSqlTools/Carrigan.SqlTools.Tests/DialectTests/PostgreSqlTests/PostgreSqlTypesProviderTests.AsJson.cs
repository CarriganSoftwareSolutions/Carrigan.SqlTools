using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsJson_Default_ReturnsJson()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsJson();

        AssertFieldProperties(actual, "JSON");
    }

    [Fact]
    public void AsJson_NullableTrue_ReturnsNullableJson()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsJson(true);

        AssertFieldProperties(actual, "JSON", isNullable: true);
    }
}
