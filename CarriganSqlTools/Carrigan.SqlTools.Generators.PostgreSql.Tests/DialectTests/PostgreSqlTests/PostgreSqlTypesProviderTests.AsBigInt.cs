using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlTypesProviderTests
{
    [Fact]
    public void AsBigInt_Default_ReturnsBigInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBigInt(false);

        AssertFieldProperties(actual, "BIGINT");
    }

    [Fact]
    public void AsBigInt_NullableTrue_ReturnsNullableBigInt()
    {
        FieldProperties actual = PostgreSqlTypesProvider.AsBigInt(false, true);

        AssertFieldProperties(actual, "BIGINT", isNullable: true);
    }
}
