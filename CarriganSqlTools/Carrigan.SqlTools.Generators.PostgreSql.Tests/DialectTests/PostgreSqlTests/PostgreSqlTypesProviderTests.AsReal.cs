using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsReal_Default_ReturnsReal()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsReal(false);

        AssertFieldProperties(actual, "REAL");
    }

    [Fact]
    public void AsReal_NullableTrue_ReturnsNullableReal()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsReal(false, true);

        AssertFieldProperties(actual, "REAL", isNullable: true);
    }
}
