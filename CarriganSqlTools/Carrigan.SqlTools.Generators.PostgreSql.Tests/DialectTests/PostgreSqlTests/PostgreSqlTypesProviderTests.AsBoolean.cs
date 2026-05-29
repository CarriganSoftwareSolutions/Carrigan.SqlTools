using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsBoolean_Default_ReturnsBoolean()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBoolean();

        AssertFieldProperties(actual, "BOOLEAN");
    }

    [Fact]
    public void AsBoolean_NullableTrue_ReturnsNullableBoolean()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBoolean(true);

        AssertFieldProperties(actual, "BOOLEAN", isNullable: true);
    }
}
