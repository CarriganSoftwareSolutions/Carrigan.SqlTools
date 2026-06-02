using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsMoney_Default_ReturnsMoney()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsMoney(false);

        AssertFieldProperties(actual, "MONEY");
    }

    [Fact]
    public void AsMoney_NullableTrue_ReturnsNullableMoney()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsMoney(false, true);

        AssertFieldProperties(actual, "MONEY", isNullable: true);
    }
}
