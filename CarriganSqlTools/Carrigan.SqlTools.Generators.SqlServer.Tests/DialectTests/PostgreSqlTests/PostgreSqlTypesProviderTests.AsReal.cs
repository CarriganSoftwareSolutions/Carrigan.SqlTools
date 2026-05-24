using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsReal_Default_ReturnsReal()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsReal();

        AssertFieldProperties(actual, "REAL");
    }

    [Fact]
    public void AsReal_NullableTrue_ReturnsNullableReal()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsReal(true);

        AssertFieldProperties(actual, "REAL", isNullable: true);
    }
}
